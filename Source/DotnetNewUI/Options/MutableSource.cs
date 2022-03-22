namespace DotnetNewUI.Options;

public class MutableSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new MutableProvider();
}
