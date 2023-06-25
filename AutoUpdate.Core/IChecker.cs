using System.Threading.Tasks;

namespace AutoUpdate.Core
{
    public interface IChecker
    {
        Task<bool> CheckUpdate();
        bool CanUpdate();
        Task<bool> DownloadPackage();
        string GetPackagePath();
    }
}
