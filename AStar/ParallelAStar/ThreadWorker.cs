using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AStar.Common;

namespace AStar.ParallelAStar
{
    public enum ProcessType
    {
        Forward,
        Backward
    }
    
    public class ThreadWorker
    {
        private readonly List<Node> _openList;
        private readonly List<Node> _closedList;
        private readonly Matrix _matrix;
        private readonly Node _startNode;
        private readonly Node _goalNode;
        private readonly ProcessType _processType;
        private readonly CancellationTokenSource _oppositeSearcherCancellationToken;
        private readonly CancellationToken _token;

        public Action<Path>? HalfPathFound;

        public ThreadWorker(Matrix matrix, Node startNode, Node goalNode, ProcessType processType, 
            CancellationTokenSource oppositeSearcherCancellationToken, CancellationToken token)
        {
            _matrix = matrix;
            _startNode = startNode;
            _goalNode = goalNode;
            _processType = processType;
            _oppositeSearcherCancellationToken = oppositeSearcherCancellationToken;
            _token = token;

            _openList = new List<Node>(){ _startNode };
            _startNode.ParallelInfo.CountToFirst = 0;
            _closedList = new List<Node>();
        }

        public void GetShortestPath()
        {
            while (_openList.Count > 0)
            {
                if (_token.IsCancellationRequested || _oppositeSearcherCancellationToken.IsCancellationRequested)
                    return;

                Node currentNode = _openList.OrderBy(F).First();

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                HandleNeighbours(currentNode);
            }

            Console.WriteLine($"{_processType}: PATH NOT FOUND");
            HalfPathFound?.Invoke(new Path(null, false));
        }
    
        private void HandleNeighbours(Node parent)
        {
            foreach (Node neighbour in parent.Neighbors)
            {
                if (_closedList.Contains(neighbour))
                    continue;

                if (MetAnotherProcess(neighbour))
                {
                    _oppositeSearcherCancellationToken.Cancel();
            
                    Path thisPath = GetPathTo(parent);
                    thisPath.Nodes.Add(neighbour);

                    Path anotherPath = GetPathTo(neighbour);

                    Path fullPath = GetFullPath(thisPath, anotherPath);
                    HalfPathFound?.Invoke(fullPath);
                    // Console.WriteLine($"{_processType}: PATH FOUND from {fullPath.Nodes.First().Position} to {fullPath.Nodes.Last().Position}");
                    return;
                }
                
                if (_token.IsCancellationRequested)
                    return;

                if (!_openList.Contains(neighbour))
                {
                    AddToOpenList(neighbour);
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

        
        private int F(Node node) => G(node) + H(node);
        private int G(Node node) => node.ParallelInfo.CountToFirst.Value;
        private int H(Node node) => DistanceBetweenNodes(node, _goalNode);
        private int DistanceBetweenNodes(Node node1, Node node2) => Math.Abs(node1.Position.X - node2.Position.X) +
                                                                    Math.Abs(node1.Position.Y - node2.Position.Y);
        private bool MetAnotherProcess(Node node)
        {
            return _processType switch
            {
                ProcessType.Forward => node.ParallelInfo.InBackwardOpenList,
                ProcessType.Backward => node.ParallelInfo.InForwardOpenList,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        private void AddToOpenList(Node currentNode)
        {
            _openList.Add(currentNode);
            switch (_processType)
            {
                case ProcessType.Forward:
                    currentNode.ParallelInfo.InForwardOpenList = true;
                    break;
                case ProcessType.Backward:
                    currentNode.ParallelInfo.InBackwardOpenList = true;
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
        private Path GetFullPath(Path thisPath, Path anotherPath)
        {
            return _processType switch
            {
                ProcessType.Forward => JoinPaths(thisPath, anotherPath),
                ProcessType.Backward => JoinPaths(anotherPath, thisPath),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        private Path JoinPaths(Path forwardPath, Path backwardPath)
        {
            if (!forwardPath.IsPathExisting || !backwardPath.IsPathExisting)
                return new Path(null, false);

            IEnumerable<Node> secondPart = backwardPath.Nodes
                .Take(backwardPath.Nodes.Count() - 1)
                .Reverse();

            IEnumerable<Node> firstPart = forwardPath.Nodes;

            return new Path(firstPart.Concat(secondPart).ToList(), true);
        }

    }
}