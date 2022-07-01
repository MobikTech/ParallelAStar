using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SequentialAStar.Common;

namespace SequentialAStar.ParallelAStar
{
    public class ParallelAStar : IPathFinder
    {
        private Matrix _matrix;
        private Task _forward;
        private Task _backward;

        // private bool _founded;
        private Path? _foundedPath;

        private void Init(Matrix matrix)
        {
            _matrix = matrix;
            matrix.ResetAlgorithmInfo();
        }

        public Path GetShortestPath(Matrix matrix)
        {
            Init(matrix);

            ThreadHandler forwardHandler = new ThreadHandler(_matrix, _matrix.StartNode, _matrix.GoalNode, ProcessType.Forward);
            ThreadHandler backwardHandler = new ThreadHandler(_matrix, _matrix.GoalNode, _matrix.StartNode, ProcessType.Backward);

            Path forwardPath = null;
            Path backwardPath = null;

            forwardHandler.HalfPathFound += path => forwardPath = path;
            backwardHandler.HalfPathFound += path => backwardPath = path;

            // Thread forward = new Thread(forwardHandler.GetShortestPath);
            // Thread backward = new Thread(backwardHandler.GetShortestPath);
            // forward.Start();
            // backward.Start();
            // forward.Join();
            // backward.Join();
            _forward = Task.Run(forwardHandler.GetShortestPath);
            _backward = Task.Run(backwardHandler.GetShortestPath);
            Task.WaitAll(_forward, _backward);

            return GetFullPath(forwardPath, backwardPath);
        }

        private Path GetFullPath(Path forwardPath, Path backwardPath)
        {
            if (!forwardPath.IsPathExisting || !backwardPath.IsPathExisting)
                return new Path(null, false);

            IEnumerable<Node> secondPart = backwardPath.Nodes
                .Take(backwardPath.Nodes.Count() - 1)
                .Reverse();

           IEnumerable<Node> firstPart = forwardPath.Nodes;

           return new Path(firstPart.Concat(secondPart), true);
        }
    }
}