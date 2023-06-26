using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpdate.Core
{
    public interface IInstaller
    {
        void Install(string packagePath);
    }
}
