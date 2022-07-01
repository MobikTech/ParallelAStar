using System;
using System.Collections.Generic;
using System.Linq;
using SequentialAStar.Common;
using SequentialAStar.Common.Generators;
using SequentialAStar.ParallelAStar;
using SequentialAStar.SequentialAStar;
using TesterAdditional;

namespace MultipleMatricesTester
{
    public class Program
    {
        private const int StartMatrixSize = 50;
        private const int MatrixSizeStep = 50;
        private const int Times = 1;
        
        static void Main(string[] args)
        {
            IPathFinder sequentialAStar = new SequentialAStar.SequentialAStar.SequentialAStar();
            IPathFinder parallelAStar = new ParallelAStar();
            
            IEnumerable<(int, Result, Result)> results = TestMultiple(sequentialAStar, parallelAStar);
            
            VisualizeResults(results);
        }

        private static IEnumerable<(int, Result, Result)> TestMultiple(IPathFinder sequentialAStar, IPathFinder parallelAStar)
        {
            int size = StartMatrixSize;
            TimeWatcher timeTester = new TimeWatcher();
            IMatrixGenerator generator = new RandomMatrixGenerator();
            List<(int, Result, Result)> results = new List<(int, Result, Result)>();

            for (int i = 0; i < Times; i++)
            {
                Matrix matrix = generator.Generate(0.9f, (size, size));
                Console.Write("generated, ");

                long sequentialTime = timeTester.TestSync(sequentialAStar.GetShortestPath, matrix, out Path pathSequential);
                long parallelTime = timeTester.TestSync(parallelAStar.GetShortestPath, matrix, out Path pathParallel);
                Console.WriteLine("tested " + i);

                Result sequentialResult = new Result(pathSequential, sequentialTime);
                Result parallelResult = new Result(pathParallel, parallelTime);
                
                results.Add((size, sequentialResult, parallelResult));
                size += MatrixSizeStep;
            }

            return results;
        }

        private static void VisualizeResults(IEnumerable<(int matrixSize, Result sequentialResult, Result parallelResult)> results)
        {
            Console.WriteLine($"Matrix size\tSeq time\tSeq path length\tSeq path exists\tPar time\tPar path length\tPar path exists");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.matrixSize}х{result.matrixSize}" +
                                  $"\t\t{result.sequentialResult.Time} ms" +
                                  $"\t\t{result.sequentialResult.Path.Nodes.Count()}" +
                                  $"\t\t{result.sequentialResult.Path.IsPathExisting}" +
                                  $"\t\t{result.parallelResult.Time} ms" +
                                  $"\t\t{result.parallelResult.Path.Nodes.Count()}" +
                                  $"\t\t{result.parallelResult.Path.IsPathExisting}");
            }
        }

        private struct Result
        {
            public readonly Path Path;
            public readonly long Time;

            public Result(Path path, long time)
            {
                Path = path;
                Time = time;
            }
        }
        
        // private static IEnumerable<(int, long, long)> TestMultipleWithGeneration(int startSize, int step, int times, IPathFinder pathFinder)
        // {
        //     int size = startSize;
        //     TimeTester timeTester = new TimeTester();
        //     IMatrixGenerator generator = new RandomMatrixGenerator();
        //     List<(int, long, long)> results = new List<(int, long, long)>();
        //
        //     for (int i = 0; i < times; i++)
        //     {
        //         long generationTime = timeTester.TestSync(generator.Generate, 0.9f, (size, size), out Matrix matrix);
        //         Console.Write("generated ");
        //         long time = timeTester.TestSync(pathFinder.GetShortestPath, matrix, out Path path);
        //         results.Add((size, time, generationTime));
        //         size += step;
        //         Console.WriteLine(i);
        //     }
        //
        //     return results;
        // }
        //
        //
        // private static void VisualizeResultsWithGeneration(IEnumerable<(int matrixSize, long time, long generationTime)> results)
        // {
        //     Console.WriteLine($"Matrix size\tTime\tGeneration time");
        //     foreach (var result in results)
        //     {
        //         Console.WriteLine($"{result.matrixSize}\t\t{result.time} ms\t\t{result.generationTime} ms");
        //     }
        // }
    }
    
}