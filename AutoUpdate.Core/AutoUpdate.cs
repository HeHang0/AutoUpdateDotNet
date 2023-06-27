using Microsoft.Extensions.Logging;
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
                Logger.Log.LogInformation("AutoUpdate Timer Start");
                timer = new Timer(CheckUpdate, null, TimeSpan.Zero, options.Interval);
            }
            else
            {
                Logger.Log.LogInformation("AutoUpdate Start Once");
                CheckUpdate(null);
            }
            return this;
        }

        private async void CheckUpdate(object state)
        {
            (bool needUpdate, string version) = await options.Checker.CheckUpdate();
            Logger.Log.LogInformation($"AutoUpdate CheckUpdate NeedUpdate: {needUpdate}, Version: {version}");
            if (needUpdate)
            {
                NewPackageChecked?.Invoke(this, new PackageCheckedEventArgs(options.Checker, version));
            }
        }

        public async void Update(IInstaller installer, CancellationToken? token, IProgress<int> progress = null)
        {
            if(!options.Checker.CanUpdate() || (token?.IsCancellationRequested ?? false))
            {
                progress?.Report(-1);
                return;
            }
            if(!await options.Checker.DownloadPackage(token, progress) || (token?.IsCancellationRequested ?? false))
            {
                progress?.Report(-1);
                return;
            }
            installer.Install(token, options.Checker.GetPackagePath());
        }

        public AutoUpdate Stop()
        {
            timer?.Dispose();
            timer = null;
            return this;
        }
    }
}
