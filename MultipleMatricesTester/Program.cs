using System;
using System.Collections.Generic;
using System.Linq;
using AStar.Common;
using AStar.Common.Generators;
using AStar.ParallelAStar;
using AStar.SequentialAStar;
using ConsoleTables;
using TesterAdditionalLibrary;

namespace MultipleMatricesTester
{
    public class Program
    {
        private const int StartMatrixSize = 200;
        private const int MatrixSizeStep = 50;
        private const int Times = 5;
        
        static void Main(string[] args)
        {
            IPathFinder sequentialAStar = new SequentialAStar();
            IPathFinder parallelAStar = new ParallelAStar();
            
            IEnumerable<ComparisonResult> results = TestMultiple(sequentialAStar, parallelAStar);
            
            VisualizeResults(results);
        }

        public struct AlgorithmResult
        {
            public readonly Path Path;
            public readonly long Time;

            public AlgorithmResult(Path path, long time)
            {
                Path = path;
                Time = time;
            }
        }
        public struct ComparisonResult
        {
            public readonly int MatrixSize;
            public readonly AlgorithmResult SequentialResult;
            public readonly AlgorithmResult ParallelResult;
            public readonly float SpeedupFactor;

            public ComparisonResult(int matrixSize, AlgorithmResult sequentialResult, AlgorithmResult parallelResult, float speedupFactor)
            {
                SequentialResult = sequentialResult;
                ParallelResult = parallelResult;
                SpeedupFactor = speedupFactor;
                MatrixSize = matrixSize;
            }
        }

        private static IEnumerable<ComparisonResult> TestMultiple(IPathFinder sequentialAStar, IPathFinder parallelAStar)
        {
            int size = StartMatrixSize;
            TimeWatcher timeTester = new TimeWatcher();
            IMatrixGenerator generator = new RandomMatrixGenerator();
            List<ComparisonResult> results = new List<ComparisonResult>();

            for (int i = 0; i < Times; i++)
            {
                Matrix matrix = generator.Generate(0.9f, (size, size));
                Console.Write("generated, ");

                matrix.ResetAlgorithmInfo();
                long sequentialTime = timeTester.TestSync(sequentialAStar.GetShortestPath, matrix, out Path pathSequential);
                matrix.ResetAlgorithmInfo();
                long parallelTime = timeTester.TestSync(parallelAStar.GetShortestPath, matrix, out Path pathParallel);
                Console.WriteLine("tested " + i);
                
                AlgorithmResult sequentialAlgorithmResult = new AlgorithmResult(pathSequential, sequentialTime);
                AlgorithmResult parallelAlgorithmResult = new AlgorithmResult(pathParallel, parallelTime);

                float speedupFactor = (float) (sequentialTime == 0 ? 0.1f : sequentialTime) / 
                                      (parallelTime == 0 ? 0.1f : parallelTime);

                results.Add(new ComparisonResult(size, sequentialAlgorithmResult, parallelAlgorithmResult, speedupFactor));
                size += MatrixSizeStep;
            }

            return results;
        }

        private static void VisualizeResults(IEnumerable<ComparisonResult> results)
        {
            var table = new ConsoleTable("Matrix size", "Sequential time", "Sequential path length", "Sequential path exists", 
                "Parallel time", "Parallel path length", "Parallel path exists", "Speedup factor");

            foreach (var result in results)
            {
                table.AddRow($"{result.MatrixSize}х{result.MatrixSize}",
                    result.SequentialResult.Time + "ms",
                    result.SequentialResult.Path.Nodes?.Count,
                    result.SequentialResult.Path.IsPathExisting,
                    result.ParallelResult.Time + "ms",
                    result.ParallelResult.Path.Nodes?.Count,
                    result.ParallelResult.Path.IsPathExisting,
                    result.SpeedupFactor);
            }
            
            table.Write();

            Console.WriteLine($"Average speedup acceleration - {results.Select(result => result.SpeedupFactor).Average()}");
        }
    }
}
