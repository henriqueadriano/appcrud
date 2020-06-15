using DataAccess.Models;
using System.Collections.Generic;

namespace DataAccess
{
    public interface IDataBaseService
    {
        int AddModel(CrudModel crudModel);
        List<CrudModel> LoadData();
        int UpdateModel(CrudModel crudModel);
        int DeleteModel(CrudModel crudModel);
        List<CrudModel> FindByOption(DropDownItem option);
        List<Report> LoadReport();
    }
}