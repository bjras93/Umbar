using Umbar.Helpers;
using Umbar.Models;
using CLI.Common.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Umbar.Commands;

[Description("Pulls images from one or more apps from the config.json")]
public sealed class PullCommand : AsyncCommand<PullSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        PullSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();
        if (config.Apps.Count == 0)
        {
            AnsiConsole.WriteLine("No apps added.");
            return 0;
        }
        var currentDirectory = Directory.GetCurrentDirectory();
        var directoryApp = config.Apps.FirstOrDefault(a => currentDirectory.Contains(a.Name));
        HashSet<App>? apps = directoryApp == null ? [] : [directoryApp];
        if (apps.Count == 0 || settings.All)
        {
            apps = await Prompt(config, settings);
        }
        if (apps.Count == 0)
        {
            AnsiConsole.WriteLine("No apps were added.");
            return 1;
        }
        foreach (var app in apps)
        {
            await ProcessHelper.RunAsync(Constants.Docker, $"compose -f {app.Path} pull");
        }

        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }
    private static async Task<HashSet<App>> Prompt(
        Config config,
        PullSettings settings)
    {
        if (settings.Force && settings.All)
            return [.. config.Apps];

        AnsiConsole.WriteLine("Please select which app to pull");
        if (settings.All)
        {
            return (await config.Apps.MultiSelectionPromptAsync(true)).ToHashSet();
        }
        return [await config.Apps.SelectionPromptAsync(a => a.Name)];
    }
}
public sealed class PullSettings : DefaultSettings
{
    [CommandOption("-a|--all")]
    [Description("Marks all apps in the multi selection")]
    public bool All { get; set; }
    [CommandOption("-f|--force")]
    [Description("Skips the multi selection")]
    public bool Force { get; set; }
}