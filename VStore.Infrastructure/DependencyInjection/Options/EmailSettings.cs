namespace VStore.Infrastructure.DependencyInjection.Options;

public class EmailSettings
{
    public static readonly string EmailSection = "EmailSettings";
    public string FromEmailAddress { get; set; }
    public string FromDisplayName { get; set; }
    public Smtp Smtp { get; set; }
}

public class Smtp
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
}