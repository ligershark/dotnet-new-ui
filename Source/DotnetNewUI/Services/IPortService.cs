namespace DotnetNewUI.Services;

public interface IPortService
{
    IConfigurationRoot Configuration { get; }

    int Port { set; }
}
