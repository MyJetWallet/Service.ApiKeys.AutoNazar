using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.ApiKeys.AutoNazar.Jobs;

namespace Service.ApiKeys.AutoNazar
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyNoSqlClientLifeTime _noSqlTcpClient;
        private readonly AutoNazarJob _autoNazarJob;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeManager> logger,
            MyNoSqlClientLifeTime noSqlTcpClient,
            AutoNazarJob autoNazarJob) : base(
            appLifetime)
        {
            _logger = logger;
            _noSqlTcpClient = noSqlTcpClient;
            _autoNazarJob = autoNazarJob;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called");
            _noSqlTcpClient.Start();
            _autoNazarJob.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called");
            _noSqlTcpClient.Stop();
            _autoNazarJob.Dispose();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called");
        }
    }
}
