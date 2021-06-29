﻿using System;
using System.Collections.Generic;
using System.Linq;
using Devon4Net.Domain.UnitOfWork.Enums;
using Devon4Net.Infrastructure.Common.Enums;
using Devon4Net.Infrastructure.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devon4Net.Domain.UnitOfWork.Common
{
    public static class SetupDatabaseConfiguration
    {
        private const int MaxRetryDelay = 30;
        private const int MaxRetryCount = 10;
        private static ServiceLifetime ServiceLifetime { get; set; }

        public static void SetupDatabase<T>(this IServiceCollection services, string connectionString, DatabaseType databaseType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, CosmosConfigurationParams cosmosConfigurationParams = null) where T : DbContext
        {
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrEmpty(connectionString)) throw new ArgumentException($"The provided connection string ({connectionString}) provided does not exists.");
            ServiceLifetime = serviceLifetime;
            SetDatabase<T>(services, databaseType, cosmosConfigurationParams, connectionString);
        }

        public static void SetupDatabase<T>(this IServiceCollection services, IConfiguration configuration, string connectionStringName, DatabaseType databaseType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool migrate = false, CosmosConfigurationParams cosmosConfigurationParams = null) where T : DbContext 
        {
            ServiceLifetime = serviceLifetime;
            var applicationConnectionStrings = configuration.GetSection(OptionSectionName.ConnectionStringsSection).GetChildren();
            if (applicationConnectionStrings == null) throw new ArgumentException("There are no connection strings provided.");
            var connectionString = applicationConnectionStrings.FirstOrDefault(c => c.Key.ToLower() == connectionStringName.ToLower());
            if (connectionString == null || string.IsNullOrEmpty(connectionString.Value)) throw new ArgumentException($"The provided connection string ({connectionStringName}) provided does not exists.");

            SetDatabase<T>(services, databaseType, cosmosConfigurationParams, connectionString.Value);
            if (migrate) Migrate<T>(services);

        }

        private static void Migrate<T>(IServiceCollection services) where T : DbContext
        {
            try
            {
                using var sp = services.BuildServiceProvider();
                if (sp == null)
                {
                    Devon4NetLogger.Error($"Unable to build the service provider, the migration {typeof(T).FullName} will not be launched");
                }
                else
                {
                    var context = sp.GetService(typeof(T));
                    if (context == null)
                    {
                        Devon4NetLogger.Error($"Unable to resolve {typeof(T).FullName} and the migration will not be launched");
                    }

                    ((T)context)?.Database.Migrate();
                    sp.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                Devon4NetLogger.Fatal(ex);
            }
        }

        private static void SetDatabase<T>(IServiceCollection services, DatabaseType databaseType,
            CosmosConfigurationParams cosmosConfigurationParams, string connectionString) where T : DbContext
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    services.AddDbContext<T>(options => options.UseSqlServer(connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: MaxRetryCount,
                                maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                                errorNumbersToAdd: null);
                        }),ServiceLifetime);
                    break;
                case DatabaseType.InMemory:
                    services.AddDbContext<T>(options => options.UseInMemoryDatabase(connectionString), ServiceLifetime);
                    break;
                case DatabaseType.MySql:
                    services.AddDbContext<T>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                            errorNumbersToAdd: new List<int>());
                    }),ServiceLifetime);
                    break;
                case DatabaseType.MariaDb:
                    services.AddDbContext<T>(options => options.UseMySql( connectionString, ServerVersion.AutoDetect(connectionString), sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                            errorNumbersToAdd: new List<int>());
                    }),ServiceLifetime);
                    break;
                case DatabaseType.Sqlite:
                    services.AddDbContext<T>(options => options.UseSqlite(connectionString), ServiceLifetime);
                    break;
                case DatabaseType.Cosmos:
                    if (cosmosConfigurationParams == null)
                        throw new ArgumentException($"The Cosmos configuration can not be null.");
                    services.AddDbContext<T>(options => options.UseCosmos(cosmosConfigurationParams.Endpoint,
                        cosmosConfigurationParams.Key, cosmosConfigurationParams.DatabaseName), ServiceLifetime);
                    break;
                case DatabaseType.PostgreSQL:
                    services.AddDbContext<T>(options => options.UseNpgsql(connectionString, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                            errorCodesToAdd: new List<string>());
                    }),ServiceLifetime);
                    break;
                case DatabaseType.FireBird:
                    services.AddDbContext<T>(options => options.UseFirebird(connectionString), ServiceLifetime);
                    break;
                case DatabaseType.Oracle:
                    services.AddDbContext<T>(options => options.UseOracle(connectionString), ServiceLifetime);
                    break;
                //MSAccess not updated to .net core 3.1
                //case DatabaseType.MSAccess:
                //    services.AddDbContext<T>(options => options.UseJet(connectionString), ServiceLifetime);
                //    break;
                default:
                    throw new ArgumentException("Not provided a database driver");
            }
        }
    }
}
