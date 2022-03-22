namespace DotnetNewUI.Options;

public class MutableProvider : ConfigurationProvider
{
    public override void Set(string key, string value)
    {
        if (value == null)
        {
            this.Data.Remove(key);
        }
        else
        {
            this.Data[key] = value;
        }
    }
}
