namespace SequentialAStar.Common
{
    public struct Position
    {
        public readonly int X;
        public readonly int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Position GetOffseted(int x, int y) => new Position(X + x, Y + y);
        public override string ToString() => $"({X}, {Y})";

        public static bool operator ==(Position pos1, Position pos2) => 
            pos1.X == pos2.X && pos1.Y == pos2.Y;
        
        public static bool operator !=(Position pos1, Position pos2) => 
            pos1.X != pos2.X || pos1.Y != pos2.Y;
    }
}