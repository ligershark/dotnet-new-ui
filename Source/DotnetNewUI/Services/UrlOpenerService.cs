namespace DotnetNewUI.Services;

using System.Diagnostics;

public class UrlOpenerService : IUrlOpenerService
{
    public void Open(Uri url)
    {
        if (OperatingSystem.IsWindows())
        {
            using var process = Process.Start(
                new ProcessStartInfo(url.ToString())
                {
                    UseShellExecute = true,
                });
        }
        else
        {
            using var process = Process.Start(
                new ProcessStartInfo("open", url.ToString())
                {
                    UseShellExecute = true,
                });
        }
    }
}
