using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeThiCNDVWeb
{
    public partial class Form1 : Form
    {
        //Inital Public Variable
        SqlConnection con;
        SqlDataAdapter da;
        SqlCommand cmd;
        DataTable matHangTable = new DataTable(), nhaCungCapTable = new DataTable();

        //This for browse button to choose image
        OpenFileDialog openFile = new OpenFileDialog();

        //This is image that processing
        byte[] image;

        String connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\clock\\Usage\\DeThiCNDVWeb\\DeThiCNDVWeb\\Database\\Database1.mdf;Integrated Security=True";
        //I dont give a fuck about this project, but in an actual project that gonna be public, this suppose to be in the env
        //To get connection string, open the Database1.mdf, go to Server Explorer tab and right click on it, propertise and you would find it there

        public Form1()
        {
            InitializeComponent();
            LoadData();
        }

        //Create connection to database
        public void ConnectDatabase()
        {
            try
            {
                con = new SqlConnection(connectionString);
                con.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DisconnectDatabase()
        {
            try
            {
                con.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void GetData()
        {
            //Im just writting this comment because ngl without vim motion, everything going kinda slow   

            ConnectDatabase();

            cmd = new SqlCommand("SELECT * FROM MATHANG", con);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(matHangTable);

            cmd = new SqlCommand("SELECT * FROM NHACUNGCAP", con);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(nhaCungCapTable);

            DisconnectDatabase();
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
            }
            catch (Exception ex)
            {
                if(ex is OutOfMemoryException)
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
            if (!Directory.Exists("Images")) {
                Directory.CreateDirectory("Images");
            }
            if (openFile.FileName != "") {
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
            GetData();

            comboBox_donViTinh.Items.Add("VNĐ");
            comboBox_donViTinh.Items.Add("USD");
            comboBox_donViTinh.Items.Add("YEN");
            comboBox_donViTinh.Items.Add("Canada USD");
            comboBox_donViTinh.Items.Add("WON");

            foreach (DataRow nhaCungCap in nhaCungCapTable.Rows)
            {
                comboBox_nhaCungCap.Items.Add(nhaCungCap.ItemArray[0]);
            }

            dataGridView1.DataSource = matHangTable;
        }

        private void resetInput()
        {
            textBox_donGia.Clear();
            textBox_hinhAnh.Clear();
            textBox_maMatHang.Clear();
            textBox_soLuong.Clear();
            textBox_tenMatHang.Clear();
            comboBox_donViTinh.SelectedIndex = 0;
            //comboBox_nam.SelectedIndex = 0;
            //comboBox_ngay.SelectedIndex = 0;
            //comboBox_thang.SelectedIndex = 0;
            pictureBox1.Image = null;
        }

        private void button_nhap_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_donGia.Text.Length == 0 || textBox_maMatHang.Text.Length == 0 || textBox_hinhAnh.Text.Length == 0 || textBox_soLuong.Text.Length == 0 || textBox_tenMatHang.Text.Length == 0 || comboBox_donViTinh.Text.Length == 0 || comboBox_nhaCungCap.Text.Length == 0)
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin");
                    return;
                }

                ConnectDatabase();

                cmd = new SqlCommand("INSERT INTO MATHANG(MAMH, TENMH, DVT, NGAYNHAP, DONGIA, SOLUONG, HINHANH, MANCC) VALUES (@MAMH, @TENMH, @DVT, @NGAYNHAP, @DONGIA, @SOLUONG, @HINHANH, @MANCC)", con);
                cmd.Parameters.Add("@MAMH", SqlDbType.VarChar).Value = textBox_maMatHang.Text;
                cmd.Parameters.Add("@TENMH", SqlDbType.NVarChar).Value = textBox_tenMatHang.Text;
                cmd.Parameters.Add("@DVT", SqlDbType.NVarChar).Value = comboBox_donViTinh.Text;
                cmd.Parameters.Add("@NGAYNHAP", SqlDbType.Date).Value = dateTimePicker1.Value;
                cmd.Parameters.Add("@DONGIA", SqlDbType.Int).Value = Int32.Parse(textBox_donGia.Text);
                cmd.Parameters.Add("@SOLUONG", SqlDbType.Int).Value = Int32.Parse(textBox_soLuong.Text);
                cmd.Parameters.Add("@HINHANH", SqlDbType.Image).Value = image;
                cmd.Parameters.Add("@MANCC", SqlDbType.VarChar).Value = comboBox_nhaCungCap.Text;

                cmd.ExecuteNonQuery();

                DisconnectDatabase();

                saveImageToUrl();

                LoadData();
            }
            catch(Exception ex)
            {
                if (ex is OverflowException)
                {
                    MessageBox.Show("Số lượng và đơn giá không thể vượt quá 2,147,483,647");
                }
                else {
                    MessageBox.Show(ex.Message, "Show this to developer");
                }

                return;
            }
        }
    }
}
