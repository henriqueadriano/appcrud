using DataAccess.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataAccess
{
    public class DataBaseService : IDataBaseService
    {
        private static string List = File.ReadAllText(@"SQL_Commands\List_Sqlite.sql");
        private static string Insert = File.ReadAllText(@"SQL_Commands\Insert_Sqlite.sql");
        private static string Update = File.ReadAllText(@"SQL_Commands\Update_Sqlite.sql");
        private static string Delete = File.ReadAllText(@"SQL_Commands\Delete_Sqlite.sql");
        private static string FindByFilter = File.ReadAllText(@"SQL_Commands\FindByFilter_Sqlite.sql");
        private static string FindByDate = File.ReadAllText(@"SQL_Commands\Report_Find_Log_Sqlite.sql");
        private static string Report = File.ReadAllText(@"SQL_Commands\Report.sql");

        private readonly ISqliteDataAccess _database;
        private readonly ILog _log;
        
        public DataBaseService(ISqliteDataAccess database, ILog log)
        {
            _database = database;
            _log = log;
        }
        public List<CrudModel> LoadData()
        {
            try
            {
                var output = _database.LoadData<CrudModel>(List);
                _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
                return output;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new List<CrudModel>();
            }
        }
        public int AddModel(CrudModel crudModel)
        {
            try
            {
                var output = _database.AddModel(crudModel, Insert);
                _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
                return output;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return 0;
            }
        }
        public int UpdateModel(CrudModel crudModel)
        {
            try
            {
                var output = _database.UpdateModel(crudModel, Update);
                _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
                return output;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return 0;
            }
        }
        public int DeleteModel(CrudModel crudModel)
        {
            try
            {
                var output = _database.UpdateModel(crudModel, Delete);
                _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
                return output;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return 0;
            }
        }
        public List<CrudModel> FindByOption(DropDownItem option)
        {
            try
            {
                _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
                string sql;
                switch (option.Filter)
                {
                    case "ID":
                        {
                            sql = FindByFilter.Replace("@Column", "ID").Replace("@Value", $"{option.Value}");
                            return _database.FindByOption<CrudModel>(sql);
                        }
                    case "Name":
                        {
                            sql = FindByFilter.Replace("@Column", "Name").Replace("@Value", $"'%{option.Value}%'");
                            var output = _database.FindByOption<CrudModel>(sql);
                            return output;
                        }
                    case "Description":
                        {
                            sql = FindByFilter.Replace("@Column", "Description").Replace("@Value", $"'%{option.Value}%'");
                            return _database.FindByOption<CrudModel>(sql);
                        }
                    default:
                        return new List<CrudModel>();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new List<CrudModel>();
            }
        }

        public List<Report> LoadReport()
        {
            try
            {
                var output = _database.LoadReport<Report>(Report);
                _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
                return output;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new List<Report>();
            }
        }

        public List<Report> FindReportByDate(string dateStart, string dateEnd)
        {
            try
            {
                _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
                string sql;
                sql = FindByDate.Replace("@DateStart", dateStart).Replace("@DateEnd", dateEnd);
                return _database.FindReportByDate<Report>(sql);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new List<Report>();
            }
        }
    }
}
