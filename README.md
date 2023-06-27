# AutoUpdate [![NuGet](https://img.shields.io/nuget/v/AutoUpdateDotNet.Core.svg)](https://nuget.org/packages/AutoUpdateDotNet.Core) [![Build AutoUpdate](https://github.com/HeHang0/AutoUpdateDotNet/actions/workflows/core.library.nuget.yml/badge.svg)](https://github.com/HeHang0/AutoUpdateDotNet/actions/workflows/core.library.nuget.yml)


AutoUpdate is a library for automatically updating .NET applications. It can check for new update packages, download them, and install them.

## Usage
-------

AutoUpdate is available as [NuGet package](https://www.nuget.org/packages/AutoUpdateDotNet.Core).

```csharp
void NewPackageChecked(AutoUpdate.Core.AutoUpdate sender, PackageCheckedEventArgs e)
{
    Console.WriteLine($"[{DateTime.Now}] New Update: {e.Version}");
    CancellationTokenSource cts = new CancellationTokenSource();
    sender.Update(new SingleInstaller(), cts.Token, new Progress<int>(p =>
    {
        Console.WriteLine($"[{DateTime.Now}] Download Progress: {p}");
        if(p == 50) cts.Cancel();
    }));
}

var githubChecker = new GithubChecker("hehang0", "FileViewer", "FileViewer.exe", "v1.3.0");
var autoUpdate = new AutoUpdate.Core.AutoUpdate(new Options(githubChecker));
autoUpdate.NewPackageChecked += NewPackageChecked;

autoUpdate.Start();
```

## Repository
-------

The source code for AutoUpdateDotNet is hosted on GitHub. You can find it at the following URL: [https://github.com/HeHang0/AutoUpdateDotNet](https://github.com/HeHang0/AutoUpdateDotNet)

## License
-------

AutoUpdateDotNet is released under the MIT license. This means you are free to use and modify it, as long as you comply with the terms of the license.