using Topshelf;

namespace SignalRServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ServicesHost>(s =>
                {
                    s.ConstructUsing(name => new ServicesHost());
                    s.WhenStarted(sv => sv.Start());
                    s.WhenStopped(sv => sv.Stop());
                });

                x.EnableServiceRecovery(t =>
                {
                    t.RestartService(0);
                    t.RestartService(0);
                    t.RestartService(0);
                    t.OnCrashOnly();
                });

                x.RunAsLocalSystem();
                x.StartAutomaticallyDelayed();

                x.SetServiceName(ConfigHelper.Instance.ServiceName);
                x.SetDisplayName(ConfigHelper.Instance.DisplayName);
                x.SetDescription(ConfigHelper.Instance.Description);
            });
        }
    }
}