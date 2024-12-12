using System.Diagnostics;
using System.Text;

namespace Umbar.Helpers;

public static class ShellHelper
{
    private const string Prefix = "#!/bin/bash\n";
    private static readonly string scriptPath = Path.Combine(AppContext.BaseDirectory, "script.sh");
    /// <summary>
    /// Starts a shell script created from the <paramref name="script"/> arguments.
    /// </summary>
    /// <param name="script">Each line of the script.</param>
    public static async Task Start(
        params string[] script
    )
    {
        var allLines = script.Select(l => l + "\n");
        var sb = new StringBuilder();
        foreach (var line in allLines)
        {
            sb.Append(line);
        }
        await Write(sb.ToString());

        await ProcessHelper.RunAsync("chmod", $"u+x {scriptPath}");

        var scriptProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = scriptPath,
                UseShellExecute = false
            }
        };
        scriptProcess.Start();
        await scriptProcess.WaitForExitAsync();
    }
    /// <summary>
    /// Changes directory in the shell.
    /// </summary>
    /// <param name="directory">Directory that the shell should change to.</param>
    public static async Task ChangeDirectory(
        string directory)
    {
        var script = $"""
{Prefix}
cd {directory} 
$SHELL
""";
        await Start(script);
    }
    private static async Task Write(
        string script)
    {
        await File.WriteAllTextAsync(scriptPath, script);
    }
}