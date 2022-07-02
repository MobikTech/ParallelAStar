using System;
using AStar.Common;

namespace TesterAdditionalLibrary
{
    public class PathVisualizer
    {
        private const char StartSymbol = 'S';
        private const char GoalSymbol = 'G';
        private const char WalkableSymbol = '.';
        private const char PathSymbol = '*';
        private const char WallSymbol = '#';

        public void Visualize(Matrix matrix, Path path)
        {
            if (path.Nodes == null)
            {
                Console.WriteLine("PATH NOT FOUND");
                return;
            }
            for (int y = 0; y < matrix.Size.y; y++)
            {
                for (int x = 0; x < matrix.Size.x; x++)
                {
                    Node node = matrix.Nodes[x, y];
                    char symbol;
                    
                    if (node.Passable == Passing.Unpassable) symbol = WallSymbol;
                    else
                    {
                        if (node == matrix.StartNode) symbol = StartSymbol;
                        else if (node == matrix.GoalNode) symbol = GoalSymbol;
                        else if (path.Nodes.Contains(node)) symbol = PathSymbol;
                        else symbol = WalkableSymbol;
                    }

                    SwitchColor(symbol);
                    Console.Write($"{symbol}  ");
                }

                Console.WriteLine();
            }
        }

        private void SwitchColor(char symbol)
        {
            Console.ForegroundColor = symbol switch
            {
                StartSymbol => ConsoleColor.Blue,
                GoalSymbol => ConsoleColor.Blue,
                PathSymbol => ConsoleColor.Green,
                WallSymbol => ConsoleColor.Red,
                _ => ConsoleColor.White
            };
        }
    }
}