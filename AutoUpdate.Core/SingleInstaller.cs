using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AutoUpdate.Core
{
    public class SingleInstaller : IInstaller
    {
        public void Install(string packagePath)
        {
            var process = Process.GetCurrentProcess();
            var currentPath = process.MainModule.FileName;
            Console.WriteLine("processId: " + process.Id);
            Console.WriteLine("currentPath: " + currentPath);
            Console.WriteLine("packagePath: " + packagePath);

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
                RedirectStandardOutput = false,
                CreateNoWindow = false
            };

            new Process { StartInfo = startInfo }.Start();

            Environment.Exit(0);
        }
    }
}
