using Npgsql;
using System;
using System.Collections.Generic;

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

        public List<DataBaseModel> GetDbServerModels()
        {
            var models = new List<DataBaseModel>();

            using var con = new NpgsqlConnection(_connectionString);
            con.Open();

            var sql = "SELECT pg_database.datname as \"database_name\", " +
                      "pg_database_size(pg_database.datname) as size " +
                      "FROM pg_database";

            using var cmd = new NpgsqlCommand(sql, con);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var size = Int32.Parse(reader[1].ToString());
                    var sizeGb = Math.Round(size * 1e-9, 2, MidpointRounding.AwayFromZero);
                    var model = new DataBaseModel
                    { DataBaseName = reader[0].ToString(), Size = sizeGb, UpdateTime = DateTime.UtcNow.Date };

                    models.Add(model);
                }
            }

            return models;
        }
    }
}
