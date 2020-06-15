using System;

namespace DataAccess.Models
{
    public class Report
    {
        private string _message { get; set; }
        public DateTime Date { get; set; }
        public string Message {
            get { return _message; }
            set
            {
                string userAction = value.Split(';')[1].Split(':')[1];
                switch (userAction)
                {
                    case "btn_insert_Click":
                        {
                            _message = "User added new item.";
                            break;
                        }
                    case "btn_add_image_Click":
                        {
                            _message = "User added new image.";
                            break;
                        }
                    case "btn_delete_Click":
                        {
                            _message = "User deleted new item.";
                            break;
                        }
                    case "btn_update_Click":
                        {
                            _message = "User updated new item.";
                            break;
                        }
                    case "dataGridView1_Click":
                        {
                            _message = "Grid List was updated.";
                            break;
                        }
                    case "btn_load_grid_Click":
                        {
                            _message = "User click to update list.";
                            break;
                        }
                    case "btn_search_Click":
                        {
                            _message = "User searched for item.";
                            break;
                        }
                    case "btn_clear_Click":
                        {
                            _message = "User clear grid.";
                            break;
                        }
                    default:
                        _message = string.Empty;
                        break;
                };
            } 
        }
    }
}

