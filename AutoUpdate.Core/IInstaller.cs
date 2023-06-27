using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AutoUpdate.Core
{
    public interface IInstaller
    {
        void Install(CancellationToken? token, string packagePath);
    }
}
