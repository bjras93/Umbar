using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Umbar.Common;
using Umbar.Helpers;

namespace Umbar.Commands;

[Description("Removes one or more apps from the config.json")]
public sealed class RemoveCommand : AsyncCommand<RemoveSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, RemoveSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();
        if (config.Apps.Count == 0)
        {
            AnsiConsole.WriteLine("No apps added");
            return 0;
        }
        if (!settings.All)
        {
            AnsiConsole.WriteLine("Please select which app to delete");
            var applicationToDelete = await config.Apps.SelectionPromptAsync(a => a.Name);
            config.Apps.Remove(applicationToDelete);
        }
        else
        {
            if (!settings.Force)
                await PromptHelper.ConfirmPromptAsync("Are you sure you want to delete all?");
            config.Apps.Clear();

        }
        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }

}

public sealed class RemoveSettings : DefaultSettings
{
    [CommandOption("-a|--all")]
    [Description("Deletes all apps from the config.json")]
    public bool All
    {
        get; init;
    }
    [CommandOption("-f|--force")]
    [Description("Skips confirmation prompt when deleting all")]
    public bool Force
    {
        get; init;
    }
}