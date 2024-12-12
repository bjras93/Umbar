using Spectre.Console.Cli;
using System.ComponentModel;
using Umbar.Common;
using Umbar.Helpers;

namespace Umbar.Commands;

[Description("Stops an app from the config.json")]
public sealed class DownCommand : AsyncCommand<DownSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DownSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();

        string? path = null;
        if (!string.IsNullOrWhiteSpace(settings.Name))
            path = config.Apps.FirstOrDefault(a => a.Name.Contains(settings.Name, StringComparison.OrdinalIgnoreCase))?.Path;


        if (string.IsNullOrWhiteSpace(path))
        {
            path = (await DockerHelper.GetApp(config.Apps)).Path;
        }

        await Docker.Commands.Down(path);

        return 0;
    }
}

public sealed class DownSettings : DefaultSettings
{
    [CommandArgument(0, "[name]")]
    [Description("Name of the app.")]
    public string? Name { get; init; }
}