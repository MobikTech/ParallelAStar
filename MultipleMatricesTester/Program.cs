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
        private const int StartMatrixSize = 100;
        private const int MatrixSizeStep = 50;
        private const int Times = 10;
        private const int RepeatTimes = 5;
        private const bool OnlyParallel = false;

        static void Main(string[] args)
        {
            IPathFinder sequentialAStar = new SequentialAStar();
            IPathFinder parallelAStar = new ParallelAStar();

            List<AverageResult> results = OnlyParallel 
                ? TestMultipleOnlyParallel(parallelAStar) 
                : TestMultiple(sequentialAStar, parallelAStar);

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

            public ComparisonResult(int matrixSize, AlgorithmResult sequentialResult, AlgorithmResult parallelResult,
                float speedupFactor)
            {
                SequentialResult = sequentialResult;
                ParallelResult = parallelResult;
                SpeedupFactor = speedupFactor;
                MatrixSize = matrixSize;
            }
        }

        public struct AverageResult
        {
            public readonly int IterationsCount;
            public readonly int MatrixSize;
            public readonly float SpeedupFactor;

            public readonly float SequentialTime;
            public readonly int SequentialPathLength;
            public readonly int SequentialFoundedCount;


            public readonly float ParallelTime;
            public readonly int ParallelPathLength;
            public readonly int ParallelFoundedCount;


            public AverageResult(int iterationsCount, int matrixSize, float speedupFactor, float sequentialTime,
                int sequentialPathLength, int sequentialFoundedCount, float parallelTime, int parallelPathLength,
                int parallelFoundedCount)
            {
                IterationsCount = iterationsCount;
                MatrixSize = matrixSize;
                SpeedupFactor = speedupFactor;
                SequentialTime = sequentialTime;
                SequentialPathLength = sequentialPathLength;
                SequentialFoundedCount = sequentialFoundedCount;
                ParallelTime = parallelTime;
                ParallelPathLength = parallelPathLength;
                ParallelFoundedCount = parallelFoundedCount;
            }
        }

        private static List<AverageResult> TestMultiple(IPathFinder sequentialAStar, IPathFinder parallelAStar)
        {
            int size = StartMatrixSize;
            TimeWatcher timeTester = new TimeWatcher();
            IMatrixGenerator generator = new RandomMatrixGenerator();
            List<AverageResult> results = new List<AverageResult>();

            for (int i = 0; i < Times; i++)
            {
                List<ComparisonResult> oneSizeResults = new List<ComparisonResult>();
                for (int t = 0; t < RepeatTimes; t++)
                {
                    Matrix matrix = generator.Generate(0.9f, (size, size));
                    Console.Write("generated, ");
                    
                    matrix.ResetAlgorithmInfo();
                    long sequentialTime =
                        timeTester.TestSync(sequentialAStar.GetShortestPath, matrix, out Path pathSequential);
                    matrix.ResetAlgorithmInfo();
                    long parallelTime =
                        timeTester.TestSync(parallelAStar.GetShortestPath, matrix, out Path pathParallel);
                    Console.WriteLine("tested " + i);

                    AlgorithmResult sequentialAlgorithmResult = new AlgorithmResult(pathSequential, sequentialTime);
                    AlgorithmResult parallelAlgorithmResult = new AlgorithmResult(pathParallel, parallelTime);

                    float speedupFactor = GetSpeedup(sequentialTime, parallelTime);


                    oneSizeResults.Add(new ComparisonResult(size, sequentialAlgorithmResult, parallelAlgorithmResult,
                        speedupFactor));
                }

                var averageResult = GetAverageResult(oneSizeResults);
                results.Add(averageResult);
                size += MatrixSizeStep;
            }

            return results;
        }
        private static List<AverageResult> TestMultipleOnlyParallel(IPathFinder parallelAStar)
        {
            int size = StartMatrixSize;
            TimeWatcher timeTester = new TimeWatcher();
            IMatrixGenerator generator = new RandomMatrixGenerator();
            List<AverageResult> results = new List<AverageResult>();

            for (int i = 0; i < Times; i++)
            {
                List<ComparisonResult> oneSizeResults = new List<ComparisonResult>();
                for (int t = 0; t < RepeatTimes; t++)
                {
                    Matrix matrix = generator.Generate(0.9f, (size, size));
                    Console.Write("generated, ");
                   
                    matrix.ResetAlgorithmInfo();
                    long parallelTime =
                        timeTester.TestSync(parallelAStar.GetShortestPath, matrix, out Path pathParallel);
                    Console.WriteLine("tested " + i);

                    AlgorithmResult sequentialAlgorithmResult = new AlgorithmResult(new Path(Array.Empty<Node>().ToList(), true), -1);
                    AlgorithmResult parallelAlgorithmResult = new AlgorithmResult(pathParallel, parallelTime);

                    oneSizeResults.Add(new ComparisonResult(size, sequentialAlgorithmResult, parallelAlgorithmResult, 0));
                }

                var averageResult = GetAverageResult(oneSizeResults);
                results.Add(averageResult);
                size += MatrixSizeStep;
            }

            return results;
        }

        private static float GetSpeedup(long sequentialTime, long parallelTime) =>
            (sequentialTime == 0 ? 0.1f : sequentialTime) / (parallelTime == 0 ? 0.1f : parallelTime);
        private static float GetSpeedup(float sequentialTime, float parallelTime) =>
            (sequentialTime == 0 ? 0.1f : sequentialTime) / (parallelTime == 0 ? 0.1f : parallelTime);

        private static AverageResult GetAverageResult(List<ComparisonResult> comparisonResults)
        {
            float averageSequentialTime = (float) comparisonResults
                .Select(result => result.SequentialResult.Time)
                .Average();
            
            float averageParallelTime = (float) comparisonResults
                .Select(result => result.ParallelResult.Time)
                .Average();
            
            return new AverageResult(
                comparisonResults.Count,
                comparisonResults
                    .First().MatrixSize,
                GetSpeedup(averageSequentialTime, averageParallelTime),
                averageSequentialTime,
                (int) comparisonResults
                    .Where(result => result.SequentialResult.Path.IsPathExisting)
                    .Select(result => result.SequentialResult.Path?.Nodes.Count)
                    .Average(),
                comparisonResults
                    .Count(result => result.SequentialResult.Path.IsPathExisting),
                averageParallelTime,
                (int) comparisonResults
                    .Where(result => result.ParallelResult.Path.IsPathExisting)
                    .Select(result => result.ParallelResult.Path?.Nodes.Count)
                    .Average(),
                comparisonResults
                    .Count(result => result.ParallelResult.Path.IsPathExisting));
        }

        private static void VisualizeResults(List<AverageResult> results)
        {
            var table = new ConsoleTable("Matrix size", "Sequential time", "Sequential path length",
                "Sequential path exists",
                "Parallel time", "Parallel path length", "Parallel path exists", "Speedup factor");

            foreach (var result in results)
            {
                table.AddRow($"{result.MatrixSize}х{result.MatrixSize}",
                    result.SequentialTime + "ms",
                    result.SequentialPathLength,
                    $"{result.SequentialFoundedCount}/{result.IterationsCount}",
                    result.ParallelTime + "ms",
                    result.ParallelPathLength,
                    $"{result.ParallelFoundedCount}/{result.IterationsCount}",
                    result.SpeedupFactor);
            }

            table.Write();

            Console.WriteLine(
                $"Average speedup acceleration - {results.Select(result => result.SpeedupFactor).Average()}");
        }
    }
}