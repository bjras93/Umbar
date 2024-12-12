using System.Diagnostics;
using System.Text;

namespace Umbar.Helpers;

public static class ShellHelper
{
    private const string Prefix = "#!/bin/bash\n";
    private static readonly string ScriptPath = Path.Combine(AppContext.BaseDirectory, "script.sh");
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

        await ProcessHelper.RunAsync("chmod", $"u+x {ScriptPath}");

        var scriptProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = ScriptPath,
                UseShellExecute = false
            }
        };
        scriptProcess.Start();
        scriptProcess.WaitForExit();
    }
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
        await File.WriteAllTextAsync(ScriptPath, script);
    }
}