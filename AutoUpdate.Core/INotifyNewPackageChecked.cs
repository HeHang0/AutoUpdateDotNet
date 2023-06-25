using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AutoUpdate.Core
{
    public interface INotifyNewPackageChecked
    {
        event NewPackageCheckedEventHandler NewPackageChecked;
    }
}
