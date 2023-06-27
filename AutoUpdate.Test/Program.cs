// See https://aka.ms/new-console-template for more information
using AutoUpdate.Core;

Console.WriteLine("Hello, World!");

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
while (true)
{
    Thread.Sleep(1000);
    Console.WriteLine($"[{DateTime.Now}] Alive");
}