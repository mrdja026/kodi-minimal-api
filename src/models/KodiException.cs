namespace KodiMinimalApi.Models;

public class KodiException : Exception
{
    public KodiException(string message) : base(message) { }
}
