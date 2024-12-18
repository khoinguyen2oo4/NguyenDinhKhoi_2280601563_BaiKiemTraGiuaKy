using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DE1
{
    public partial class FrmSinhvien : Form
    {
        

        public FrmSinhvien()
        {
            InitializeComponent();
        }
       
        private void FrmSinhvien_Load(object sender, EventArgs e)
        {
            LoadDataFromDatabase();
            LoadComboBoxLop();
        }
        private void LoadDataFromDatabase()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=QuanLySV;Integrated Security=True";
            string query = "SELECT sv.MaSV, sv.HotenSV, sv.NgaySinh, l.TenLop " +
                           "FROM Sinhvien sv " +
                           "JOIN Lop l ON sv.MaLop = l.MaLop";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvSinhvien.DataSource = dt;
                    dgvSinhvien.Columns[0].HeaderText = "Mã SV";
                    dgvSinhvien.Columns[1].HeaderText = "Họ và tên";
                    dgvSinhvien.Columns[2].HeaderText = "Ngày sinh";
                    dgvSinhvien.Columns[3].HeaderText = "Lớp";

                    dgvSinhvien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }
        private void LoadComboBoxLop()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=QuanLySV;Integrated Security=True";
            string query = "SELECT MaLop, TenLop FROM Lop";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cboLop.Items.Add($"{reader["MaLop"]} - {reader["TenLop"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách lớp: " + ex.Message);
            }
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtHoTenSV.Text) || cboLop.SelectedIndex == -1)
            {            
                return;
            }

            string connectionString = "Data Source=localhost;Initial Catalog=QuanLySV;Integrated Security=True";
            string query = "INSERT INTO Sinhvien (MaSV, HotenSV, MaLop, NgaySinh) VALUES (@MaSV, @HotenSV, @MaLop, @NgaySinh)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSV", txtMaSV.Text.Trim());
                        cmd.Parameters.AddWithValue("@HotenSV", txtHoTenSV.Text.Trim());
                        cmd.Parameters.AddWithValue("@MaLop", cboLop.Text.Split('-')[0].Trim());
                        cmd.Parameters.AddWithValue("@NgaySinh", dtNgaysinh.Value);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Thêm dữ liệu thành công!", "Thông báo");

                        LoadDataFromDatabase();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm dữ liệu: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=QuanLySV;Integrated Security=True";
            string query = "UPDATE Sinhvien SET HotenSV = @HotenSV, MaLop = @MaLop, NgaySinh = @NgaySinh WHERE MaSV = @MaSV";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSV", txtMaSV.Text.Trim());
                        cmd.Parameters.AddWithValue("@HotenSV", txtHoTenSV.Text.Trim());
                        cmd.Parameters.AddWithValue("@MaLop", cboLop.Text.Split('-')[0].Trim());
                        cmd.Parameters.AddWithValue("@NgaySinh", dtNgaysinh.Value);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo");
                        LoadDataFromDatabase();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật dữ liệu: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhvien.CurrentRow != null)
            {
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    string maSV = dgvSinhvien.CurrentRow.Cells[0].Value.ToString();
                    string connectionString = "Data Source=localhost;Initial Catalog=QuanLySV;Integrated Security=True";
                    string query = "DELETE FROM Sinhvien WHERE MaSV = @MaSV";

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@MaSV", maSV);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Xóa dữ liệu thành công!", "Thông báo");
                                LoadDataFromDatabase();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa dữ liệu: " + ex.Message);
                    }
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTim_TextChanged(object sender, EventArgs e)
        {
            string searchValue = btnTim.Text.ToLower();
            foreach (ListViewItem item in lvSinhvien.Items)
            {
                if (item.SubItems[1].Text.ToLower().Contains(searchValue))
                {
                    item.BackColor = Color.Yellow;
                }
                else
                {
                    item.BackColor = Color.White;
                }
            }
        }
      
        private void dgvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhvien.Rows[e.RowIndex];
                txtMaSV.Text = row.Cells[0].Value.ToString();
                txtHoTenSV.Text = row.Cells[1].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(row.Cells[2].Value);
                cboLop.Text = row.Cells[3].Value.ToString();
            }
        }

        private void lvSinhvien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSinhvien.SelectedItems.Count > 0)
            {
                var item = lvSinhvien.SelectedItems[0];
                txtMaSV.Text = item.SubItems[0].Text;
                txtHoTenSV.Text = item.SubItems[1].Text;
                dtNgaysinh.Value = DateTime.Parse(item.SubItems[2].Text);
                cboLop.SelectedItem = item.SubItems[3].Text;
            }
        }
      

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchName = txtHoTenSV.Text.Trim().ToLower();
            (dgvSinhvien.DataSource as DataTable).DefaultView.RowFilter = $"[Họ và tên] LIKE '%{searchName}%'";
        }
    }
    }

