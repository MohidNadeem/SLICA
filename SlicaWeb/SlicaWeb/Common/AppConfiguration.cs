namespace SlicaWeb.Common
{
    public class AppConfiguration
    {
        public static IConfigurationRoot configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

    }
}
