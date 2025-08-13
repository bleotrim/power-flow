using Microsoft.Extensions.Configuration;

public class Configuration
{
    public string PortName { get; set; } = "setup_port";
    public int TimeoutMilliseconds { get; set; } = 5000;

    public Configuration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        PortName = configuration["Configuration:PortName"] ?? PortName;
        TimeoutMilliseconds = int.TryParse(configuration["Configuration:TimeoutMilliseconds"], out var timeout) ? timeout : TimeoutMilliseconds;
    }
}