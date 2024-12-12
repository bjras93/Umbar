using Umbar.Helpers;
using Umbar.Models;
using CLI.Common.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Umbar.Commands;

[Description("Pulls images from one or more apps from the config.json")]
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
            AnsiConsole.WriteLine("Please select which app to pull");
            app = await config.Applications.SelectionPromptAsync(a => a.Name);
        }
        var output = await ProcessHelper.RunAsync(Constants.Docker, $"compose -f {app.Path} pull");

        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }
}
