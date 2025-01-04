using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Data;
using System.Data;
using System.Data.Common;
using System.Text.Json.Serialization;
using System.Text;

namespace DeThiCNDVWebSerivce.Controller
{
    [ApiController]
    public class MainController : ControllerBase
    {
        SqlConnection con;

        private void ConnectDatabase()
        {
            String connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\clock\\Usage\\DeThiCNDVWeb\\DeThiCNDVWeb\\Database\\Database1.mdf;Integrated Security=True";

            try
            {
                con = new SqlConnection(connectionString);
                con.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DisconnectDatabase()
        {
            try
            {
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private DataTable getItemData()
        {
            ConnectDatabase();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM mathang";
            DataTable matHangTable = new DataTable();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(matHangTable);

            DisconnectDatabase();

            return matHangTable;
        }

        private DataTable getSupData()
        {
            ConnectDatabase();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM nhacungcap";
            DataTable supTable = new DataTable();

            SqlDataAdapter da = new SqlDataAdapter( cmd);
            da.Fill(supTable);

            DisconnectDatabase() ;
            return supTable;
        }

        private void insertItemData(string mamh, string tenmh, string dvt, DateTime ngaynhap, int dongia, int soluong, byte[] image, string mancc)
        {
            try
            {
                ConnectDatabase();

                SqlCommand command = new SqlCommand("INSERT INTO mathang(mamh, tenmh, dvt, ngaynhap, dongia, soluong, hinhanh, mancc) VALUES(@mamh, @tenmh, @dvt, @ngaynhap, @dongia, @soluong, @hinhanh, @mancc)", con);
                command.Parameters.AddWithValue("@mamh", mamh);
                command.Parameters.AddWithValue("@tenmh", tenmh);
                command.Parameters.AddWithValue("@dvt", dvt);
                command.Parameters.AddWithValue("@ngaynhap", ngaynhap);
                command.Parameters.AddWithValue("@dongia", dongia);
                command.Parameters.AddWithValue("@soluong", soluong);
                command.Parameters.Add("@hinhanh", SqlDbType.Image).Value = image;
                command.Parameters.AddWithValue("@mancc", mancc);

                command.ExecuteNonQuery();

                DisconnectDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine("insert Error: " + ex.Message);
            }
        }

        [HttpGet("/")]
        public String getHome()
        {
            return "/getItem: lấy thông tin mặt hàng\n/getSup: lấy nhà cung cấp\n/postItem: thêm mặt hàng";
        }

        [HttpGet("/getItem")]
        public IActionResult getItem()
        {
            DataTable matHangTable = getItemData();

            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in matHangTable.Rows)
            {
                foreach(DataColumn col in matHangTable.Columns)
                {
                    if (col.DataType == typeof(byte[]))
                    {
                        byte[] imageData = (byte[])row[col];
                        string base64 = Convert.ToBase64String(imageData);
                        sb.Append(base64 + "|");
                    }
                    else
                    {
                        sb.Append(row[col].ToString() + "|");
                    }
                }
                sb.AppendLine();
            }


            return Ok(new JsonResult(sb.ToString()));
        }

        [HttpGet("/getSup")]
        public IActionResult getSup()
        {
            DataTable nhaCungCap = getSupData();
            StringBuilder sb = new StringBuilder();

            foreach(DataRow row in nhaCungCap.Rows)
            {
                foreach(DataColumn column in nhaCungCap.Columns)
                {
                    sb.Append(row[column].ToString() + "|");
                }
            }

            return Ok(new JsonResult(sb.ToString()));
        }

        [HttpPost("/postItem")]
        public IActionResult postItem([FromBody] JsonElement data)
        {
            try
            {
                string mamh = data.GetProperty("id").GetString() ?? string.Empty;
                string name = data.GetProperty("name").GetString() ?? string.Empty;
                string currency = data.GetProperty("currency").GetString() ?? string.Empty;
                DateTime date = data.GetProperty("date").GetDateTime();
                int price = data.GetProperty("price").GetInt32();
                int amount = data.GetProperty("amount").GetInt32();
                string base64Image = data.GetProperty("image").GetString() ?? string.Empty;

                string mancc = data.GetProperty("mancc").GetString() ?? string.Empty;

                byte[] image = Convert.FromBase64String(base64Image);

                insertItemData(mamh, name, currency, date, price, amount, image, mancc);

                return Ok("Add Data Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
