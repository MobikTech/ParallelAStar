using System;
using System.Collections.Generic;
using SequentialAStar.Common;
using SequentialAStar.Common.Generators;
using TesterAdditional;

namespace Tester
{
    static class Program
    {
        private const int MatrixSize = 20;
        
        static void Main()
        {
            IMatrixGenerator generator = new RandomMatrixGenerator();
            // IMatrixGenerator generator = new ManualMatrixGenerator();
            
            IPathFinder sequentialAStar = new SequentialAStar.SequentialAStar.SequentialAStar();
            IPathFinder parallelAStar = null;
            
            Matrix matrix = generator.Generate(0.9f, (MatrixSize, MatrixSize));
            
            PathVisualizer pathVisualizer = new PathVisualizer();
            TimeWatcher timeTester = new TimeWatcher();
            
            
            long sequentialAStarTime = timeTester.TestSync(sequentialAStar.GetShortestPath, matrix, out Path seqAStarPath);
            long parallelAStarTime = -1;

            Console.WriteLine($"Time of Sequential A* - {sequentialAStarTime} ms");
            Console.WriteLine($"Time of Parallel A* - {parallelAStarTime} ms");
            Console.WriteLine("Sequential A* searched path visualization:");
            pathVisualizer.Visualize(matrix, seqAStarPath);
        }
        
    }
}