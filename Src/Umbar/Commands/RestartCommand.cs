using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Umbar.Common;
using Umbar.Helpers;
using Umbar.Models;

namespace Umbar.Commands;

[Description("Restarts apps from the config.json")]
public sealed class RestartCommand : AsyncCommand<RestartSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        RestartSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();
        if (config.Apps.Count == 0)
        {
            AnsiConsole.WriteLine("No apps added");
            return 0;
        }
        var currentDirectory = Directory.GetCurrentDirectory();
        App? app = config.Apps.FirstOrDefault(a => currentDirectory.Contains(a.Name));
        if (app == null)
        {
            AnsiConsole.WriteLine("Please select which app to restart");
            app = await config.Apps.SelectionPromptAsync(a => a.Name);
        }
        AnsiConsole.WriteLine($"Restarting {app.Name}");
        await Docker.Commands.Restart(app.Path);
        if (settings.Follow)
            await Docker.Commands.Logs(app.Path);

        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }
}
public sealed class RestartSettings : DefaultSettings
{
    [CommandOption("-f|--follow")]
    [Description("Runs the logs --follow command on docker compose after restarting app")]
    public bool Follow
    {
        get; set;
    }
}