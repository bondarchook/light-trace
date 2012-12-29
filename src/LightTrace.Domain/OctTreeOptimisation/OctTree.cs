using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightTrace.Domain.GeomertryPrimitives;
using Microsoft.Xna.Framework;

namespace LightTrace.Domain.OctTreeOptimisation
{
    public class OctTree
    {
        public Node Root { get; set; }

        private int _minCount = int.MaxValue;
        private int _maxCount = int.MinValue;
        private int _totalCount;
        private int _avrCount;
        private int _leafCount;

        public OctTree(Node root)
        {
            Root = root;
        }

        public IList<Geomertry> GetPotencialObjects(Ray ray)
        {
            List<Geomertry> result = new List<Geomertry>();

            GetObjects(ray, Root, result);

            return result;
        }

        private void GetObjects(Ray ray, Node node, List<Geomertry> result)
        {
            if (!ray.Intersects(node.Box).HasValue)
                return;

            if (node.Nodes.Any())
            {
                foreach (var subNode in node.Nodes)
                {
                    GetObjects(ray, subNode, result);
                }
            }
            else
            {
                result.AddRange(node.Objects);
            }
        }

        public string GetStatistics()
        {
            CalculateStatistic(Root);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Min count in leaf = {0}", _minCount));
            builder.AppendLine(string.Format("Max count in leaf = {0}", _maxCount));
            builder.AppendLine(string.Format("Avr count in leaf = {0}", _avrCount));
            builder.AppendLine(string.Format("Total count in leaf = {0}", _totalCount));
            builder.AppendLine(string.Format("Leaf count = {0}", _leafCount));

            return builder.ToString();
        }

        public void CalculateStatistic(Node node)
        {
            foreach (var subNode in node.Nodes)
            {
                if (subNode.Nodes.Any())
                {
                    CalculateStatistic(subNode);
                }
                else
                {
                    _minCount = Math.Min(_minCount, subNode.Objects.Count);
                    _maxCount = Math.Max(_maxCount, subNode.Objects.Count);
                    _totalCount += subNode.Objects.Count;
                    _leafCount++;
                }
            }
            if (_leafCount != 0)
            {
                _avrCount = _totalCount/_leafCount;
            }
        }
    }
}