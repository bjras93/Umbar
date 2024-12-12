using Umbar.Helpers;
using Umbar.Models;
using CLI.Common.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Umbar.Commands;

public sealed class RestartCommand : AsyncCommand<DefaultSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DefaultSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();
        if (config.Applications.Count == 0)
        {
            AnsiConsole.WriteLine("No apps added");
            return 0;
        }
        var currentDirectory = Directory.GetCurrentDirectory();
        App? app = config.Applications.FirstOrDefault(a => currentDirectory.Contains(a.Name));
        if (app == null)
        {
            AnsiConsole.WriteLine("Please select which app to restart");
            app = await config.Applications.SelectionPromptAsync(a => a.Name);
        }
        AnsiConsole.WriteLine($"Restarting {app.Name}");
        await ProcessHelper.RunAsync(Constants.Docker, $"compose -f {app.Path} restart");
        await ShellHelper.Start($"docker compose -f {app.Path} logs -f");

        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }
}
