﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using X42.Configuration.Logging;
using X42.Feature.Setup;
using X42.MasterNode;
using X42.Server;

namespace X42.Feature.Database
{
    /// <summary>
    /// Provides an ability to comminicate with diffrent database types.
    /// </summary>
    public class DatabaseFeatures : ServerFeature
    {
        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        public DatabaseFeatures(MasterNodeBase network, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Prints command-line help.
        /// </summary>
        /// <param name="network">The network to extract values from.</param>
        public static void PrintHelp(MasterNodeBase masterNodeBase)
        {
            DatabaseSettings.PrintHelp(masterNodeBase);
        }

        /// <summary>
        /// Get the default configuration.
        /// </summary>
        /// <param name="builder">The string builder to add the settings to.</param>
        /// <param name="network">The network to base the defaults off.</param>
        public static void BuildDefaultConfigurationFile(StringBuilder builder, MasterNodeBase network)
        {
            DatabaseSettings.BuildDefaultConfigurationFile(builder, network);
        }

        /// <summary>
        /// Connect to the database.
        /// </summary>
        /// <param name="walletName">The name of the wallet.</param>
        /// <param name="walletPassword">The password of the wallet.</param>
        public void Connect()
        {
            this.logger.LogInformation("Connected to database");
        }

        public void Disconnect()
        {
            this.logger.LogInformation("Disconnected from database");
        }

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            this.logger.LogInformation("Database Feature Initialized");

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            this.Disconnect();
        }

        /// <inheritdoc />
        public override void ValidateDependencies(IServerServiceProvider services)
        {
            // TODO: Check settings and verify features here, then throw exteption if not valid
            // Example: throw new ConfigurationException("Somethign went wrong.");
        }
    }

    /// <summary>
    /// A class providing extension methods for <see cref="DatabaseFeatures"/>.
    /// </summary>
    public static class DatabaseBuilderExtension
    {        /// <summary>
             /// Adds POW and POS miner components to the node, so that it can mine or stake.
             /// </summary>
             /// <param name="fullNodeBuilder">The object used to build the current node.</param>
             /// <returns>The full node builder, enriched with the new component.</returns>
        public static IServerBuilder UsePostgreSQL(this IServerBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<DatabaseFeatures>("database");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<DatabaseFeatures>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<DatabaseFeatures>();
                    });
            });

            return fullNodeBuilder;
        }

        public static IServerBuilder UseMongoDB(this IServerBuilder fullNodeBuilder)
        {
            throw new NotImplementedException("MongoDB is not yet supported");
        }
    }
}