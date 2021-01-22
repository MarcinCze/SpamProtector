namespace ProtectorLib.Providers
{
    public interface IServiceRunScheduleProvider
    {
        bool ShouldRun(string serviceName);
    }
}
