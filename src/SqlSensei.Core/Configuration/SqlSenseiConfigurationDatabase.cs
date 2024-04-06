using System;

namespace SqlSensei.Core
{
    public class SqlSenseiConfigurationDatabase
    {
        public SqlSenseiConfigurationDatabase(string database, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(database))
            {
                throw new ArgumentException("Database name cannot be null or whitespace.", nameof(database));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Invalid connection string.", nameof(connectionString));
            }

            Database = database;
            ConnectionString = connectionString;
        }

        public string Database { get; }
        public string ConnectionString { get; }
    }
}