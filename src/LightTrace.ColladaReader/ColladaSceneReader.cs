using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LightTrace.Domain;
using LightTrace.Domain.Nodes;
using Microsoft.Xna.Framework;

namespace LightTrace.ColladaReader
{
	public class ColladaSceneReader
	{
		private static readonly XNamespace Ns = @"http://www.collada.org/2005/11/COLLADASchema";
		private XmlNamespaceManager _nsMgr;

		private XDocument _document;
		private Scene _scene;

		public Scene Load(string path)
		{
			if (!File.Exists(path))
				throw new IOException(string.Format("File '{0}' not found", path));

			_document = XDocument.Load(path);

			_nsMgr = new XmlNamespaceManager(new NameTable());
			_nsMgr.AddNamespace("c", _document.Root.GetDefaultNamespace().NamespaceName);

			LoadScene();

			return _scene;
		}

		private void LoadScene()
		{
			XElement sceneElement = _document.XPathSelectElement("//c:library_visual_scenes/c:visual_scene", _nsMgr);


			_scene = new Scene
			         	{
			         		Id = sceneElement.Attribute("id").Value,
			         		Name = sceneElement.Attribute("name").Value
			         	};

			foreach (var nodeElement in sceneElement.Elements(Ns + "node"))
			{
				XElement instanceElement = nodeElement.XPathSelectElement("*[contains(local-name(),'instance_')]", _nsMgr);
				string instanceType = instanceElement.Name.LocalName;
				string instanceUrl = instanceElement.Attribute("url").Value.Substring(1);

				switch (instanceType)
				{
					case "instance_camera":
						{
							Camera camera = new Camera();

							ReadNodeInfo(camera, nodeElement);
							ReadNodeTransform(camera, nodeElement);
							ReadCameraInfo(camera, instanceUrl);
							_scene.Nodes.Add(camera);
							break;
						}
					case "instance_light":
						{
							Light light = ReadLight(instanceUrl);

							ReadNodeInfo(light, nodeElement);
							ReadNodeTransform(light, nodeElement);
							_scene.Nodes.Add(light);
							break;
						}
					case "instance_geometry":
						{
							MeshGeometry mesh = new MeshGeometry();

							ReadNodeInfo(mesh, nodeElement);
							ReadNodeTransform(mesh, nodeElement);
							ReadMesh(mesh, instanceUrl);
							_scene.Nodes.Add(mesh);
							break;
						}
				}
			}
		}

		private void ReadMesh(MeshGeometry mesh, string url)
		{
			XElement instanceElement = _document.XPathSelectElement(string.Format("//c:library_geometries/c:geometry[@id='{0}']", url), _nsMgr);
			XElement meshElement = instanceElement.XPathSelectElement("c:mesh", _nsMgr);

			XElement polylistElement = meshElement.Element(Ns + "polylist");
			string materialId = polylistElement.GetAttributeValue("material", false);
			int polygonCount = polylistElement.GetMandatoryAttributeIntValue("count");

			string verticesSourceId = meshElement.XPathSelectElement("c:vertices/c:input", _nsMgr).Attribute("source").Value.Substring(1);
			string normalsSourceId = polylistElement.XPathSelectElement("c:input[@semantic='NORMAL']", _nsMgr).Attribute("source").Value.Substring(1);

			float[] vertices = ReadMeshData(meshElement, verticesSourceId);
			float[] normals = ReadMeshData(meshElement, normalsSourceId);
			float[] texcoord = null;

			XElement texCoordElement = polylistElement.XPathSelectElement("c:input[@semantic='TEXCOORD']", _nsMgr);
			if (texCoordElement != null)
			{
				string texcoordSourceId = texCoordElement.Attribute("source").Value.Substring(1);
				texcoord = ReadMeshData(meshElement, texcoordSourceId);
			}

			int[] vertexCounts = polylistElement.Element(Ns + "vcount").Value.ToIntArray();
			int[] poligonIndexes = polylistElement.Element(Ns + "p").Value.ToIntArray();

			mesh.BuildMesh(vertexCounts, poligonIndexes, vertices, normals, texcoord);
		}

		private float[] ReadMeshData(XElement meshElement, string sourceId)
		{
			XElement sourceElement = meshElement.XPathSelectElement(string.Format("c:source[@id='{0}']", sourceId), _nsMgr);
			XElement arrayElement = sourceElement.Element(Ns + "float_array");

			return arrayElement.Value.ToFloatArray();
		}

		private Light ReadLight(string url)
		{
			XElement instanceElement = _document.XPathSelectElement(string.Format("//c:library_lights/c:light[@id='{0}']", url), _nsMgr);
			XElement lightElement = instanceElement.XPathSelectElement("c:technique_common/node()", _nsMgr);

			switch (lightElement.Name.LocalName)
			{
				case "point":
					{
						PointLight light = new PointLight();

						light.Color = lightElement.Element(Ns + "color").Value.ToVec3();
						light.AttenuationConst = lightElement.Element(Ns + "constant_attenuation").Value.ToFloat();
						light.AttenuationLinear = lightElement.Element(Ns + "linear_attenuation").Value.ToFloat();
						light.AttenuationQuadratic = lightElement.Element(Ns + "quadratic_attenuation").Value.ToFloat();

						return light;
					}
				case "directional":
					{
						DirectionalLight light = new DirectionalLight();

						light.Color = lightElement.Element(Ns + "color").Value.ToVec3();
						return light;
					}
				default:
					{
						throw new Exception("Unknown light type");
					}
			}
		}

		private void ReadCameraInfo(Camera camera, string url)
		{
			XElement instanceElement = _document.XPathSelectElement(string.Format("//c:library_cameras/c:camera[@id='{0}']", url), _nsMgr);
			XElement cameraElement = instanceElement.XPathSelectElement("c:optics/c:technique_common/c:perspective", _nsMgr);

			camera.XFov = cameraElement.Element(Ns + "xfov").Value.ToFloat();
			camera.AspectRatio = cameraElement.Element(Ns + "aspect_ratio").Value.ToFloat();
		}

		private void ReadNodeInfo(Node node, XElement nodeElement)
		{
			node.Id = nodeElement.Attribute("id").Value;
			node.Name = nodeElement.Attribute("name").Value;
		}

		private void ReadNodeTransform(Node node, XElement nodeElement)
		{
			Vector3 translate = nodeElement.Element(Ns + "translate").Value.ToVec3();
			node.Translation = Matrix.CreateTranslation(translate);

			Matrix rotation = Matrix.Identity;
			IEnumerable<XElement> rotations = nodeElement.Elements(Ns + "rotate");
			foreach (var rotationElement in rotations)
			{
				Vector3 rot = rotationElement.Value.ToVec3();
				float angle = rotationElement.Value.ToFloat(3);

				rotation = Transform.Rotate(angle, rot)*rotation;
			}
			node.Rotation = rotation;

			Vector3 scale = nodeElement.Element(Ns + "scale").Value.ToVec3();
			node.Scale = Matrix.CreateScale(scale);
		}
	}
}