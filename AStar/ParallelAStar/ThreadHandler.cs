using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SequentialAStar.Common;

namespace SequentialAStar.ParallelAStar
{
    public enum ProcessType
    {
        Forward,
        Backward
    }
    public class ThreadHandler
    {
        private readonly List<Node> _openList;
        private readonly List<Node> _closedList;
        private readonly Matrix _matrix;
        private readonly Node _startNode;
        private readonly Node _goalNode;
        private readonly ProcessType _processType;
        
        public Action<Path>? HalfPathFound;
        private Path? _halfPath;
        private static bool s_founded;
        private static Node _metNode;

        public ThreadHandler(Matrix matrix, Node startNode, Node goalNode, ProcessType processType)
        {
            _matrix = matrix;
            _startNode = startNode;
            _goalNode = goalNode;
            _processType = processType;

            _openList = new List<Node>(){ _startNode };
            _startNode.ParallelInfo.CountToFirst = 0;
            _closedList = new List<Node>();
            _metNode = null;
        }
        
        public void GetShortestPath()
        {
            s_founded = false;
            while (_openList.Count > 0)
            {
                Console.WriteLine($"{_processType} {_closedList.Count}");
                if (s_founded)
                {
                    _halfPath ??= GetPathTo(_metNode);
                    HalfPathFound?.Invoke(_halfPath);
                    Console.WriteLine($"PATH FOUND ON {_processType} {_halfPath.Nodes.Last().Position}");
                    return;
                }

                Node currentNode = _openList.OrderBy(F).First();

                // if (IsFinal(currentNode))
                // {
                //     s_founded = true;
                //     HalfPathFound?.Invoke(GetPathTo(currentNode));
                //     return;
                // }

                _openList.Remove(currentNode);
                AddToClosedList(currentNode);

                HandleNeighbours(currentNode);
            }

            HalfPathFound?.Invoke(new Path(null, false));
        }
        
        private void HandleNeighbours(Node parent)
        {
            foreach (Node neighbour in parent.Neighbors)
            {
                if (MetAnotherProcess(neighbour))
                {
                    Console.WriteLine($"Met on {_processType} {neighbour.Position}");
                    lock (neighbour)
                    {
                        Node oldParent = neighbour.ParallelInfo.Parent;
                        neighbour.ParallelInfo.Parent = parent;
                        Path path = GetPathTo(neighbour);
                        neighbour.ParallelInfo.Parent = oldParent;

                        _halfPath = path;
                        _metNode = neighbour;
                        s_founded = true;
                    }
                    return;
                }
                
                if (_closedList.Contains(neighbour))
                    continue;

                if (!_openList.Contains(neighbour))
                {
                    _openList.Add(neighbour);
                    neighbour.ParallelInfo.Parent = parent;
                    neighbour.ParallelInfo.CountToFirst = parent.ParallelInfo.CountToFirst + 1;
                }
                else
                {
                    int cost = G(parent) + 1;
                    if (cost < G(neighbour))
                    {
                        neighbour.ParallelInfo.Parent = parent;
                        neighbour.ParallelInfo.CountToFirst = parent.ParallelInfo.CountToFirst + 1;
                    }
                }
            }
        }

        
        
        private bool IsFinal(Node currentNode) => currentNode == _goalNode;
        private int F(Node node) => G(node) + H(node);
        private int G(Node node) => node.ParallelInfo.CountToFirst.Value;
        private int H(Node node) => DistanceBetweenNodes(node, _matrix.GoalNode);
        private int DistanceBetweenNodes(Node node1, Node node2) => Math.Abs(node1.Position.X - node2.Position.X) +
                                                                    Math.Abs(node1.Position.Y - node2.Position.Y);

        private bool MetAnotherProcess(Node node)
        {
            return _processType switch
            {
                ProcessType.Forward => node.ParallelInfo.InBackwardClosedList,
                ProcessType.Backward => node.ParallelInfo.InForwardClosedList,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void AddToClosedList(Node currentNode)
        {
            _closedList.Add(currentNode);
            switch (_processType)
            {
                case ProcessType.Forward:
                    currentNode.ParallelInfo.InForwardClosedList = true;
                    break;
                case ProcessType.Backward:
                    currentNode.ParallelInfo.InBackwardClosedList = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private Path GetPathTo(Node goal)
        {
            List<Node> path = new List<Node>();
            Node? currentNode = goal;
            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.ParallelInfo.Parent;
            }

            path.Reverse();
            return new Path(path, true);
        }

    }
}