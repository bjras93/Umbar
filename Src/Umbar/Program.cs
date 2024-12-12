using Spectre.Console.Cli;
using System.Text.Json.Serialization;
using Umbar.Commands;
using Umbar.Common;
using Umbar.Docker.Models;
using Umbar.Models;

var commandApp = new CommandApp();

commandApp.Configure(config =>
{
    config.SetApplicationName("umbar");
    config.SetHelpProvider(new CustomHelpProvider(config.Settings));
    config
        .AddCommand<AddCommand>("add");
    config
        .AddCommand<NewCommand>("new")
        .WithAlias("create");
    config
        .AddCommand<UpCommand>("up")
        .WithAlias("start");
    config
    .AddCommand<DownCommand>("down")
    .WithAlias("stop");
    config
        .AddCommand<RemoveCommand>("remove")
        .WithAlias("rm");
    config
        .AddCommand<PullCommand>("pull");
    config
        .AddCommand<RestartCommand>("restart")
        .WithAlias("reset");
});
await commandApp.RunAsync(args);


[JsonSerializable(typeof(Config))]
[JsonSerializable(typeof(DockerService[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}