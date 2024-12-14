using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Umbar.Common;
using Umbar.Helpers;
using Umbar.Models;

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

        App? app = null;
        if (!string.IsNullOrWhiteSpace(settings.Name))
        {
            app = config.Apps.FirstOrDefault(a =>
                    a.Name.Contains(settings.Name))
                    ?? throw new ArgumentException($"No app found with name: {settings.Name}");
        }
        app ??= config.Apps.FirstOrDefault(a =>
                currentDirectory.Contains(a.Name));

        HashSet<App>? apps = app == null ? [] : [app];
        if (apps.Count == 0 || settings.All)
        {
            apps = await Prompt(config, settings);
        }
        if (apps.Count == 0)
        {
            AnsiConsole.WriteLine("No apps were added.");
            return 1;
        }
        var tasks = apps.Select(a => Docker.Commands.Pull(a.Path));

        await Task.WhenAll(tasks);
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
            return (await config.Apps.MultiSelectionPromptAsync(true, a => a.Name)).ToHashSet();
        }
        return (await config.Apps.MultiSelectionPromptAsync(nameConverter: a => a.Name)).ToHashSet();
    }
}
public sealed class PullSettings : DefaultSettings
{
    [CommandOption("-a|--all")]
    [Description("Marks all apps in the multi selection.")]
    public bool All { get; set; }
    [CommandOption("-f|--force")]
    [Description("Skips the multi selection.")]
    public bool Force { get; set; }
    [CommandArgument(0, "[Name]")]
    [Description("Name of the app to be pulled.")]
    public string? Name { get; set; }
}