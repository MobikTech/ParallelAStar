using System;
using System.Collections.Generic;
using System.Linq;

namespace SequentialAStar.Common.Generators
{
    public class RandomMatrixGenerator : IMatrixGenerator
    {
        private readonly Random _rnd;
        
        public RandomMatrixGenerator()
        {
            _rnd = new Random();
        }

        private static readonly (int x, int y)[] s_neighborOffsets = new (int, int)[]
        {
            (-1, 0),
            (0, 1),
            (1, 0),
            (0, -1)
        };
        
        public Matrix Generate(float passablePercent, (int, int) size)
        {
            Node[,] matrix = GenerateMatrix(passablePercent, size);
            SetNeighbors(ref matrix, out IEnumerable<Node> passableNodes);
            var startNode = GetRandomNode(passableNodes);
            var goalNode = GetRandomNode(passableNodes.
                Where(node => !node.Position.Equals(startNode.Position)));
            return new Matrix(size, startNode, goalNode, matrix);
        }

        private Node[,] GenerateMatrix(float passablePercent, (int x, int y) size)
        {
            var matrix = new Node[size.x, size.y];

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    matrix[i, j] = new Node(new Position(i, j), GenerateByChance(passablePercent));
                }
            }

            return matrix;
        }

        private Passing GenerateByChance(float passingProbability)
        {
            bool passing = _rnd.NextDouble() <= passingProbability;
            return passing switch
            {
                true => Passing.Passable,
                false => Passing.Unpassable
            };
        }

        private void SetNeighbors(ref Node[,] nodes, out IEnumerable<Node> passableNodes)
        {
            passableNodes = new List<Node>();
            foreach (var node in nodes)
            {
                if (node.Passable == Passing.Unpassable)
                    continue;

                passableNodes = passableNodes.Append(node);
                foreach (var offset in s_neighborOffsets)
                {
                    if (TryGetNode(nodes, node.Position.GetOffseted(offset.x, offset.y), out Node neigbour) && 
                        neigbour.Passable == Passing.Passable)
                    {
                        node.Neighbors = node.Neighbors.Append(neigbour);
                    }
                }
            }
        }

        private bool TryGetNode(Node[,] nodes, Position pos, out Node node)
        {
            node = null;
            if (pos.X >= nodes.GetLength(0) || pos.X < 0 || 
                pos.Y >= nodes.GetLength(1) || pos.Y < 0) 
                return false;
            
            node = nodes[pos.X, pos.Y];
            return true;
        }

        private Node GetRandomNode(IEnumerable<Node> nodes) => 
            nodes.ElementAt(_rnd.Next(nodes.Count()));
    }
}