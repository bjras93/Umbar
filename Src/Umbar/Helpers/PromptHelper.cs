using Spectre.Console;
using Umbar.Models;

namespace Umbar.Helpers;

public static class PromptHelper
{
    /// <summary>
    /// Creates a new text prompt and shows it.
    /// </summary>
    /// <param name="question">The text the user is prompted with.</param>
    /// <param name="defaultValue">The default value that will be set.</param>
    /// <param name="allowEmpty">When true the user can just press enter without inputting anything.</param>
    /// <returns>User input</returns>
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
        if (values.Count() == 1)
        {
            string name;
            if (nameConverter != null)
            {
                name = values.Select(nameConverter).First();
            }
            else
            {
                name = values.First().ToString()!;
            }

            return await ConfirmPromptAsync($"Select {name}?") ? values.First() :
                throw new Exception("No item selected");
        }
        var selectionPrompt = new SelectionPrompt<T>();

        if (nameConverter != null)
            selectionPrompt.Converter = nameConverter;

        selectionPrompt.AddChoices(values);

        return await selectionPrompt.ShowAsync(AnsiConsole.Console, CancellationToken.None);
    }
    public static async Task<IEnumerable<T>> MultiSelectionPromptAsync<T>(
            this IEnumerable<T> values,
            bool preselected = false,
            Func<T, string>? nameConverter = null)
        where T : notnull
    {
        var selectionPrompt = new MultiSelectionPrompt<T>();

        if (nameConverter != null)
            selectionPrompt.Converter = nameConverter;
        if (preselected)
        {
            foreach (var value in values)
            {
                var choice = selectionPrompt.AddChoice(value);
                choice.Select();
            }
        }
        else
        {
            selectionPrompt.AddChoices(values);
        }

        return await selectionPrompt.ShowAsync(AnsiConsole.Console, CancellationToken.None);
    }
    public static async Task<IEnumerable<T>> MultiSelectionPromptAsync<T>(
        this IEnumerable<MultiSelectItem<T>> values,
        Func<T, string>? nameConverter = null)
    where T : notnull
    {
        var selectionPrompt = new MultiSelectionPrompt<T>();

        if (nameConverter != null)
            selectionPrompt.Converter = nameConverter;
        foreach (var value in values)
        {
            var choice = selectionPrompt.AddChoice(value.Data);
            if (value.Selected)
                choice.Select();
        }

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