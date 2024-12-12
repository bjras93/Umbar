using System.Text.Json.Serialization;
using Umbar.Commands;
using Umbar.Models;
using Spectre.Console.Cli;
using Umbar.Common;

var commandApp = new CommandApp();

commandApp.Configure(config =>
{
    config.SetApplicationName("umbar");
    config.SetHelpProvider(new CustomHelpProvider(config.Settings));
    config
        .AddCommand<RemoveCommand>("remove");
    config
        .AddCommand<AddCommand>("add");
    config
        .AddCommand<PullCommand>("pull");
    config
        .AddCommand<RestartCommand>("restart");
});
await commandApp.RunAsync(args);


[JsonSerializable(typeof(Config))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}