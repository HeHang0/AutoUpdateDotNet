using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace AutoUpdate.Core
{
    public class SingleInstaller : IInstaller
    {
        public void Install(CancellationToken? token, string packagePath)
        {
            if(token?.IsCancellationRequested ?? false) return;
            var process = Process.GetCurrentProcess();
            var currentPath = process.MainModule.FileName;
            Logger.Log.LogInformation($"Install [{currentPath}] with Package [{packagePath}]");

            var psText = @"
param(
    [Parameter(Mandatory=$true)]
    [string]$currentPath,

    [Parameter(Mandatory=$true)]
    [string]$packagePath
)

Start-Sleep -Seconds 1

# Replace the file
Copy-Item -Path $packagePath -Destination $currentPath -Force

# Start the application
Start-Process -FilePath $currentPath";

            var psFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(currentPath) + ".update.ps1");
            File.WriteAllText(psFilePath, psText);

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File \"{psFilePath}\" -currentPath \"{currentPath}\" -packagePath \"{packagePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            new Process { StartInfo = startInfo }.Start();

            Environment.Exit(0);
        }
    }
}
