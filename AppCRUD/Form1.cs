using DataAccess;
using DataAccess.Models;
using Helper;
using log4net;
using Squirrel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppCRUD
{
    public partial class Form1 : Form
    {
        #region Global Variables

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font myFont;
        private const string UPDATE_FOLDER_PATH = @"C:\Temp\Releases";

        //Dependency Injection
        private readonly IDataBaseService _database; 
        private readonly IFormHelper _formHelper;
        private readonly ILog _log;

        private const string INSERT_SUCCESS = "Model Created Successiful.";
        private const string DELETE_SUCCESS = "Model Deleted Successiful.";
        private const string UPDATE_SUCCESS = "Model Updated Successiful.";
        #endregion

        public Form1(IDataBaseService database, IFormHelper formHelper, ILog log)
        {
            SqliteDataAccess.CreateSQLiteDB();

            #region Dependency Injection
            _database = database; 
            _formHelper = formHelper;
            _log = log;
            #endregion

            #pragma warning disable
            UpdateApp();
            #pragma warning restore
            
            InitializeComponent();
            AddVersionNumber();
            FontConfig();
            SetCustomizedFont(this);
            TextBoxSetMaxLenght();
            LoadDropBoxSearchFilter();
            LoadGridView();
        }

        #region Event Handlers
        private void dataGridView1_Click(object sender, EventArgs e)
        {
            byte[] img2 = (byte[])dataGridView1.CurrentRow.Cells[3].Value;
            MemoryStream ms2 = new MemoryStream(img2);
            pictureBox1.Image = Image.FromStream(ms2);

            textBox_ID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBox_Name.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox_description.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
        }
        
        private void btn_add_image_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";

            if (opf.ShowDialog() == DialogResult.OK)
                if (_formHelper.CanResizeImage(opf))
                {
                    pictureBox1.Image = Image.FromFile(opf.FileName);
                }
                else
                {
                    MessageBox.Show(
                  "Image can not be bigger than 400Kb!\nPlease choose an image not bigger than 400Kb.",
                  "Image size error.",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Exclamation
                  );
                }
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
        }
        
        private void btn_insert_Click(object sender, EventArgs e)
        {
            int? number = null;
            CrudModel model = new CrudModel
            {
                ID = textBox_ID.Text == string.Empty ? number : int.Parse(textBox_ID.Text),
                Name = textBox_Name.Text,
                Description = textBox_description.Text,
                Image = GetImageFromView()
            };

            if (_formHelper.FieldsValidationInsert(model))
            {
                int result = _database.AddModel(model);
                ResultOperation(result, INSERT_SUCCESS);
            }
            else
                MessageBox.Show(
                    _formHelper.sbErrorMessage.ToString(),
                    "Validation Fields",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                    );
            _formHelper.sbErrorMessage.Clear();
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
        }
        
        private void btn_update_Click(object sender, EventArgs e)
        {
            int? number = null;
            CrudModel crudModel = new CrudModel
            {
                ID = textBox_ID.Text == string.Empty ? number : int.Parse(textBox_ID.Text),
                Name = textBox_Name.Text,
                Description = textBox_description.Text,
                Image = GetImageFromView()
            };

            if (_formHelper.FieldsValidationUpdate(crudModel))
            {
                int result = _database.UpdateModel(crudModel);
                ResultOperation(result, UPDATE_SUCCESS);
            }
            else
                MessageBox.Show(
                    _formHelper.sbErrorMessage.ToString(),
                    "Validation Fields",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                    );
            _formHelper.sbErrorMessage.Clear();
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
        }
        
        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (_formHelper.FieldsValidationDelete(textBox_ID.Text))
            {
                CrudModel crudModel = new CrudModel { ID = int.Parse(textBox_ID.Text) };
                int result = _database.DeleteModel(crudModel);
                ResultOperation(result, DELETE_SUCCESS);
            }
            else
                MessageBox.Show(
                    _formHelper.sbErrorMessage.ToString(),
                    "Validation Fields",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                    );
            _formHelper.sbErrorMessage.Clear();
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
        }
        
        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearFields();
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
        }
        
        private void btn_search_Click(object sender, EventArgs e)
        {
            var itemSearch = new DropDownItem
            {
                Filter = cb_search.SelectedItem.ToString(),
                Value = textBox_Search.Text
            };
            if (_formHelper.FieldsValidationSearch(itemSearch))
            {
                dataGridView1.DataSource = _database.FindByOption(itemSearch);
                PopulateData();
            }
            else
                MessageBox.Show(
                    _formHelper.sbErrorMessage.ToString(),
                    "Validation Fields",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                    );
            _formHelper.sbErrorMessage.Clear();
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
        }
        
        private void btn_load_grid_Click(object sender, EventArgs e)
        {
            LoadGridView();
            _log.Info(new LogDetails().SetLogClass(this.GetType().Name).SetLogMethod(LogDetails.GetCurrentMethod()));
        }
        #endregion

        #region Helpers
        public CrudModel CreateModel() =>
            new CrudModel
            {
                Name = textBox_Name.Text,
                Description = textBox_description.Text,
                Image = GetImageFromView()
            };
        
        public void LoadGridView()
        {
            dataGridView1.DataSource = _database.LoadData();
            PopulateData();
        }
        
        private void PopulateData()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.Columns[3].FillWeight = 15;
            dataGridView1.Columns[0].FillWeight = 20;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewImageColumn imgCol = (DataGridViewImageColumn)dataGridView1.Columns[3];
            imgCol.ImageLayout = DataGridViewImageCellLayout.Stretch;
        }

        private void LodGridHistory()
        {
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.RowTemplate.Height = 40;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.DataSource = _database.LoadReport();
        }
        
        private void ResultOperation(int result, string message)
        {
            if (result == 1)
            {
                LoadGridView();
                ClearFields();
                MessageBox.Show(message);
            }
            else MessageBox.Show("Query not executed");
        }
        
        private void LoadDropBoxSearchFilter()
        {
            string[] filters = { "ID", "Name", "Description" };
            cb_search.Items.AddRange(filters);
            cb_search.SelectedIndex = 0;
        }
        
        private void ClearFields()
        {
            textBox_ID.Text = string.Empty;
            textBox_Name.Text = string.Empty;
            textBox_description.Text = string.Empty;
            pictureBox1.Image = null;
            textBox_Search.Text = string.Empty;
        }
        
        private byte[] GetImageFromView()
        {
            if (_formHelper.VerifyImageNotNull(pictureBox1.Image))
            {
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                return ms.ToArray();
            }
            else return null;
        }
        
        private void TextBoxSetMaxLenght()
        {
            textBox_Search.MaxLength = 50;
            textBox_Name.MaxLength = 200;
            textBox_description.MaxLength = 1000;
        }
        
        private void FontConfig()
        {
            byte[] fontData = Properties.Resources.ARCADECLASSIC;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.ARCADECLASSIC.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.ARCADECLASSIC.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            myFont = new Font(fonts.Families[0], 11.0F);
        }
        
        public void SetCustomizedFont(Control control)
        {
            btn_search.Font = myFont;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = myFont;

            groupBox1.Font = myFont;
            label1.Font = myFont;
            label2.Font = myFont;
            label3.Font = myFont;
            btn_delete.Font = myFont;
            btn_insert.Font = myFont;
            btn_load_grid.Font = myFont;
            btn_clear.Font = myFont;
            btn_update.Font = myFont;
            btn_add_image.Font = myFont;

        }
        #endregion

        #region Update AppCRUD
        private void AddVersionNumber()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Text += $" version {versionInfo.FileVersion} ";
        }
        
        private static async Task UpdateApp()
        {
            string subPath = UPDATE_FOLDER_PATH;
            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);
            using (var manager = new UpdateManager(subPath))
            {
                await manager.UpdateApp();
            }
        }
        #endregion

        private void tabControl1_Click(object sender, EventArgs e)
        {
            LodGridHistory();
        }
    }

}
