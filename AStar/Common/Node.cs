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
            SequentialInfo = new SequentialInfo();
            ParallelInfo = new ParallelInfo();
        }

        public IEnumerable<Node> Neighbors { get; set; }
        public Position Position { get; }
        public Passing Passable { get; }
        public SequentialInfo SequentialInfo { get; }
        public ParallelInfo ParallelInfo { get; }


        
    }

    public class SequentialInfo
    {
        public SequentialInfo() => Reset();

        public Node? Parent { get; set; }
        public int? CountToFirst { get; set; }
        
        public void Reset()
        {
            Parent = null;
            CountToFirst = null;
        }
    }
    
    public class ParallelInfo
    {
        public ParallelInfo() => Reset();

        public Node? Parent { get; set; }
        public int? CountToFirst { get; set; }
        public bool InForwardClosedList { get; set; }
        public bool InBackwardClosedList { get; set; }
        
        public void Reset()
        {
            Parent = null;
            CountToFirst = null;
            InForwardClosedList = false;
            InBackwardClosedList = false;
        }
    }
}