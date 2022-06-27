using System;

namespace SequentialAStar.Common
{
    public interface IPathFinder
    {
        public Path GetShortestPath(Matrix matrix);
    }
}