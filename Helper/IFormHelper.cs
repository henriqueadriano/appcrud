using DataAccess;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace Helper
{
    public interface IFormHelper
    {
        StringBuilder sbErrorMessage { get; }
        bool CanResizeImage(OpenFileDialog opf);
        bool FieldsValidationDelete(string ID);
        bool FieldsValidationInsert(CrudModel model);
        bool FieldsValidationUpdate(CrudModel model);
        bool VerifyImageNotNull(Image image);
        bool FieldsValidationSearch(DropDownItem itemSearch);
    }
}