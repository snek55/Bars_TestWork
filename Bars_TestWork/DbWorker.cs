using System;
using System.Collections.Generic;
using Npgsql;

namespace Bars_TestWork
{
    public class DbWorker
    {
        private readonly string _connectionString;

        public DbWorker(string connectionString)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
                _connectionString = connectionString;
        }

        public Dictionary<string, double> GetSizes()
        {
            var dictionary = new Dictionary<string, double>();

            using (var con = new NpgsqlConnection(_connectionString))
            {
                var sql = "SELECT pg_database.datname as \"database_name\", " +
                          "pg_database_size(pg_database.datname) as size " +
                          "FROM pg_database";

                using var cmd = new NpgsqlCommand(sql, con);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var size = Int32.Parse(reader[1].ToString());
                        var sizeGb = size * 1e-9;
                        dictionary.Add(reader[0].ToString(), sizeGb);
                    }
                }
            }

            return dictionary;
        }
    }
}
