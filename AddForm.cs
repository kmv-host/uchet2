using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // Добавили пространство имен для работы с SQL Server  

namespace uchet2
{
    public partial class AddForm : Form
    {
        private string _connectionString;

        public AddForm(string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Computers 
                                (IpAddress, CpuInfo, RamSize, HddSize, OsType, Fio, Email, Phone, Department, MacAddress, NetbiosName) 
                                VALUES 
                                (@Ip, @Cpu, @Ram, @Hdd, @Os, @Fio, @Email, @Phone, @Dept, @Mac, @Netbios)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Ip", txtIp.Text);
                    cmd.Parameters.AddWithValue("@Cpu", txtCpu.Text);
                    cmd.Parameters.AddWithValue("@Ram", txtRam.Text);
                    cmd.Parameters.AddWithValue("@Hdd", txtHdd.Text);
                    cmd.Parameters.AddWithValue("@Os", txtOs.Text); // Ожидается "Windows" или "Linux"
                    cmd.Parameters.AddWithValue("@Fio", txtFio.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@Dept", txtDept.Text);
                    cmd.Parameters.AddWithValue("@Mac", txtMac.Text);
                    cmd.Parameters.AddWithValue("@Netbios", txtNetbios.Text);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при добавлении записи: " + ex.Message);
                    }
                }
            }
        }
    }
}
