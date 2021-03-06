﻿using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using X42.Configuration.Logging;
using X42.Feature.Database;
using X42.Feature.Setup;
using X42.Feature.X42Client;
using X42.MasterNode;
using X42.Server;
using X42.Utilities;

namespace X42.Feature.Network
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides an ability to communicate with different network types.
    /// </summary>
    public class NetworkFeatures : ServerFeature
    {
        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>Global application life cycle control - triggers when application shuts down.</summary>
        private readonly IX42ServerLifetime serverLifetime;

        /// <summary>Factory for creating background async loop tasks.</summary>
        private readonly IAsyncLoopFactory asyncLoopFactory;

        private readonly MasterNodeBase network;
        private readonly NetworkMonitor networkMonitor;
        private readonly DatabaseSettings databaseSettings;
        private readonly X42ClientSettings x42ClientSettings;

        public NetworkFeatures(
            MasterNodeBase network,
            ILoggerFactory loggerFactory,
            DatabaseSettings databaseSettings,
            X42ClientSettings x42ClientSettings,
            IX42ServerLifetime serverLifetime,
            IAsyncLoopFactory asyncLoopFactory
            )
        {
            this.network = network;
            logger = loggerFactory.CreateLogger(GetType().FullName);
            this.databaseSettings = databaseSettings;
            this.x42ClientSettings = x42ClientSettings;
            this.serverLifetime = serverLifetime;
            this.asyncLoopFactory = asyncLoopFactory;
        }

        /// <summary>
        ///     Prints command-line help.
        /// </summary>
        /// <param name="masterNodeBase">The masternode to extract values from.</param>
        public static void PrintHelp(MasterNodeBase masterNodeBase)
        {
            NetworkSettings.PrintHelp(masterNodeBase);
        }

        /// <summary>
        ///     Get the default configuration.
        /// </summary>
        /// <param name="builder">The string builder to add the settings to.</param>
        /// <param name="network">The network to base the defaults off.</param>
        public static void BuildDefaultConfigurationFile(StringBuilder builder, MasterNodeBase network)
        {
            NetworkSettings.BuildDefaultConfigurationFile(builder, network);
        }

        /// <summary>
        ///     Connect to the network.
        /// </summary>
        public void Connect()
        {
            logger.LogInformation("Connecting to network");
        }

        public void Disconnect()
        {
            logger.LogInformation("Disconnected from network");
        }

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            NetworkMonitor networkMonitor = new NetworkMonitor(logger, serverLifetime, asyncLoopFactory, databaseSettings, x42ClientSettings);

            networkMonitor.Start();

            logger.LogInformation("Network Initialized");

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            Disconnect();
        }

        /// <inheritdoc />
        public override void ValidateDependencies(IServerServiceProvider services)
        {
            // TODO: Check settings and verify features here, then throw exception if not valid
            // Example: throw new ConfigurationException("Something went wrong.");
        }
    }

    /// <summary>
    ///     A class providing extension methods for <see cref="NetworkFeatures" />.
    /// </summary>
    public static class NetworkBuilderExtension
    {
        /// <summary>
        ///     Adds SQL components to the server.
        /// </summary>
        /// <param name="serverBuilder">The object used to build the current node.</param>
        /// <returns>The server builder, enriched with the new component.</returns>
        public static IServerBuilder UseNetwork(this IServerBuilder serverBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<NetworkFeatures>("network");

            serverBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<NetworkFeatures>()
                    .DependOn<DatabaseFeatures>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<NetworkFeatures>();
                        services.AddSingleton<NetworkSettings>();
                    });
            });

            return serverBuilder;
        }
    }
}