using DataAccess;
using DataAccess.Models;
using Helper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using Squirrel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AppCRUD
{
    public partial class Form1 : Form
    {
        #region Global Variables

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        System.Drawing.Font myFont;
        private const string UPDATE_FOLDER_PATH = @"C:\Temp\Releases";
        private const string DATE_FORMAT = "yyyy-MM-dd";

        //Dependency Injection
        private readonly IDataBaseService _database; 
        private readonly IFormHelper _formHelper;
        private readonly ILog _log;

        private const string INSERT_SUCCESS = "Model Created Successiful.";
        private const string DELETE_SUCCESS = "Model Deleted Successiful.";
        private const string UPDATE_SUCCESS = "Model Updated Successiful.";
        private const string SEARCH_NO_RESULT = "No results!";
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
            //TODO Fix the Auto-Update using Squirrel library
            //UpdateApp();
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
            pictureBox1.Image = System.Drawing.Image.FromStream(ms2);

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
                    pictureBox1.Image = System.Drawing.Image.FromFile(opf.FileName);
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

        private void tabControl1_Click(object sender, EventArgs e)
        {
            LodGridHistory();
        }

        private void btn_search_report_Click(object sender, EventArgs e)
        {
            var dateStart = dateTimePicker1.Value;
            var dateEnd = dateTimePicker2.Value;

            if (_formHelper.FieldsValidationReportSearch(dateStart, dateEnd))
            {
                var result = _database.FindReportByDate(dateStart.ToString(DATE_FORMAT), dateEnd.ToString(DATE_FORMAT));
                if (result.Count == 0)
                    MessageBox.Show(SEARCH_NO_RESULT);
                else
                    dataGridView2.DataSource = result;
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

        private void btn_export_pdf_Click(object sender, EventArgs e)
        {
            ExportToPdf();
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
            myFont = new System.Drawing.Font(fonts.Families[0], 11.0F);
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

        private void ExportToPdf()
        {
            try
            {
                var pdfDoc = new Document(PageSize.LETTER, 40f, 40f, 60f, 60f);
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "Save file as...";
                dialog.Filter = "Pdf Files|*.pdf";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PdfWriter.GetInstance(pdfDoc, new FileStream(dialog.FileName, FileMode.OpenOrCreate));
                    pdfDoc.Open();

                    var image = Properties.Resources.AppCRUD_logo;
                    var png = iTextSharp.text.Image.GetInstance(image, ImageFormat.Png);
                    png.ScalePercent(40f);
                    png.SetAbsolutePosition(pdfDoc.Left, pdfDoc.Top);
                    pdfDoc.Add(png);

                    var spacer = new Paragraph("")
                    {
                        SpacingBefore = 10f,
                        SpacingAfter = 10f,
                    };
                    pdfDoc.Add(spacer);

                    var headerTable = new PdfPTable(new[] { .75f, 2f })
                    {
                        HorizontalAlignment = Left,
                        WidthPercentage = 75,
                        DefaultCell = { MinimumHeight = 22f }
                    };

                    headerTable.AddCell("Date");
                    headerTable.AddCell(DateTime.Now.ToString());
                    headerTable.AddCell("Name");
                    headerTable.AddCell("Henrique");
                    headerTable.AddCell("Project Number");
                    headerTable.AddCell("80000");

                    pdfDoc.Add(headerTable);
                    pdfDoc.Add(spacer);

                    var columnCount = dataGridView2.ColumnCount;
                    var columnWidths = new[] { 2f, 2f };

                    var table = new PdfPTable(columnWidths)
                    {
                        HorizontalAlignment = Left,
                        WidthPercentage = 100,
                        DefaultCell = { MinimumHeight = 22f }
                    };

                    var cell = new PdfPCell(new Phrase("User Report Actictons"))
                    {
                        Colspan = columnCount,
                        HorizontalAlignment = 1,  //0=Left, 1=Centre, 2=Right
                        MinimumHeight = 25f
                    };

                    table.AddCell(cell);

                    dataGridView2.Columns
                        .OfType<DataGridViewColumn>()
                        .ToList()
                        .ForEach(c => table.AddCell(c.Name));

                    dataGridView2.Rows
                        .OfType<DataGridViewRow>()
                        .ToList()
                        .ForEach(r =>
                        {
                            var cells = r.Cells.OfType<DataGridViewCell>().ToList();
                            cells.ForEach(c => table.AddCell(c.Value.ToString()));
                        });

                    pdfDoc.Add(table);

                    pdfDoc.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        #endregion

        #region Update AppCRUD
        private void AddVersionNumber()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Text += $" version {versionInfo.FileVersion} ";
        }
        
        private void UpdateApp()
        {
            string subPath = UPDATE_FOLDER_PATH;
            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);
            using (var manager = new UpdateManager(subPath))
            {
                manager.UpdateApp();
            }
        }
        
        #endregion
    }

}
