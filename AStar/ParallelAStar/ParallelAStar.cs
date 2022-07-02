using System.Threading;
using System.Threading.Tasks;
using AStar.Common;

namespace AStar.ParallelAStar
{
    public class ParallelAStar : IPathFinder
    {
        public Path GetShortestPath(Matrix matrix)
        {
            CancellationTokenSource forwardSource = new CancellationTokenSource();
            CancellationTokenSource backwardSource = new CancellationTokenSource();

            
            ThreadWorker forwardWorker = new ThreadWorker(matrix.StartNode, matrix.GoalNode, ProcessType.Forward, backwardSource, forwardSource.Token);
            ThreadWorker backwardWorker = new ThreadWorker(matrix.GoalNode, matrix.StartNode, ProcessType.Backward, forwardSource, backwardSource.Token);

            Path fullPath = null;

            forwardWorker.PathFound += path => fullPath = path;
            backwardWorker.PathFound += path => fullPath = path;

            // Thread forward = new Thread(forwardWorker.GetShortestPath);
            // Thread backward = new Thread(backwardWorker.GetShortestPath);
            // forward.Start();
            // backward.Start();
            // forward.Join();
            // backward.Join();
            
            Task forwardTask = Task.Run(forwardWorker.GetShortestPath, forwardSource.Token);
            Task backwardTask = Task.Run(backwardWorker.GetShortestPath, backwardSource.Token);
            
            Task.WaitAll(forwardTask, backwardTask);
            
            return fullPath;
        }
    }
}
