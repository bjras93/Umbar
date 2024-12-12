namespace Umbar.Docker.Models;

public sealed class DockerService
{
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string ConfigFiles { get; set; }
    public bool IsRunning()
    {
        return Status.Contains("running", StringComparison.OrdinalIgnoreCase);
    }
}