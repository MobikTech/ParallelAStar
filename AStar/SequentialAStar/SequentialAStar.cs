using System;
using System.Collections.Generic;
using System.Linq;
using SequentialAStar.Common;

namespace SequentialAStar.SequentialAStar
{
    public class SequentialAStar : IPathFinder
    {
        private List<Node> _openList;
        private List<Node> _closedList;
        private Matrix _matrix;
        
        private void Init(Matrix matrix)
        {
            _matrix = matrix;
            _openList = new List<Node>(){ _matrix.StartNode };
            _matrix.StartNode.PathToThis = new List<Node>(){_matrix.StartNode};
            _closedList = new List<Node>();
        }

        public void GetShortestPath(Matrix matrix, Action<Matrix, Path> searchFinished)
        {
            Init(matrix);
            
            while (_openList.Count > 0)
            {
                Node currentNode = _openList.OrderBy(F).First();
                
                if (IsFinal(currentNode, matrix.GoalNode))
                {
                    searchFinished?.Invoke(matrix, new Path(currentNode.PathToThis, true)); 
                    return;
                }

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                HandleNeighbours(currentNode);
            }
            
            searchFinished?.Invoke(matrix, new Path(Array.Empty<Node>(), false));
        }

        public Path GetShortestPath(Matrix matrix)
        {
            Init(matrix);
            
            while (_openList.Count > 0)
            {
                Node currentNode = _openList.OrderBy(F).First();
                
                if (IsFinal(currentNode, matrix.GoalNode))
                    return new Path(currentNode.PathToThis, true);

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                HandleNeighbours(currentNode);
            }

            return new Path(Array.Empty<Node>(), false);
        }

        private void HandleNeighbours(Node parent)
        {
            foreach (Node neighbour in parent.Neighbors)
            {
                int cost = G(parent) + 1;

                if (_openList.Contains(neighbour) && cost < G(neighbour)) 
                    _openList.Remove(neighbour);
                
                if (_closedList.Contains(neighbour) && cost < G(neighbour)) 
                    _closedList.Remove(neighbour);

                if (_openList.Contains(neighbour) || _closedList.Contains(neighbour)) 
                    continue;
                
                neighbour.PathToThis = parent.PathToThis.Append(neighbour);
                _openList.Add(neighbour);
            }
        }

        private static bool IsFinal(Node currentNode, Node goal) => currentNode == goal;
        private int F(Node node) => G(node) + H(node);
        private int G(Node node) => node.PathToThis.Count();
        private int H(Node node) => DistanceBetweenNodes(node, _matrix.GoalNode);
        private int DistanceBetweenNodes(Node node1, Node node2) => Math.Abs(node1.Position.X - node2.Position.X) +
                                                                Math.Abs(node1.Position.Y - node2.Position.Y);
    }
}