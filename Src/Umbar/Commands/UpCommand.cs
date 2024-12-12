using Spectre.Console.Cli;
using System.ComponentModel;
using Umbar.Common;
using Umbar.Helpers;

namespace Umbar.Commands;

[Description("Starts an app from the config.json")]
public sealed class UpCommand : AsyncCommand<UpSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        UpSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();

        string? path = null;
        if (!string.IsNullOrWhiteSpace(settings.Name))
            path = config.Apps.FirstOrDefault(a => a.Name.Contains(settings.Name, StringComparison.OrdinalIgnoreCase))?.Path;

        if (string.IsNullOrWhiteSpace(path))
            path = (await config.Apps.SelectionPromptAsync(c => c.Name)).Path;

        await Docker.Commands.Up(path);

        return 0;
    }
}

public sealed class UpSettings : DefaultSettings
{
    [CommandArgument(0, "[name]")]
    [Description("Name of the app.")]
    public string? Name { get; init; }
}