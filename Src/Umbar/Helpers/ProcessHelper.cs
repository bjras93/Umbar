using System.Diagnostics;

namespace Umbar.Helpers;

public static class ProcessHelper
{
    public static async Task<int> RunAsync(
        string name,
        string arguments)
    => await Task.Run(() =>
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = name, // Specify the shell to use
            RedirectStandardOutput = true, // Redirect output for reading
            RedirectStandardInput = true,
            UseShellExecute = false, // Don't use the shell to execute the command directly
            Arguments = arguments
        };

        // Create and start the process
        var process = new Process
        {
            StartInfo = startInfo
        };
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
    });
}