namespace Umbar.Models;

public sealed class MultiSelectItem<T>
{
    public required T Data { get; set; }
    public bool Selected { get; set; }
}
