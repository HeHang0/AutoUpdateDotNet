using System;
using System.Threading.Tasks;

namespace AutoUpdate.Core
{
    public interface IChecker
    {
        Task<(bool, string)> CheckUpdate();
        bool CanUpdate();
        Task<bool> DownloadPackage(IProgress<int> progress = null);
        string GetPackagePath();
    }
}
