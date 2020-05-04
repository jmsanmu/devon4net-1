﻿using System;
using System.Collections.Generic;
using System.Linq;
using Devon4Net.Domain.UnitOfWork.Enums;
using EntityFrameworkCore.Jet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devon4Net.Domain.UnitOfWork.Common
{
    public static class SetupDatabaseConfiguration
    {
        private const int MaxRetryDelay = 30;
        private const int MaxRetryCount = 10;

        public static void SetupDatabase<T>(this IServiceCollection services, string connectionString, DatabaseType databaseType, CosmosConfigurationParams cosmosConfigurationParams = null) where T : DbContext
        {
            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrEmpty(connectionString)) throw new ArgumentException($"The provided connection string ({connectionString}) provided does not exists.");

            SetDatabase<T>(services, databaseType, cosmosConfigurationParams, connectionString);
        }

        public static void SetupDatabase<T>(this IServiceCollection services, IConfiguration configuration, string connectionStringName, DatabaseType databaseType, CosmosConfigurationParams cosmosConfigurationParams = null) where T : DbContext 
        {
            var applicationConnectionStrings = configuration.GetSection("ConnectionStrings").GetChildren();
            if (applicationConnectionStrings == null) throw new ArgumentException("There are no connection strings provided.");
            var connectionString = applicationConnectionStrings.FirstOrDefault(c => c.Key.ToLower() == connectionStringName.ToLower());
            if (connectionString == null || string.IsNullOrEmpty(connectionString.Value)) throw new ArgumentException($"The provided connection string ({connectionStringName}) provided does not exists.");

            SetDatabase<T>(services, databaseType, cosmosConfigurationParams, connectionString.Value);
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
                        }));
                    break;
                case DatabaseType.InMemory:
                    services.AddDbContext<T>(options => options.UseInMemoryDatabase(connectionString));
                    break;
                case DatabaseType.MySql:
                    services.AddDbContext<T>(options => options.UseMySql(connectionString, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                            errorNumbersToAdd: new List<int>());
                    }));
                    break;
                case DatabaseType.MariaDb:
                    services.AddDbContext<T>(options => options.UseMySql(connectionString, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                            errorNumbersToAdd: new List<int>());
                    }));
                    break;
                case DatabaseType.Sqlite:
                    services.AddDbContext<T>(options => options.UseSqlite(connectionString));
                    break;
                case DatabaseType.Cosmos:
                    if (cosmosConfigurationParams == null)
                        throw new ArgumentException($"The Cosmos configuration can not be null.");
                    services.AddDbContext<T>(options => options.UseCosmos(cosmosConfigurationParams.Endpoint,
                        cosmosConfigurationParams.Key, cosmosConfigurationParams.DatabaseName));
                    break;
                case DatabaseType.PostgreSQL:
                    services.AddDbContext<T>(options => options.UseNpgsql(connectionString, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                            errorCodesToAdd: new List<string>());
                    }));
                    break;
                case DatabaseType.FireBird:
                    services.AddDbContext<T>(options => options.UseFirebird(connectionString));
                    break;
                case DatabaseType.Oracle:
                    services.AddDbContext<T>(options => options.UseOracle(connectionString));
                    break;
                case DatabaseType.MSAccess:
                    services.AddDbContext<T>(options => options.UseJet(connectionString));
                    break;
                default:
                    throw new ArgumentException("Not provided a database driver");
            }
        }
    }
}
