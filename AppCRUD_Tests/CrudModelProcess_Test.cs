using DataAccess;
using log4net;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AppCRUD_Tests
{
    public class CrudModelProcess_Test
    {
        //Config
        Mock<ISqliteDataAccess> datasource;
        Mock<ILog> mockLog;

        [Fact]
        public void LoadData_Pass()
        {
            //Arrange
            datasource = new Mock<ISqliteDataAccess>();
            datasource.Setup(m => m.LoadData<CrudModel>(It.IsAny<string>()))
            .Returns(getList());

            //Act
            mockLog = new Mock<ILog>();
            var process = new DataBaseService(datasource.Object, mockLog.Object);
            var expected = getList();
            var actual = process.LoadData();

            //Assert
            Assert.True(actual != null);
            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].ID, actual[i].ID);
                Assert.Equal(expected[i].Name, actual[i].Name);
                Assert.Equal(expected[i].Description, actual[i].Description);
                Assert.Equal(expected[i].Image, actual[i].Image);
            }
        }

        [Fact]
        public void AddModel_Pass()
        {
            //Arrange
            datasource = new Mock<ISqliteDataAccess>();
            datasource
                .Setup(m => m.AddModel<CrudModel>(GetModel(), It.IsAny<string>()))
                .Returns(int.MinValue);

            //Act
            mockLog = new Mock<ILog>();
            var process = new DataBaseService(datasource.Object, mockLog.Object);
            var expected = 0;
            var actual = process.AddModel(GetModel());

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateModel_Pass()
        {
            //Arrange
            var model = GetModel();
            model.ID = 1;
            datasource = new Mock<ISqliteDataAccess>();
            datasource
                .Setup(m => m.UpdateModel<CrudModel>(model, It.IsAny<string>()))
                .Returns(1);

            //Act
            mockLog = new Mock<ILog>();
            var process = new DataBaseService(datasource.Object, mockLog.Object);
            var actual = process.UpdateModel(model);
            var expected = 1;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DeleteModel_Pass()
        {
            //Arrange
            var model = GetModel();
            model.ID = 0;
            datasource = new Mock<ISqliteDataAccess>();
            datasource
                .Setup(m => m.DeleteModel<CrudModel>(model, It.IsAny<string>()))
                .Returns(0);
            //Act
            mockLog = new Mock<ILog>();
            var process = new DataBaseService(datasource.Object, mockLog.Object);
            var actual = process.DeleteModel(model);
            var expected = 0;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindByOption_Pass()
        {
            //Arrange
            var filterByID = new DropDownItem { Filter = "ID", Value = "1" };
            var filterByName = new DropDownItem { Filter = "Name", Value = "Henrique" };
            var filterByDescription = new DropDownItem { Filter = "Description", Value = "Description" };
            
            datasource = new Mock<ISqliteDataAccess>();
            datasource
                .Setup(m => m.FindByOption<CrudModel>(It.IsAny<string>()))
                .Returns(getList());

            //Act
            mockLog = new Mock<ILog>();
            var process = new DataBaseService(datasource.Object, mockLog.Object);
            var actual_ID = process.FindByOption(filterByID);
            var actual_Name = process.FindByOption(filterByName);
            var actual_Description = process.FindByOption(filterByDescription);
            var expected = getList();

            //Assert
            Assert.Equal(expected.Count, actual_ID.Count);
            Assert.Equal(expected.Count, actual_Name.Count);
            Assert.Equal(expected.Count, actual_Description.Count);
        }

        #region Helpers
        private List<CrudModel> getList()
        {
            return new List<CrudModel> {
                new CrudModel { ID = 1,Name="Henrique",Image = new byte[0]},
                new CrudModel { ID = 2,Name="Kelvia",Image = new byte[0]},
                new CrudModel { ID = 3,Name="Olivia",Image = new byte[0]}
            };
        }
        private CrudModel GetModel()
        {
            return new CrudModel { ID = null, Name = "Sonic 2", Description = "Video Game", Image = new byte[0] };
        }
        #endregion
    }
}
