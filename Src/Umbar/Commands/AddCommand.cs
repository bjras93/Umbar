using Umbar.Models;
using CLI.Common.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Umbar.Commands;

public sealed class AddCommand : AsyncCommand<DefaultSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DefaultSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();

        var currentDirectory = Directory.GetCurrentDirectory();
        currentDirectory = await PromptHelper.TextPromptAsync($"Would you like to use the current?", currentDirectory, true);
        currentDirectory = currentDirectory.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        var files = Directory.GetFiles(currentDirectory).Where(c =>
        {
            var ext = Path.GetExtension(c);
            if (ext == ".yaml" || ext == ".yml")
            {
                return true;
            }
            return false;
        }).ToList();

        if (files.Count == 0)
        {
            AnsiConsole.WriteLine("No dockerfile found");
            return 0;
        }

        string dockerFile;
        if (files.Count > 1)
        {
            dockerFile = await files.SelectionPromptAsync();
        }
        else
        {
            dockerFile = files.First();
        }

        var directoryName = Path.GetFileName(currentDirectory);
        directoryName = await PromptHelper.TextPromptAsync($"Would you to keep this name?", directoryName, true);
        config.Applications.Add(new App
        {
            Name = directoryName,
            Path = Path.Combine(currentDirectory, dockerFile)
        });
        await ConfigurationManager.UpdateAsync(config);
        return 0;
    }
}
