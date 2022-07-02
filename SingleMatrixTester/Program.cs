using System;
using System.Collections.Generic;
using AStar.Common;
using AStar.Common.Generators;
using AStar.ParallelAStar;
using AStar.SequentialAStar;
using TesterAdditionalLibrary;

namespace Tester
{
    static class Program
    {
        private const int MatrixSize = 30;
        
        static void Main()
        {
            IMatrixGenerator generator = new RandomMatrixGenerator();
            // IMatrixGenerator generator = new ManualMatrixGenerator();
            
            IPathFinder sequentialAStar = new SequentialAStar();
            IPathFinder parallelAStar = new ParallelAStar();
            
            Matrix matrix = generator.Generate(0.9f, (MatrixSize, MatrixSize));
            
            PathVisualizer pathVisualizer = new PathVisualizer();
            TimeWatcher timeTester = new TimeWatcher();

            matrix.ResetAlgorithmInfo();
            long sequentialAStarTime = timeTester.TestSync(sequentialAStar.GetShortestPath, matrix, out Path seqAStarPath);
            matrix.ResetAlgorithmInfo();
            long parallelAStarTime = timeTester.TestSync(parallelAStar.GetShortestPath, matrix, out Path parAStarPath);

            Console.WriteLine($"Time of Sequential A* - {sequentialAStarTime} ms");
            Console.WriteLine($"Time of Parallel A* - {parallelAStarTime} ms");
            
            
            Console.WriteLine("Sequential A* searched path visualization:");
            pathVisualizer.Visualize(matrix, seqAStarPath);
            
            Console.WriteLine("Parallel A* searched path visualization:");
            pathVisualizer.Visualize(matrix, parAStarPath);

        }

        private static void PrintList<T>(IEnumerable<T> list)
        {
            foreach (var el in list)
            {
                Console.Write(el + " ");
            }

            Console.WriteLine();
        }
        
        private static void PrintNodes(IEnumerable<Node> list)
        {
            foreach (var el in list)
            {
                Console.Write($"{el}-[{el.ParallelInfo.Parent?.Position.X}, {el.ParallelInfo.Parent?.Position.Y}] ");
            }

            Console.WriteLine();
        }
        
    }
}