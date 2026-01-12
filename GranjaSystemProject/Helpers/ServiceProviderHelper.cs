namespace GranjaSystemProject.Helpers;
public static class ServiceProviderHelper
{
    public static IServiceProvider ServiceProvider { get; private set; }
    public static void Configure(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }
    public static T GetService<T>() => ServiceProvider.GetService<T>();
}
