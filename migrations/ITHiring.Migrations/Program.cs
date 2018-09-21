using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ITHiring.Migrations
{
    class Program
    {
        
            
        public static void TryCreateDatabase(ILogger logger, string connectionString)
        {
            try
            {
                logger.LogInformation("TRYING CREATE DATABASE");
                var bld = new NpgsqlConnectionStringBuilder(connectionString);
                var database = bld.Database;
                var userName = bld.Username;
                bld.Database = "postgres";
                var serverOnly = bld.ToString();
                using (var conn = new NpgsqlConnection(serverOnly))
                {
                    conn.Open();

                    bool exists;
                    using (var existsCmd =
                        new NpgsqlCommand($@"SELECT count(*) from pg_database WHERE datname='{database}'", conn))
                    {
                        exists = existsCmd.ExecuteScalar()?.ToString() != "0";
                    }

                    if (!exists)
                    {
                        using (var cmd = new NpgsqlCommand($@"
                            CREATE DATABASE ""{database}""
                                WITH OWNER = {userName}
                                ENCODING = 'UTF8'
                                TABLESPACE = pg_default
                                LC_COLLATE = 'ru_RU.utf8'
                                LC_CTYPE = 'ru_RU.utf8'
                                CONNECTION LIMIT = -1; ",
                            conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        logger.LogInformation("Database already exists. Skipping creation.");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
        }


        static void Main(string[] args)
        {
            var cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            var postgres = cfg["postgres"];

            var container = new ServiceCollection()
                .AddLogging(log => log.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                {
                    rb
                        .AddPostgres()
                        .WithGlobalConnectionString(postgres)
                        .ScanIn(typeof(Program).Assembly);
                })
                .BuildServiceProvider();

            using (var scope = container.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var log = provider.GetService<ILoggerFactory>().CreateLogger("");
                TryCreateDatabase(log, postgres);
                provider.GetRequiredService<IMigrationRunner>().MigrateUp();
            }

            Console.ReadKey();
        }
    }
}
