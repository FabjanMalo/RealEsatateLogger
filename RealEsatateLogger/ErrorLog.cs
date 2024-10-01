namespace RealEsatateLogger;

public class ErrorLog
{
    public Guid Id { get;  set; }
    public string Message { get;  set; }
    public string Source { get;  set; }
    public string? Exception { get;  set; }
    public DateTime CreatedOnUtc { get;  set; }
}
