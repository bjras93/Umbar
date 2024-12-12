using Spectre.Console.Cli;
using System.ComponentModel;
using Umbar.Common;
using Umbar.Helpers;
using Umbar.Models;

namespace Umbar.Commands;

[Description("Downloads a yaml file from provided url")]
public sealed class NewCommand : AsyncCommand<NewSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        NewSettings settings)
    {

        var url = settings.Url;
        if (string.IsNullOrWhiteSpace(settings.Location))
        {
            settings.Location = settings.Force ? Directory.GetCurrentDirectory() :
             await PromptHelper.TextPromptAsync("Insert folder location or keep current folder", Directory.GetCurrentDirectory());
        }

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url must be set");

        var uri = new Uri(url!);

        var extension = Path.GetExtension(uri.LocalPath);
        if (!extension.Equals(".yaml", StringComparison.OrdinalIgnoreCase) && !extension.Contains("yml", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Url must end as yaml or yml");

        var fileName = Path.GetFileName(uri.LocalPath);

        var client = new HttpClient();
        using var stream = await client.GetStreamAsync(uri);
        using var fileStream = new FileStream(fileName, FileMode.CreateNew);

        await stream.CopyToAsync(fileStream);
        await fileStream.FlushAsync();
        if (settings.Start)
            await Docker.Commands.Up(Path.Combine(settings.Location, fileName));

        var config = await ConfigurationManager.GetAsync();

        config.Apps.Add(new App
        {
            Name = Path.GetFileName(settings.Location),
            Path = Path.Combine(settings.Location, fileName)
        });
        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }
}

public sealed class NewSettings : DefaultSettings
{
    [CommandOption("-f|--force")]
    [Description("Forces skip of folder location prompt, and uses current directory.")]
    public bool Force { get; init; }

    [CommandOption("-s|--start")]
    [Description("Runs compose up command after downloading the file.")]
    public bool Start { get; init; }

    [CommandArgument(1, "[path]")]
    [Description("Path to store the downloaded compose file.")]
    public string? Location { get; set; }

    [CommandArgument(0, "<url>")]
    [Description("Url of the yaml file.")]
    public string? Url { get; set; }
}