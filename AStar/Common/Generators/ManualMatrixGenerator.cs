using System.Collections.Generic;
using System.Linq;

namespace SequentialAStar.Common.Generators
{
    public class ManualMatrixGenerator : IMatrixGenerator
    {
         private static readonly (int x, int y)[] s_neighborOffsets = new (int, int)[]
        {
            (-1, 0),
            (0, 1),
            (1, 0),
            (0, -1)
        };

         private static readonly (int x, int y) s_startPoint = (3, 3);
         private static readonly (int x, int y) s_goalPoint = (3, 9);

         private static readonly (int x, int y)[] s_walls = new (int, int)[]
         {
            (3, 5),
            (4, 5),
            (5, 5),
            (5, 4),
            (5, 3),
            
            (2, 7),
            (3, 7),
            (4, 7),
         };
        
        public Matrix Generate(float passablePercent, (int, int) size)
        {
            size = (20, 20);
            Node[,] matrix = GenerateMatrix(size);
            SetNeighbors(ref matrix, out IEnumerable<Node> passableNodes);
            var startNode = matrix[s_startPoint.x, s_startPoint.y];
            var goalNode = matrix[s_goalPoint.x, s_goalPoint.y];
            return new Matrix(size, startNode, goalNode, matrix);
        }

        private Node[,] GenerateMatrix((int x, int y) size)
        {
            var matrix = new Node[size.x, size.y];

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    if (s_walls.Contains((i, j)))
                        matrix[i, j] = new Node(new Position(i, j), Passing.Unpassable);
                    else
                        matrix[i, j] = new Node(new Position(i, j), Passing.Passable);
                }
            }

            return matrix;
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
    }
}