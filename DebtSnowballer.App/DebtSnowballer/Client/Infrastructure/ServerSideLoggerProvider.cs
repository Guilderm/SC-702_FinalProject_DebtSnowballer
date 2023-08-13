using System.Text;
using System.Text.Json;

namespace DebtSnowballer.Client.Infrastructure;

public class ServerSideLogger : ILogger
{
    private readonly string _categoryName;
    private readonly HttpClient _httpClient;
    private readonly Uri _serverUri;

    public ServerSideLogger(HttpClient httpClient, Uri serverUri, string categoryName)
    {
        _httpClient = httpClient;
        _serverUri = serverUri;
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        // No scope support in this example
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        // Log all levels in this example
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var logMessage = new
        {
            Category = _categoryName,
            LogLevel = logLevel,
            EventId = eventId,
            State = state,
            Exception = exception?.ToString(),
            Message = formatter(state, exception)
        };

        string json = JsonSerializer.Serialize(logMessage);

        // Send the log message to the server
        try
        {
            HttpResponseMessage response = _httpClient
                .PostAsync(_serverUri, new StringContent(json, Encoding.UTF8, "application/json"))
                .Result;

            // Check the response status code and throw an exception if it's not successful
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to send log message to server. Status code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            // Handle the exception (e.g., log it to a local logger or console)
            Console.WriteLine($"Error sending log message to server: {ex.Message}");
        }
    }
}