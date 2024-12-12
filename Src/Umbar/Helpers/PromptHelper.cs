using Spectre.Console;

namespace CLI.Common.Helpers;

public static class PromptHelper
{
    public static async Task<string> TextPromptAsync(
    string question,
    string? defaultValue = null,
    bool allowEmpty = false
    )
    {
        var textPrompt = new TextPrompt<string>(question);
        if (!string.IsNullOrWhiteSpace(defaultValue))
        {
            textPrompt.DefaultValue(defaultValue);
        }
        if (allowEmpty)
        {
            textPrompt.AllowEmpty = true;
        }
        return await textPrompt.ShowAsync(AnsiConsole.Console, CancellationToken.None);
    }
    public static async Task<T> SelectionPromptAsync<T>(
        this IEnumerable<T> values,
        Func<T, string>? nameConverter = null)
    where T : notnull
    {
        var selectionPrompt = new SelectionPrompt<T>();

        if (nameConverter != null)
            selectionPrompt.Converter = nameConverter;

        selectionPrompt.AddChoices(values);

        return await selectionPrompt.ShowAsync(AnsiConsole.Console, CancellationToken.None);
    }
    public static async Task<TEnum> SelectionPromptAsync<TEnum>(bool descending = false)
    where TEnum : struct, Enum
    {
        var selectionPrompt = new SelectionPrompt<TEnum>();
        var values = Enum.GetValues<TEnum>();
        if (descending)
            values = [.. values.OrderDescending()];
        selectionPrompt.AddChoices(values);

        return await selectionPrompt.ShowAsync(AnsiConsole.Console, CancellationToken.None);
    }
    public static async Task<bool> ConfirmPromptAsync(
        string question
        )
    {
        var prompt = new ConfirmationPrompt(question);

        return await prompt.ShowAsync(AnsiConsole.Console, CancellationToken.None);
    }
}