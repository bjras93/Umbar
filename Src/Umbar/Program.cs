using Spectre.Console.Cli;
using System.Text.Json.Serialization;
using Umbar.Commands;
using Umbar.Common;
using Umbar.Models;

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