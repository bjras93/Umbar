using Umbar.Helpers;

namespace Umbar.Docker;

public static class Commands
{
    public static async Task Up(string fileLocation)
    => await ProcessHelper.RunAsync(Constants.Docker, $"compose -f {fileLocation} up -d");

    public static async Task Down(string fileLocation)
    => await ProcessHelper.RunAsync(Constants.Docker, $"compose -f {fileLocation} down");

    public static async Task Pull(string fileLocation)
    => await ProcessHelper.RunAsync(Constants.Docker, $"compose -f {fileLocation} pull");

    public static async Task Restart(string fileLocation)
    => await ProcessHelper.RunAsync(Constants.Docker, $"compose -f {fileLocation} restart");

    public static async Task Logs(string fileLocation)
    => await ShellHelper.Start($"docker compose -f {fileLocation} logs -f");

    public static async Task<int> Config(string yamlFile, string args)
    => await ProcessHelper.RunAsync(Constants.Docker, $"compose -f \"{yamlFile}\" config {args}", true);
}