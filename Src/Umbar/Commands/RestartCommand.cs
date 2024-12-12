using Umbar.Helpers;
using Umbar.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

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
        await ProcessHelper.RunAsync(Constants.Docker, $"compose -f {app.Path} restart");
        if (settings.Follow)
            await ShellHelper.Start($"docker compose -f {app.Path} logs -f");

        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }
}
public sealed class RestartSettings : DefaultSettings
{
    [CommandOption("-f|--follow")]
    [Description("Runs the logs --follow command on docker compose after restarting app")]
    public bool Follow { get; set; }
}