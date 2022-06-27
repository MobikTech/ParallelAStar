using System.Collections.Generic;

namespace SequentialAStar.Common
{
    public enum Passing
    {
        Passable,
        Unpassable
    }
    
    public class Node
    {
        public Node(Position position, Passing passable)
        {
            Position = position;
            Passable = passable;
            Neighbors = new List<Node>();
        }

        public IEnumerable<Node> Neighbors { get; set; }
        public Position Position { get; }
        public Passing Passable { get; }
        public IEnumerable<Node>? PathToThis { get; set; }
    }
}