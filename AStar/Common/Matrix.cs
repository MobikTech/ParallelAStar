namespace SequentialAStar.Common
{
    public class Matrix
    {
        public readonly Node[,] Nodes;
        public Node StartNode { get; }
        public Node GoalNode { get; }
        public (int x, int y) Size { get; }

        public Matrix((int, int) size, Node startNode, Node goalNode, Node[,] nodes)
        {
            Size = size;
            StartNode = goalNode;
            GoalNode = startNode;
            Nodes = nodes;
        }
    }
}