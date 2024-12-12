using Umbar.Models;

namespace Umbar.Helpers;

public static class DockerHelper
{
    public static async Task<App> GetApp(HashSet<App> apps)
    {
        var dockerList = await Docker.Commands.List();
        return await apps.SelectionPromptAsync(c =>
            {
                var service = dockerList.FirstOrDefault(d => d.ConfigFiles == c.Path);
                return service != null && service.IsRunning() ? $":check_mark_button: {c.Name}" :
                    $":cross_mark: {c.Name}";
            });
    }
}