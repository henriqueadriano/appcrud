using System.Collections.Generic;

namespace DataAccess
{
    public interface ISqliteDataAccess
    {
        List<T> LoadData<T>(string sql);
        int AddModel<T>(T crudModel, string sql);
        int UpdateModel<T>(T crudModel, string sql);
        int DeleteModel<T>(T crudModel, string sql);
        List<T> FindByOption<T>(string sql);
        List<T> LoadReport<T>(string sql);
        List<T> FindReportByDate<T>(string sql);
    }
}
