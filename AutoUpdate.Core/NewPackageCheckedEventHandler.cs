using System;
using System.Collections.Generic;
using System.Text;
using static AutoUpdate.Core.AutoUpdate;

namespace AutoUpdate.Core
{
    public delegate void NewPackageCheckedEventHandler(AutoUpdate sender, PackageCheckedEventArgs e);
}
