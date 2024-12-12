using Umbar.Models;
using CLI.Common.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Umbar.Commands;

public sealed class RemoveCommand : AsyncCommand<DefaultSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DefaultSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();
        if (config.Applications.Count == 0)
        {
            AnsiConsole.WriteLine("No apps added");
            return 0;
        }
        AnsiConsole.WriteLine("Please select which app to delete");
        var applicationToDelete = await config.Applications.SelectionPromptAsync(a => a.Name);
        config.Applications.Remove(applicationToDelete);
        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }
}
