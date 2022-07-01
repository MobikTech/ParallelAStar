// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Collections.Generic;
// using SequentialAStar.Common;
//
// namespace SequentialAStar.ParallelAStar
// {
//     public class ParallelAStar2 : IPathFinder
//     {
//         private BinaryHeap<int, Node> _closedList;
//         private BinaryHeap<int, Node> _openList;
//         private Matrix _matrix;
//         
//         private void Init(Matrix matrix)
//         {
//             _matrix = matrix;
//             _matrix.StartNode.CountToFirst = 0;
//             _closedList = new BinaryHeap<int, Node>();
//             _openList = new BinaryHeap<int, Node>();
//             _openList.Add(F(_matrix.StartNode), _matrix.StartNode);
//         }
//
//         public Path GetShortestPath(Matrix matrix)
//         {
//             Init(matrix);
//             
//             while (_openList.Count > 0)
//             {
//                 // Node currentNode = _openList.OrderBy(F).First();
//                 Node currentNode = _openList.Remove().Value;
//                 currentNode.InOpenList = false;
//                 _closedList.Add(F(currentNode), currentNode);
//                 currentNode.InClosedList = true;
//
//                 if (IsFinal(currentNode))
//                     return GetPathTo(currentNode);
//
//
//                 HandleNeighbours(currentNode);
//             }
//
//             return new Path(Array.Empty<Node>(), false);
//         }
//
//         private void HandleNeighbours(Node parent)
//         {
//             Parallel.ForEach(parent.Neighbors, neighbour =>
//             {
//                 if (neighbour.InClosedList)
//                     return;
//
//                 if (!neighbour.InOpenList)
//                 {
//                     _openList.Add(F(neighbour), neighbour);
//                     neighbour.InOpenList = true;
//                     
//                     neighbour.Parent = parent;
//                     neighbour.CountToFirst = parent.CountToFirst + 1;
//                 }
//                 else
//                 {
//                     int cost = G(parent) + 1;
//                     _openList.
//                     if (cost < G(neighbour))
//                     {
//                         neighbour.Parent = parent;
//                         neighbour.CountToFirst = parent.CountToFirst + 1;
//                     }
//                 }
//             });
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