using Spectre.Console;
using Spectre.Console.Cli;
using System.Collections.Concurrent;
using System.ComponentModel;
using Umbar.Common;
using Umbar.Helpers;
using Umbar.Models;

namespace Umbar.Commands;

[Description("Adds apps to the config.json")]
public sealed class AddCommand : AsyncCommand<AddSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, AddSettings settings)
    {
        var config = await ConfigurationManager.GetAsync();
        int exitCode;
        if (settings.Find)
        {
            exitCode = await FindAll(config, settings.MaxDepth);
        }
        else
        {
            exitCode = await Manual(config);
        }

        await ConfigurationManager.UpdateAsync(config);
        return exitCode;
    }
    private static async Task<int> Manual(Config config)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        List<string> files = [];
        while (files.Count == 0)
        {
            files = Directory.GetFiles(currentDirectory).Where(c =>
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
                currentDirectory = await PromptHelper.TextPromptAsync($"No compose file in current folder, please insert desired folder.", currentDirectory, true);
                currentDirectory = currentDirectory.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
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
        config.Apps.Add(new App
        {
            Name = directoryName,
            Path = Path.Combine(currentDirectory, dockerFile)
        });
        return 0;
    }
    /// <summary>
    /// Iterates through all sub folders to find folders with yaml/yml files
    /// </summary>
    /// <returns>Exit code</returns>
    private static async Task<int> FindAll(
        Config config,
        int maxDepth)
    {
        var dict = new ConcurrentDictionary<string, string>();
        var directories = DirectoryHelper.GetAccessibleDirectories(Directory.GetCurrentDirectory(), "*", maxDepth);
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        await Parallel.ForEachAsync(directories, parallelOptions, async (directory, ct) =>
        {
            var yamlFile = (await DirectoryHelper.GetAccessibleFiles(directory))
            .FirstOrDefault(f =>
            {
                var ext = Path.GetExtension(f);

                return ext.Equals(".yaml", StringComparison.OrdinalIgnoreCase) ||
                        ext.Equals(".yml", StringComparison.OrdinalIgnoreCase);
            });

            if (yamlFile == null)
                return;

            if (await ProcessHelper.RunAsync(Constants.Docker, $"compose -f \"{yamlFile}\" config -q", true) != 0)
                return;

            dict.TryAdd(Path.GetFileName(directory), yamlFile);

        });

        var apps = dict.Select(d => new MultiSelectItem<App>
        {
            Data = new App
            {
                Name = d.Key,
                Path = d.Value
            },
            Selected = config.Apps.Any(a => a.Name.Equals(d.Key, StringComparison.OrdinalIgnoreCase))
        });

        AnsiConsole.WriteLine("Please select the apps you'd want to add");

        var addApps = await apps.MultiSelectionPromptAsync(a => a.Name);
        foreach (var app in addApps)
        {
            config.Apps.Add(app);
        }
        return 0;
    }

}

public sealed class AddSettings : DefaultSettings
{
    [CommandOption("-f|--find-all")]
    [Description("Recursively looks through all subfolders to determine if any compose files exists.")]
    public bool Find
    {
        get; init;
    }

    [CommandArgument(0, "[depth]")]
    [Description("Determines the depth of folders to recursively look through, default is 3.")]
    public int MaxDepth { get; init; } = 3;
}