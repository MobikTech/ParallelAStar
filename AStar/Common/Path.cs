using System.Collections.Generic;

namespace SequentialAStar.Common
{
    public record Path(IEnumerable<Node>? Nodes, bool IsPathExisting);
}