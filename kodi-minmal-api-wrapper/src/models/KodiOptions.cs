namespace KodiMinimalApi.Models;

public class KodiOptions
{
    public string Host { get; set; } = "192.168.0.100";
    public int Port { get; set; } = 8080;
    public string? Username { get; set; } = "kodi";
    public string? Password { get; set; } = "1233";

}
