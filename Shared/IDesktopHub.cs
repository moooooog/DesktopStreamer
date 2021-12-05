using System.Collections.Generic;
using System.Threading;

namespace Shared
{
    public interface IDesktopHub
    {
        IAsyncEnumerable<Frame> Frames(CancellationToken cancellationToken);
    }
}
