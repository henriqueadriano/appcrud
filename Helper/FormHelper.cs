using DataAccess;
using DataAccess.Models;
using log4net;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Helper
{
    public class FormHelper : IFormHelper
    {
        private StringBuilder _sbErrorMessage;
        private readonly ILog _log;
        public FormHelper(ILog log) =>_log = log;
        
        public StringBuilder sbErrorMessage { get => _sbErrorMessage; }
        public bool CanResizeImage(OpenFileDialog opf)
        {
            byte[] bufferImageFile = File.ReadAllBytes(opf.FileName);
            int imageSize = bufferImageFile.Length;

            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));

            if (imageSize > 400000)
                return false;
            else
                return true;
        }
        public bool FieldsValidationInsert(CrudModel model)
        {
            bool validation = true;
            _sbErrorMessage = CreateStringBuilder();

            if (model.ID != null)
            { _sbErrorMessage.Append("Please Click on Clear Fields first\nThen load an image\nand fill the name field."); validation = false; }

            if (model.Name == string.Empty)
            { _sbErrorMessage.Append("Name should have value!\n"); validation = false; }

            if (model.Image == null)
            { _sbErrorMessage.Append("You need to load an image!\n"); validation = false; }

            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));

            return validation;
        }
        public bool FieldsValidationDelete(string ID)
        {
            bool validation = true;
            _sbErrorMessage = CreateStringBuilder();

            if (ID == string.Empty)
            { _sbErrorMessage.Append("Please Click on Clear Fields first\nThen choose a record from the Grid\nOr use the search button to find a record.\n"); validation = false; }
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
            return validation;
        }
        public bool FieldsValidationUpdate(CrudModel model)
        {
            bool validation = true;
            _sbErrorMessage = _sbErrorMessage == null ? new StringBuilder() : _sbErrorMessage;
            if (model.ID == null)
            { _sbErrorMessage.Append("Please Click on Clear Fields first\nThen choose a record from the Grid\nOr use the search button to find a record.\n"); validation = false; }
            if (model.Name == string.Empty)
            { _sbErrorMessage.Append("Name should have value!\n"); validation = false; }
            if (model.Image == null)
            { _sbErrorMessage.Append("You need to load an image!\n"); validation = false; }
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
            return validation;
        }
        public bool VerifyImageNotNull(Image image)
        {
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
            if (image == null) return false;
            else return true;
        }
        public bool FieldsValidationSearch(DropDownItem itemSearch)
        {
            _sbErrorMessage = CreateStringBuilder();
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
            try
            {
                switch (itemSearch.Filter)
                {
                    case "ID":
                        {
                            int number = 0;
                            if (string.IsNullOrEmpty(itemSearch.Value))
                            { _sbErrorMessage.Append("Please inform the ID"); throw new Exception(); }
                            else if (!int.TryParse(itemSearch.Value, out number))
                            { _sbErrorMessage.Append("Please inform a valid number"); return false; }
                            else { return true; }
                        }
                    case "Name":
                        {
                            if (string.IsNullOrEmpty(itemSearch.Value))
                            { _sbErrorMessage.Append("Please inform the Name"); return false; }
                            else { return true; }
                        }
                    case "Description":
                        {
                            if (string.IsNullOrEmpty(itemSearch.Value))
                            { _sbErrorMessage.Append("Please inform the Description"); return false; }
                            else { return true; }
                        }
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return false;
            }
        }

        public bool FieldsValidationReportSearch(DateTime dateStart, DateTime dateEnd)
        {
            bool validation = true;
            _sbErrorMessage = _sbErrorMessage == null ? new StringBuilder() : _sbErrorMessage;
            
            if (dateStart.Date > DateTime.Now)
            { 
                _sbErrorMessage.Append("Please choose a start date not bigger then today.\n"); validation = false; 
            }
            if (dateEnd.Date > DateTime.Now)
            {
                _sbErrorMessage.Append("Please choose a end date not bigger then today.\n"); validation = false;
            }
            if (dateStart.Date > dateEnd.Date)
            {
                _sbErrorMessage.Append("Please choose a end date not bigger then start date."); validation = false;
            }
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
            return validation;
        }
        private StringBuilder CreateStringBuilder() => _sbErrorMessage is null ? new StringBuilder() : _sbErrorMessage;
    }
}