using Dapper;
using System.Data;
using Npgsql;

namespace DapperMvcApp.Models
{
    public interface IUserRepository
    {
        void Create<T>(T entry);
        void Delete<T>(Guid uuid);
        T Get<T>(Guid uuid) where T : notnull;
        void Update<T>(Guid uuid, T entry);
    }
    public class UserRepository : IUserRepository
    {
        string connectionString;
        public UserRepository(string conn = "Host=localhost;Database=test_task_wpf;Username=postgres;Password=postgres")
        {
            connectionString = conn;
        }

        public T Get<T>(Guid uuid) where T : notnull
        {
            string tableName = typeof(T).Name.ToLower();
            string query = $"SELECT * FROM {tableName} WHERE uuid = @uuid";

            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                return db.Query<T>(query, uuid).FirstOrDefault();
            }
        }

        public List<T> All<T>() where T : notnull
        {
            string tableName = typeof(T).Name.ToLower();
            string query = $"SELECT * FROM {tableName}";

            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                return db.Query<T>(query).ToList();
            }
        }

        public void Create<T>(T entry)
        {
            string tableName = typeof(T).Name.ToLower();

            string columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));
            string values = string.Join(", ", typeof(T).GetProperties().Select(p => $"@{p.Name}"));
            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                db.Execute(query, entry);
            }
        }

        public void Update<T>(Guid uuid, T entry)
        {
            string tableName = typeof(T).Name.ToLower();

            string setClause = string.Join(", ", typeof(T).GetProperties().Select(p => $"{p.Name} = @{p.GetValue(p)}"));
            string query = $"UPDATE {tableName} SET {setClause} WHERE uuid = @uuid";

            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                typeof(T).GetProperty("Uuid").SetValue(entry, uuid);

                db.Execute(query, entry);
            }
        }

        public void Delete<T>(Guid uuid)
        {
            string tableName = typeof(T).Name.ToLower();
            string query = $"DELETE FROM {tableName} WHERE uuid = @uuid";

            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                db.Execute(query, uuid);
            }
        }
    }
}