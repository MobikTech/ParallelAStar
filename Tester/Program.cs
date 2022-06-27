using System;
using SequentialAStar.Common;
using SequentialAStar.Common.Generators;

namespace Tester
{
    static class Program
    {
        private const int MatrixSize = 600;
        
        static void Main(string[] args)
        {
            IMatrixGenerator generator = new RandomMatrixGenerator();
            // IMatrixGenerator generator = new ManualMatrixGenerator();
            Matrix matrix = generator.Generate(0.9f, (MatrixSize, MatrixSize));
            IPathFinder pathFinder = new SequentialAStar.SequentialAStar.SequentialAStar();
            PathVisualizer pathVisualizer = new PathVisualizer();

            TimeTester timeTester = new TimeTester();
            TimeSpan sequentialAStarTime = timeTester.TestSync(pathFinder.GetShortestPath, matrix, out Path path);
            
            // pathVisualizer.Visualize(matrix, path);
            Console.WriteLine($"Time of Sequential A* - {sequentialAStarTime.Milliseconds} ms");
            Console.WriteLine($"Time of Parallel A* - _ ms");
        }
    }
}