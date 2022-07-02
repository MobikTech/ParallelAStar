namespace AStar.Common
{
    public interface IPathFinder
    {
        public Path GetShortestPath(Matrix matrix);
    }
}