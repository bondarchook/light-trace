﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LightTrace.Domain;
using LightTrace.Domain.Nodes;
using LightTrace.Domain.Shading;
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

		private Material ReadMaterial(string id)
		{
			XElement instanceElement = _document.XPathSelectElement(string.Format("//c:library_materials/c:material[@id='{0}']/c:instance_effect", id), _nsMgr);
			string effectId = instanceElement.Attribute("url").Value.Substring(1);

			XElement effectInstanceElement = _document.XPathSelectElement(string.Format("//c:library_effects/c:effect[@id='{0}']", effectId), _nsMgr);
			XElement profileElement = effectInstanceElement.XPathSelectElement("c:profile_COMMON", _nsMgr);
			XElement effectElement = profileElement.XPathSelectElement("c:technique[@sid='common']/c:phong", _nsMgr);

			Material material = new Material();

			material.EmissionColor = ReadColor(effectElement, "c:emission/c:color");
			material.AmbientColor = ReadColor(effectElement, "c:ambient/c:color");
			material.DiffuseColor = ReadColorSampler(profileElement, effectElement, "c:diffuse/node()");
			material.SpecularColor = ReadColor(effectElement, "c:specular/c:color");
			material.ReflectiveColor = ReadColor(effectElement, "c:reflective/c:color");

			material.Shininess = ReadFloat(effectElement, "c:shininess/c:float");
			material.Reflectivity = ReadFloat(effectElement, "c:reflectivity/c:float");

			return material;
		}

		private ColorSampler ReadColorSampler(XElement profileElement, XElement effectElement, string xpath)
		{
			XElement shaiderElement = effectElement.XPathSelectElement(xpath, _nsMgr);

			if (shaiderElement.Name.LocalName == "color")
			{
				return new PlaneColorSampler(shaiderElement == null ? Vector3.Zero : shaiderElement.Value.ToVec3());
			}
			else if (shaiderElement.Name.LocalName == "texture")
			{
				string textureSamplerID = shaiderElement.Attribute("texture").Value;
				string textureSurfcaeID = profileElement.XPathSelectElement(string.Format("c:newparam[@sid='{0}']/c:sampler2D/c:source", textureSamplerID), _nsMgr).Value;
				string textureImageID = profileElement.XPathSelectElement(string.Format("c:newparam[@sid='{0}']/c:surface/c:init_from", textureSurfcaeID), _nsMgr).Value;
				string texturePath = _document.XPathSelectElement(string.Format("//c:library_images/c:image[@id='{0}']/c:init_from", textureImageID), _nsMgr).Value.Trim('/');

				Texture texture = new Texture(texturePath);
				_scene.Textures.Add(texture);

				return new Texture2DSampler(texture);
			}

			throw new Exception("Unknown surface shader");
		}

		private Vector3 ReadColor(XElement effectElement, string xpath)
		{
			XElement colorElement = effectElement.XPathSelectElement(xpath, _nsMgr);
			return colorElement == null ? Vector3.Zero : colorElement.Value.ToVec3();
		}

		private float ReadFloat(XElement effectElement, string xpath)
		{
			XElement floatElement = effectElement.XPathSelectElement(xpath, _nsMgr);
			return floatElement == null ? 0 : floatElement.Value.ToFloat();
		}


		private void ReadMesh(MeshGeometry mesh, string url)
		{
			XElement instanceElement = _document.XPathSelectElement(string.Format("//c:library_geometries/c:geometry[@id='{0}']", url), _nsMgr);
			XElement meshElement = instanceElement.XPathSelectElement("c:mesh", _nsMgr);

			XElement polylistElement = meshElement.Element(Ns + "polylist");

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

			string materialId = polylistElement.GetAttributeValue("material", false);
			Material material = ReadMaterial(materialId);

			mesh.BuildMesh(vertexCounts, poligonIndexes, vertices, normals, texcoord, material);
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

			camera.Fov = cameraElement.Element(Ns + "xfov").Value.ToFloat();
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