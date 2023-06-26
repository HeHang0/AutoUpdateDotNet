using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpdate.Core
{
    public class PackageCheckedEventArgs : EventArgs
    {
        public PackageCheckedEventArgs(IChecker checker, string version)
        {
            this.checker = checker;
            this.version = version;
        }

        private readonly IChecker checker;
        public IChecker Checker => checker;

        private readonly string version;
        public string Version => version;
    }
}
