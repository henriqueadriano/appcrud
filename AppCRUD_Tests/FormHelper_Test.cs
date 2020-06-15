using DataAccess;
using System.IO;
using System.Windows.Forms;
using Xunit;
using Helper;
using System.Drawing;
using Moq;
using log4net;

namespace AppCRUD_Tests
{
    public class FormHelper_Test
    {
        FormHelper helper;
        
        public FormHelper_Test()
        {
            //Initialyze component - Config
            var mockLog = new Mock<ILog>();
            helper = new FormHelper(mockLog.Object);
        }

        [Fact]
        public void FormHelper_CanResizeImage_Passing()
        {
            //Arrange
            OpenFileDialog opf = new OpenFileDialog();
            string imagePath = @"\ImageTest\lessThan400kb.jpg";
            opf.FileName = $"{Directory.GetCurrentDirectory()}{imagePath}";

            //Act
            var result = helper.CanResizeImage(opf);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void FormHelper_CanResizeImage_Fail()
        {
            //Arrange
            OpenFileDialog opf = new OpenFileDialog();
            string imagePath = @"\ImageTest\moreThan400kb.jpg";
            opf.FileName = $"{Directory.GetCurrentDirectory()}{imagePath}";

            //Act
            var result = helper.CanResizeImage(opf);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void FormHelper_FieldsValidationInsert_WithID()
        {
            //Arrange
            CrudModel model = new CrudModel
            {
                ID = 1,
                Name = "Randon Name",
                Image = new byte[0],
                Description = "Some description"
            };
            string withID = "Please Click on Clear Fields first\nThen load an image\nand fill the name field.";

            //Act
            var result = helper.FieldsValidationInsert(model);

            //Assert
            Assert.False(result);
            Assert.Equal(withID, helper.sbErrorMessage.ToString());
        }

        [Fact]
        public void FormHelper_FieldsValidationInsert_WithNoName()
        {
            //Arrange
            CrudModel model = new CrudModel
            {
                ID = null,
                Name = string.Empty,
                Image = new byte[0],
                Description = "Some description"
            };
            string noName = "Name should have value!\n";

            //Act
            var result = helper.FieldsValidationInsert(model);

            //Assert
            Assert.False(result);
            Assert.Equal(noName, helper. sbErrorMessage.ToString());
        }

        [Fact]
        public void FormHelper_FieldsValidationInsert_WithNoImage()
        {
            //Arrange
            CrudModel model = new CrudModel
            {
                ID = null,
                Name = "Some name",
                Image = null,
                Description = "Some description"
            };
            string noName = "You need to load an image!\n";

            //Act
            var result = helper.FieldsValidationInsert(model);

            //Assert
            Assert.False(result);
            Assert.Equal(noName, helper.sbErrorMessage.ToString());
        }

        [Fact]
        public void FormHelper_FieldsValidationInsert_Pass()
        {
            //Arrange
            CrudModel model = new CrudModel
            {
                ID = null,
                Name = "Some name",
                Image = new byte[0],
                Description = "Some description"
            };
            string noName = "";

            //Act
            var result = helper.FieldsValidationInsert(model);

            //Assert
            Assert.True(result);
            Assert.Equal(noName, helper.sbErrorMessage.ToString());
        }

        [Fact]
        public void FormHelper_VerifyImageNotNull_False()
        {
            //Arrange
            Image img = null;

            //Act
            var result = helper.VerifyImageNotNull(img);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void FormHelper_VerifyImageNotNull_True()
        {
            //Arrange
            string imagePath = @"\ImageTest\moreThan400kb.jpg";
            string imageFromFile = $"{Directory.GetCurrentDirectory()}{imagePath}";
            Image img = Image.FromFile(imageFromFile);

            //Act
            var result = helper.VerifyImageNotNull(img);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void FormHelper_FieldsValidationDelete_False()
        {
            //Arrange
            string ID = string.Empty;

            //Act
            var result = helper.FieldsValidationDelete(ID);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void FormHelper_FieldsValidationDelete_True()
        {
            //Arrange
            string ID = "1";

            //Act
            var result = helper.FieldsValidationDelete(ID);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void FormHelper_FieldsValidationUpdate_Fail()
        {
            //Arragen
            string noID = "Please Click on Clear Fields first\nThen choose a record from the Grid\nOr use the search button to find a record.\n";
            var model = new CrudModel
            {
                ID = null,
                Description = "aaa",
                Name = "xxx",
                Image = new byte[0]
            };

            //Act
            var result = helper.FieldsValidationUpdate(model);

            //Assert
            Assert.False(result);
            Assert.Contains(noID, helper.sbErrorMessage.ToString());
        }

        [Fact]
        public void FormHelper_FieldsValidationUpdate_Pass()
        {
            //Arrange
            var model = new CrudModel
            {
                ID = 1,
                Name = "Name",
                Image = new byte[0]
            };

            //Act
            var result = helper.FieldsValidationUpdate(model);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void FormHelper_FieldsValidationSearch_Pass()
        {
            //Arrange
            var searchByID_pass = new DropDownItem {Filter = "ID", Value = "1" };
            var searchByName_pass = new DropDownItem {Filter = "Name", Value = "some name" };
            var searchByDescription_pass = new DropDownItem {Filter = "Description", Value = "Some Description" };
            
            //Act
            var result_validID = helper.FieldsValidationSearch(searchByID_pass);
            var result_validName = helper.FieldsValidationSearch(searchByName_pass);
            var result_validDescription = helper.FieldsValidationSearch(searchByDescription_pass);

            //Assert
            Assert.True(result_validID);
            Assert.True(result_validName);
            Assert.True(result_validDescription);
        }

        [Fact]
        public void FormHelper_FieldsValidationSearch_ID()
        {
            //Arrange
            string no_ID = "Please inform the ID";
            string invalid_ID = "Please inform a valid number";
            var searchByID_fail_empty = new DropDownItem { Filter = "ID", Value = "" };
            var searchByID_fail_string = new DropDownItem { Filter = "ID", Value = "house" };
            
            //Act
            //Assert
            var result_invalidID_empty = helper.FieldsValidationSearch(searchByID_fail_empty);
            Assert.False(result_invalidID_empty);
            Assert.Equal(no_ID, helper.sbErrorMessage.ToString());
            helper.sbErrorMessage.Clear();

            //Act
            //Assert
            var result_invalidID_string = helper.FieldsValidationSearch(searchByID_fail_string);
            Assert.False(result_invalidID_string);
            Assert.Equal(invalid_ID, helper.sbErrorMessage.ToString());
            helper.sbErrorMessage.Clear();
        }

        [Fact]
        public void FormHelper_FieldsValidationSearch_Name()
        {
            //Arrange
            string emptyName = "Please inform the Name";
            var searchByNameEmpty = new DropDownItem { Filter = "Name", Value = "" };

            //Act
            var resultEmptyName = helper.FieldsValidationSearch(searchByNameEmpty);
            
            //Assert
            Assert.False(resultEmptyName);
            Assert.Equal(emptyName, helper.sbErrorMessage.ToString());
        }

        [Fact]
        public void FormHelper_FieldsValidationSearch_Description()
        {
            //Arrange
            string emptyDescription = "Please inform the Description";
            var searchByEmptyDescription = new DropDownItem { Filter = "Description", Value = "" };

            //Act
            var resultEmptyDescription = helper.FieldsValidationSearch(searchByEmptyDescription);

            //Assert
            Assert.False(resultEmptyDescription);
            Assert.Equal(emptyDescription, helper.sbErrorMessage.ToString());
        }
    }
}
