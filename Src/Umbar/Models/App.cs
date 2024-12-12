namespace Umbar.Models;

public sealed class App
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public override bool Equals(object? obj)
    {
        if (obj is App other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Name?.ToLowerInvariant(),
            Path?.ToLowerInvariant());
    }
}