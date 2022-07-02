using System.Collections.Generic;

namespace AStar.Common
{
    public record Path(List<Node>? Nodes, bool IsPathExisting);
}