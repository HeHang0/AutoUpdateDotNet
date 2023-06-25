using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpdate.Core
{
    public class PackageCheckedEventArgs : EventArgs
    {
        public PackageCheckedEventArgs(IChecker checker)
        {
            this.checker = checker;
        }

        private readonly IChecker checker;
        public IChecker Checker => checker;
    }
}
