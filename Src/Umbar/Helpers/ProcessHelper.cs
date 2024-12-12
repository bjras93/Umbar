using System.Diagnostics;

namespace Umbar.Helpers;

public static class ProcessHelper
{
    /// <summary>
    /// Runs the process with provided <paramref name="name"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="arguments"></param>
    /// <param name="quiet"></param>
    /// <param name="token"></param>
    /// <returns>Exit code.</returns>
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

        var process = new Process
        {
            StartInfo = startInfo
        };
        process.Start();

        await process.WaitForExitAsync(token ?? CancellationToken.None);

        return process.ExitCode;
    }
}