// using System;
// using System.Collections.Generic;
// using System.Linq;
// using SequentialAStar.Common;
//
// namespace SequentialAStar.SequentialAStar
// {
//     public class SequentialAStar2 : IPathFinder
//     {
//         private List<Node> _openList;
//         private List<Node> _closedList;
//         private Matrix _matrix;
//         
//         private void Init(Matrix matrix)
//         {
//             _matrix = matrix;
//             _openList = new List<Node>(){ _matrix.StartNode };
//             _openList[0].CountToFirst = 0;
//             _closedList = new List<Node>();
//         }
//
//         public Path GetShortestPath(Matrix matrix)
//         {
//             Init(matrix);
//             
//             while (_openList.Count > 0)
//             {
//                 Node currentNode = _openList.OrderBy(F).First();
//
//                 if (IsFinal(currentNode))
//                     return GetPathTo(currentNode);
//
//                 _openList.Remove(currentNode);
//                 _closedList.Add(currentNode);
//
//                 HandleNeighbours(currentNode);
//             }
//
//             return new Path(Array.Empty<Node>(), false);
//         }
//
//         private void HandleNeighbours(Node parent)
//         {
//              // foreach (Node neighbour in parent.Neighbors)
//             // {
//             //     Console.WriteLine(_openList.Count);
//             //     // if (_closedList.Contains(neighbour))
//             //     //     continue;
//             //     
//             //     int cost = G(parent) + 1;
//             //     
//             //     if (_openList.Contains(neighbour) && cost < G(neighbour)) 
//             //         _openList.Remove(neighbour);
//             //     
//             //     if (_closedList.Contains(neighbour) && cost < G(neighbour)) 
//             //         _closedList.Remove(neighbour);
//             //     
//             //     if (_openList.Contains(neighbour) || _closedList.Contains(neighbour)) 
//             //         continue;
//             //     
//             //     // neighbour.PathToThis = parent.PathToThis.Append(neighbour);
//             //     neighbour.Parent = parent;
//             //     neighbour.CountToFirst = parent.CountToFirst + 1;
//             //     _openList.Add(neighbour);
//             // }
//             
//             // foreach (Node neighbour in parent.Neighbors)
//             // {
//             //     Console.WriteLine(_openList.Count);
//             //     int cost = G(parent) + 1;
//             //     
//             //     bool inOpenList = _openList.Contains(neighbour);
//             //     bool inClosedList = _closedList.Contains(neighbour);
//             //
//             //     if (inOpenList && cost < G(neighbour))
//             //     {
//             //         _openList.Remove(neighbour);
//             //         inOpenList = false;
//             //     }
//             //
//             //     if (inClosedList && cost < G(neighbour))
//             //     {
//             //         _closedList.Remove(neighbour);
//             //         inClosedList = false;
//             //     }
//             //     
//             //     if (inOpenList || inClosedList) 
//             //         continue;
//             //     
//             //     neighbour.Parent = parent;
//             //     neighbour.CountToFirst = parent.CountToFirst + 1;
//             //     _openList.Add(neighbour);
//             // }
//             
//             // foreach (Node neighbour in parent.Neighbors)
//             // {
//             //     if (_closedList.Contains(neighbour))
//             //         continue;
//             //
//             //     if (!_openList.Contains(neighbour))
//             //     {
//             //         _openList.Add(neighbour);
//             //         neighbour.Parent = parent;
//             //         neighbour.CountToFirst = parent.CountToFirst + 1;
//             //     }
//             //     else
//             //     {
//             //         int cost = G(parent) + 1;
//             //         if (cost < G(neighbour))
//             //         {
//             //             neighbour.Parent = parent;
//             //             neighbour.CountToFirst = parent.CountToFirst + 1;
//             //         }
//             //     }
//             // }
//             
//             foreach (Node neighbour in parent.Neighbors)
//             {
//                 int cost = G(parent) + 1;
//                 
//                 bool inOpenList = _openList.Contains(neighbour);
//                 bool inClosedList = _closedList.Contains(neighbour);
//                 
//                 if (inOpenList && cost > G(neighbour)) 
//                     continue;
//                 
//                 if (inClosedList && cost > G(neighbour)) 
//                     continue;
//                 
//                 if (inOpenList) _openList.Remove(neighbour);
//                 if (inClosedList) _closedList.Remove(neighbour);
//                 
//                 neighbour.Parent = parent;
//                 neighbour.CountToFirst = parent.CountToFirst + 1;
//                 
//                 _openList.Add(neighbour);
//             }
//         }
//
//         private bool IsFinal(Node currentNode) => currentNode == _matrix.GoalNode;
//         private int F(Node node) => G(node) + H(node);
//         private int G(Node node) => node.CountToFirst.Value;
//         private int H(Node node) => DistanceBetweenNodes(node, _matrix.GoalNode);
//         private int DistanceBetweenNodes(Node node1, Node node2) => Math.Abs(node1.Position.X - node2.Position.X) +
//                                                                 Math.Abs(node1.Position.Y - node2.Position.Y);
//
//         private Path GetPathTo(Node goal)
//         {
//             List<Node> path = new List<Node>();
//             Node? currentNode = goal;
//             while (currentNode != null)
//             {
//                 path.Add(currentNode);
//                 currentNode = currentNode.Parent;
//             }
//
//             path.Reverse();
//             return new Path(path, true);
//         }
//     }
// }