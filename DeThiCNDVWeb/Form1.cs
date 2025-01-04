using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace DeThiCNDVWeb
{
    public partial class Form1 : Form
    {
        DataTable matHangTable = new DataTable(), nhaCungCapTable = new DataTable();

        //This for browse button to choose image
        OpenFileDialog openFile = new OpenFileDialog();

        //This is image that processing
        byte[] image;
        string base64ImageString;

        private async void fetchGetItem()
        {
            try
            {
                resetMatHangTable();
                HttpClient client = new HttpClient();
                String url = "https://localhost:7232/getItem";
                HttpResponseMessage responseMessage = await client.GetAsync(url);

                responseMessage.EnsureSuccessStatusCode();
                String responseBody = await responseMessage.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(responseBody);
                String value = jsonDoc.RootElement.GetProperty("value").GetString();

                ConvertStringToDataTable(value);

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = matHangTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        private async void fetchGetNCC()
        {
            try
            {
                resetNhaCungCapTable();
                HttpClient client = new HttpClient();
                String url = "https://localhost:7232/getSup";
                HttpResponseMessage responseMessage = await client.GetAsync(url);

                responseMessage.EnsureSuccessStatusCode();
                String responseBody = await responseMessage.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(responseBody);
                String value = jsonDoc.RootElement.GetProperty("value").GetString();
                Console.WriteLine("ncc: " + value);

                String[] ncc = value.Split(new char[] { '|' });

                for (int i = 0; i < ncc.Length; i += 2)
                {
                    if (i + 1 < ncc.Length)
                    {
                        string mancc = ncc[i].Trim(); string tenncc = ncc[i + 1].Trim(); if (!string.IsNullOrEmpty(mancc) && !string.IsNullOrEmpty(tenncc)) 
                        {
                            DataRow dr = nhaCungCapTable.NewRow();
                            dr["mancc"] = mancc;
                            dr["tenncc"] = tenncc; 
                            nhaCungCapTable.Rows.Add(dr);
                            comboBox_nhaCungCap.Items.Add($"{mancc} - {tenncc}");
                            Console.WriteLine($"ddddd: {mancc} - {tenncc}");
                        }
                    }
                }

                //comboBox_nhaCungCap.DataSource = null;
                //comboBox_nhaCungCap.DataSource = nhaCungCapTable;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        private void ConvertStringToDataTable(string data)
        {
            string[] rows = data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                string[] columns = row.Split('|');
                byte[] imageData = Convert.FromBase64String(columns[6]);
                Image image = ByteArrayToImage(imageData);
                DataRow dataRow = matHangTable.NewRow();
                dataRow["Mã Hàng"] = columns[0];
                dataRow["Tên Hàng"] = columns[1];
                dataRow["Đơn Vị Tính"] = columns[2];
                dataRow["Ngày Nhập"] = DateTime.Parse(columns[3]);
                dataRow["Đơn Giá"] = decimal.Parse(columns[4]);
                dataRow["Số Lượng"] = int.Parse(columns[5]);
                dataRow["Hình Ảnh"] = image;
                dataRow["Mã Nhà Cung Cấp"] = columns[7];
                matHangTable.Rows.Add(dataRow);
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        public void resetMatHangTable()
        {
            matHangTable = new DataTable();

            matHangTable.Columns.Add("Mã Hàng");
            matHangTable.Columns.Add("Tên Hàng");
            matHangTable.Columns.Add("Đơn Vị Tính");
            matHangTable.Columns.Add("Ngày Nhập");
            matHangTable.Columns.Add("Đơn Giá");
            matHangTable.Columns.Add("Số Lượng");
            matHangTable.Columns.Add("Hình Ảnh", typeof(Image));
            matHangTable.Columns.Add("Mã Nhà Cung Cấp");

        }

        public void resetNhaCungCapTable()
        {
            nhaCungCapTable = new DataTable();

            nhaCungCapTable.Columns.Add("mancc");
            nhaCungCapTable.Columns.Add("tenncc");
        }

        public Form1()
        {
            InitializeComponent();

            LoadData();
        }

        private void button_browse_Click(object sender, EventArgs e)
        {
            try
            {
                openFile.InitialDirectory = "C:";
                openFile.FileName = "";
                openFile.Filter = "All Files (*.*) | *.*";
                openFile.RestoreDirectory = true;
                openFile.ShowDialog();

                if (openFile.FileName != "")
                {
                    textBox_hinhAnh.Text = openFile.SafeFileName.Substring(openFile.SafeFileName.LastIndexOf("\\") + 1);
                    pictureBox1.Image = Image.FromFile(openFile.FileName);
                }

                FileStream fs = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read);
                image = new byte[fs.Length];
                fs.Read(image, 0, image.Length);
                fs.Close();

                base64ImageString = Convert.ToBase64String(image);
            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException)
                {
                    MessageBox.Show("Ảnh không hợp lệ, vui lòng thử tải ảnh hoặc chọn ảnh khác");
                }
                else
                {
                    MessageBox.Show(ex.Message, "Show this to developer");
                }

                return;
            }
        }

        public void saveImageToUrl()
        {
            if (!Directory.Exists("Images"))
            {
                Directory.CreateDirectory("Images");
            }
            if (openFile.FileName != "")
            {
                switch (Path.GetExtension(openFile.FileName))
                {
                    case ".jpg":
                        pictureBox1.Image.Save(Directory.GetCurrentDirectory() + "\\Images\\" + textBox_hinhAnh.Text, ImageFormat.Jpeg);
                        break;

                    case ".bmp":
                        pictureBox1.Image.Save(Directory.GetCurrentDirectory() + "\\Images\\" + textBox_hinhAnh.Text, ImageFormat.Bmp);
                        break;

                    case ".png":
                        pictureBox1.Image.Save(Directory.GetCurrentDirectory() + "\\Images\\" + textBox_hinhAnh.Text, ImageFormat.Png);
                        break;

                    case ".gif":
                        pictureBox1.Image.Save(Directory.GetCurrentDirectory() + "\\Images\\" + textBox_hinhAnh.Text, ImageFormat.Gif);
                        break;
                }
            }
        }

        public void LoadData()
        {
            comboBox_donViTinh.Items.Add("VNĐ");
            comboBox_donViTinh.Items.Add("USD");
            comboBox_donViTinh.Items.Add("YEN");
            comboBox_donViTinh.Items.Add("Canada USD");
            comboBox_donViTinh.Items.Add("WON");

            fetchGetNCC();

            fetchGetItem();
        }

        private void resetInput()
        {
            textBox_donGia.Clear();
            textBox_hinhAnh.Clear();
            textBox_maMatHang.Clear();
            textBox_soLuong.Clear();
            textBox_tenMatHang.Clear();
            comboBox_donViTinh.SelectedIndex = 0;
            pictureBox1.Image = null;

            dataGridView1.Refresh();
        }

        private async void button_nhap_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_donGia.Text.Length == 0 || textBox_maMatHang.Text.Length == 0 || textBox_hinhAnh.Text.Length == 0 || textBox_soLuong.Text.Length == 0 || textBox_tenMatHang.Text.Length == 0 || comboBox_donViTinh.Text.Length == 0 || comboBox_nhaCungCap.Text.Length == 0)
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin");
                    return;
                }

                var data = new
                {
                    id = textBox_maMatHang.Text,
                    name = textBox_tenMatHang.Text,
                    currency = comboBox_donViTinh.Text,
                    date = dateTimePicker1.Value,
                    price = int.Parse(textBox_donGia.Text),
                    amount = int.Parse(textBox_soLuong.Text),
                    image = base64ImageString,
                    mancc = comboBox_nhaCungCap.Text,
                };

                String jsonData = JsonSerializer.Serialize(data);

                using (HttpClient client = new HttpClient())
                {
                    string url = "https://localhost:7232/postItem";

                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage responseMessage = await client.PostAsync(url, content);
                        responseMessage.EnsureSuccessStatusCode();
                        String response = await responseMessage.Content.ReadAsStringAsync();
                        MessageBox.Show(response);
                        if (responseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            LoadData();
                            resetInput();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                {
                    MessageBox.Show("Số lượng và đơn giá không thể vượt quá 2,147,483,647");
                }
                else
                {
                    MessageBox.Show(ex.Message, "Show this to developer");
                }

                return;
            }
        }
    }
}
