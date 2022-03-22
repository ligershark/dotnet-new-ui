namespace DotnetNewUI;

/// <summary>
/// <see cref="ILogger"/> extension methods. Helps log messages using strongly typing and source generators.
/// </summary>
internal static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 5000,
        Level = LogLevel.Information,
        Message = "Added endpoint {Endpoint} to {Endpoints}.")]
    public static partial void AddedEndpoint(this ILogger logger, string endpoint, string endpoints);

    [LoggerMessage(
        EventId = 5001,
        Level = LogLevel.Information,
        Message = "Removed endpoint {Endpoint} to {Endpoints}.")]
    public static partial void RemovedEndpoint(this ILogger logger, string endpoint, string endpoints);
}
