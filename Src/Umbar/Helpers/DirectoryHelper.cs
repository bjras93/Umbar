namespace Umbar.Helpers;

public static class DirectoryHelper
{
    /// <summary>
    /// Gets the subdirectories that are accessible
    /// </summary>
    /// <param name="rootDirectory"></param>
    /// <returns>List of directory paths</returns>
    public static IEnumerable<string> GetAccessibleDirectories(
    string rootDirectory,
    string searchPattern,
    int maxDepth = int.MaxValue,
    int currentDepth = 0)
    {
        if (currentDepth >= maxDepth)
            yield break;

        foreach (var directory in SafeEnumerateDirectories(rootDirectory, searchPattern))
        {
            yield return directory;

            foreach (var subDir in GetAccessibleDirectories(directory, searchPattern, maxDepth, currentDepth + 1))
            {
                yield return subDir;
            }
        }
    }

    private static IEnumerable<string> SafeEnumerateDirectories(string rootDirectory, string searchPattern)
    {
        try
        {
            return Directory.EnumerateDirectories(rootDirectory, searchPattern, SearchOption.TopDirectoryOnly);
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }
        catch (IOException)
        {
            return [];
        }
    }
    /// <summary>
    /// Gets the files paths that are accessible
    /// </summary>
    /// <param name="rootDirectory"></param>
    /// <returns>List of file paths</returns>
    public static async Task<IEnumerable<string>> GetAccessibleFiles(string rootDirectory)
    {
        var files = new List<string>();

        try
        {
            var yamlTask = Task.Run(() =>
            {
                foreach (var file in Directory.EnumerateFiles(rootDirectory, "*.yaml", SearchOption.TopDirectoryOnly))
                {
                    files.Add(file);
                }
            });
            var ymlTask = Task.Run(() =>
            {
                foreach (var file in Directory.EnumerateFiles(rootDirectory, "*.yml", SearchOption.TopDirectoryOnly))
                {
                    files.Add(file);
                }
            });
            await Task.WhenAll(yamlTask, ymlTask);
        }
        catch (UnauthorizedAccessException)
        {

        }
        catch (IOException)
        {
        }

        return files;
    }
}
