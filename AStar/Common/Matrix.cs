namespace AStar.Common
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

        public void ResetAlgorithmInfo()
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Nodes[x, y].SequentialInfo.Reset();
                    Nodes[x, y].ParallelInfo.Reset();
                }
            }
        }
    }
}