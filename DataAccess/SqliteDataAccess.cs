using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace DataAccess
{
    public class SqliteDataAccess : ISqliteDataAccess
    {
        private static string CreateTable_TB_CRUD = File.ReadAllText(@"SQL_Commands\CreateTable_TB_CRUD.sql");
        private static string CreateTable_LOG = File.ReadAllText(@"SQL_Commands\CreateTable_Log.sql");
        private static string DB_APPCRUD = "AppCRUD.db";

        public static string LoadConnStrAppCRUD(string id = "AppCRUD") => ConfigurationManager.ConnectionStrings[id].ConnectionString;
        public static string LoadConnStrLog(string id = "Log") => ConfigurationManager.ConnectionStrings[id].ConnectionString;
        public static void CreateSQLiteDB()
        {
            if (!File.Exists(DB_APPCRUD))
            {
                CreateDBAppCRUD();
                //First time run Log.db is already created by log4net framework.
                CreateDBLog();
            }
        }
        private static void CreateDBAppCRUD()
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(LoadConnStrAppCRUD()))
            {
                sqlite_conn.Open();
                SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = CreateTable_TB_CRUD;
                sqlite_cmd.ExecuteNonQuery();
                sqlite_conn.Close();
            }
        }

        private static void CreateDBLog()
        {
            using (SQLiteConnection sqlite_conn = new SQLiteConnection(LoadConnStrLog()))
            {
                sqlite_conn.Open();
                SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = CreateTable_LOG;
                sqlite_cmd.ExecuteNonQuery();
                sqlite_conn.Close();
            }
        }

        public List<T> LoadData<T>(string sql)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnStrAppCRUD()))
            {
                var output = cnn.Query<T>(sql, new DynamicParameters());
                return output.ToList();
            }
        }
        public int AddModel<T>(T crudModel, string sql)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnStrAppCRUD()))
            {
                return cnn.Execute(sql, crudModel);
            }
        }
        public int UpdateModel<T>(T crudModel, string sql)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnStrAppCRUD()))
            {
                return cnn.Execute(sql, crudModel);
            }
        }
        public int DeleteModel<T>(T crudModel, string sql)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnStrAppCRUD()))
            {
                return cnn.Execute(sql, crudModel);
            }
        }
        public List<T> FindByOption<T>(string sql)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnStrAppCRUD()))
            {
                 return cnn.Query<T>(sql, new DynamicParameters()).ToList();
            }
        }

        public List<T> LoadReport<T>(string sql)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnStrLog()))
            {
                var result = cnn.Query<T>(sql, new DynamicParameters());
                return result.ToList();
            }
        }
    }
}
