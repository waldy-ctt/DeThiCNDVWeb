using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
        //This form showing 2 table's data (MATHANG & NHACC) so it suppose to have 2 datatable, trust me XD

        String connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\clock\\Usage\\DeThiCNDVWeb\\DeThiCNDVWeb\\Database\\Database1.mdf;Integrated Security=True";
        //I dont give a fuck about this project, but in an actual project that gonna be public, this suppose to be in the env
        //To get connection string, open the Database1.mdf, go to Server Explorer tab and right click on it, propertise and you would find it there

        public Form1()
        {
            InitializeComponent();
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
            cmd = new SqlCommand("SELECT * FROM MATHANG");
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(matHangTable);

            cmd = new SqlCommand("SELECT * FROM NHACUNGCAP");
            cmd.CommandType=CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(nhaCungCapTable);
            
            DisconnectDatabase();
        }

        public void LoadData()
        {
            //dataGridView1.Columns.Add("MaMatHang", "Mã Mặt Hàng");
            //dataGridView1.Columns.Add("TenMatHang", "Tên Mặt Hàng");
            //dataGridView1.Columns.Add("DonViTinh", "Đơn Vị Tính");
            //dataGridView1.Columns.Add("NgayNhap", "Ngày Nhập");
            //dataGridView1.Columns.Add("DonGia", "Đơn Giá");
            //dataGridView1.Columns.Add("SoLuong", "Số Lượng");
            //dataGridView1.Columns.Add("HinhAnh", "HìnhẢnh");
            //dataGridView1.Columns.Add("MaNhaCungCap", "Mã Nhà Cung Cấp");

            //Create mergeTable as the showing data table
            DataTable mergeTable = new DataTable();
            
            mergeTable.Columns.Add("Ma"
        }
    }
}
