using Microsoft.Extensions.Logging;

namespace SportChallenge.Logger;

public class FileLogger : ILogger
{
    private string filePath;
    private static object _lock = new object();

    public FileLogger(string path)
    {
        filePath = path;
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (formatter != null && eventId == 0)
        {
            lock (_lock)
            {
                var n = Environment.NewLine;
                var exc = "";
                if (exception != null) exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                Console.WriteLine(logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter!.Invoke(state, exception) + n + exc);
                File.AppendAllText(filePath, logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc);
            }
        }
        else
        {
            var n = Environment.NewLine;
            var exc = "";
            if (exception != null) exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
            Console.WriteLine(logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter!.Invoke(state, exception) + n + exc);
        }
    }
}