﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using X42.Feature.Database.Context;
using X42.Configuration;
using X42.Configuration.Logging;
using X42.Feature.Setup;
using X42.MasterNode;
using X42.Server;

namespace X42.Feature.Database
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides an ability to communicate with different database types.
    /// </summary>
    public class DatabaseFeatures : ServerFeature
    {
        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>Instance logger.</summary>
        private readonly DatabaseSettings databaseSettings;

        public bool DatabaseConnected { get; set; } = false;

        public DatabaseFeatures(
            MasterNodeBase network,
            ILoggerFactory loggerFactory,
            DatabaseSettings databaseSettings
            )
        {
            logger = loggerFactory.CreateLogger(GetType().FullName);
            this.databaseSettings = databaseSettings;
        }

        /// <summary>
        ///     Prints command-line help.
        /// </summary>
        /// <param name="masterNodeBase">The masternode to extract values from.</param>
        public static void PrintHelp(MasterNodeBase masterNodeBase)
        {
            DatabaseSettings.PrintHelp(masterNodeBase);
        }

        /// <summary>
        ///     Get the default configuration.
        /// </summary>
        /// <param name="builder">The string builder to add the settings to.</param>
        /// <param name="network">The network to base the defaults off.</param>
        public static void BuildDefaultConfigurationFile(StringBuilder builder, MasterNodeBase network)
        {
            DatabaseSettings.BuildDefaultConfigurationFile(builder, network);
        }

        /// <summary>
        ///     Connect to the database.
        /// </summary>
        public void Connect()
        {
            logger.LogInformation("Connected to database");
        }

        public void Disconnect()
        {
            logger.LogInformation("Disconnected from database");
        }

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            try
            {
                using (X42DbContext dbContext = new X42DbContext(databaseSettings.ConnectionString))
                {
                    logger.LogInformation("Connecting to database");
                    
                    dbContext.Database.Migrate();

                    DatabaseConnected = true;

                    logger.LogInformation("Database Feature Initialized");
                }
            }
            catch
            {
                logger.LogCritical("Database failed to Initialize.");
                logger.LogTrace("(-)[INITIALIZE_EXCEPTION]");
                throw;
            }

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
            if (string.IsNullOrEmpty(databaseSettings.ConnectionString))
            {
                throw new ConfigurationException("Connection string is required.");
            }
        }
    }

    /// <summary>
    ///     A class providing extension methods for <see cref="DatabaseFeatures" />.
    /// </summary>
    public static class DatabaseBuilderExtension
    {
        /// <summary>
        ///     Adds SQL components to the server.
        /// </summary>
        /// <param name="serverBuilder">The object used to build the current node.</param>
        /// <returns>The server builder, enriched with the new component.</returns>
        public static IServerBuilder UseSql(this IServerBuilder serverBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<DatabaseFeatures>("database");

            serverBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<DatabaseFeatures>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<DatabaseFeatures>();
                        services.AddSingleton<DatabaseSettings>();

                    });
            });

            return serverBuilder;
        }

        public static IServerBuilder UseNoql(this IServerBuilder serverBuilder)
        {
            throw new NotImplementedException("NoSQL is not yet supported");
        }
    }
}