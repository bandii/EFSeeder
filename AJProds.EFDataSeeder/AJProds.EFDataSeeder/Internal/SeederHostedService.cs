using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AJProds.EFDataSeeder.Internal
{
    internal class SeederHostedService : IHostedService
    {
        private readonly CancellationTokenSource _cts;

        private readonly IServiceProvider _servicesProvider;

        private readonly BaseSeederManager _baseSeederManager;

        private readonly ILogger<SeederHostedService> _logger;

        public SeederHostedService(IServiceProvider servicesProvider,
                                   BaseSeederManager baseSeederManager,
                                   ILogger<SeederHostedService> logger)
        {
            _servicesProvider = servicesProvider;
            _baseSeederManager = baseSeederManager;
            _logger = logger;

            _cts = new CancellationTokenSource();
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (_servicesProvider.CreateScope())
            {
                using (CancellationTokenSource linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken))
                {
                    try
                    {
                        await Task.Run(() => _baseSeederManager.SeedAsync(SeedMode.AfterAppStart),
                                       linkedCts.Token);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Data seeding could not finish!");
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}