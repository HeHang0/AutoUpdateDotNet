using System;
using System.Threading;

namespace AutoUpdate.Core
{
    public class AutoUpdate
    {
        public event NewPackageCheckedEventHandler NewPackageChecked;

        private Options options;

        private Timer timer = null;

        public AutoUpdate(Options options)
        {
            this.options = options;
        }

        public AutoUpdate Start()
        {
            if(options.Interval != TimeSpan.Zero)
            {
                timer = new Timer(CheckUpdate, null, TimeSpan.Zero, options.Interval);
            }
            else
            {
                CheckUpdate(null);
            }
            return this;
        }

        private async void CheckUpdate(object state)
        {
            bool needUpdate = await options.Checker.CheckUpdate();
            if (needUpdate)
            {
                NewPackageChecked?.Invoke(this, new PackageCheckedEventArgs(options.Checker));
            }
        }

        public async void Update()
        {
            if(!options.Checker.CanUpdate())
            {
                return;
            }
            if(!await options.Checker.DownloadPackage())
            {

            }
        }

        public AutoUpdate Stop()
        {
            timer?.Dispose();
            timer = null;
            return this;
        }
    }
}
