using System.Diagnostics;

namespace Umbar.Helpers;

public static class ProcessHelper
{
    public static async Task<int> RunAsync(
        string name,
        string arguments,
        bool quiet = false,
        CancellationToken? token = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = name,
            UseShellExecute = false,
            Arguments = arguments
        };
        if (quiet)
        {
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
        }

        // Create and start the process
        var process = new Process
        {
            StartInfo = startInfo
        };
        process.Start();

        await process.WaitForExitAsync(token ?? CancellationToken.None);

        return process.ExitCode;
    }
}