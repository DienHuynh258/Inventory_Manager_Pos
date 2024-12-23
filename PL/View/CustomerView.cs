﻿using BL;
using BL.Customer;
using PL.Edit;
using PL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL.View
{
    public partial class CustomerView : SampleView
    {
        public CustomerView()
        {
            InitializeComponent();
            this.Load += LoadGridView;
            guna2DataGridView1.CellFormatting += Guna2DataGridView1_CellFormatting;
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick;
            guna2DataGridView1.CellClick += Guna2DataGridView1_CellClick_Delete;
        }

        private void LoadGridView(object sender, EventArgs e)
        {
            LoadCustomerToGridViewFunction();
        }

        public async void LoadCustomerToGridViewFunction()
        {
            try
            {
                // Tắt tự động tạo cột
                guna2DataGridView1.AutoGenerateColumns = false;

                // Ánh xạ cột với dữ liệu từ cơ sở dữ liệu
                guna2DataGridView1.Columns["dgvid"].DataPropertyName = "Id";
                guna2DataGridView1.Columns["dgvName"].DataPropertyName = "Name";
                guna2DataGridView1.Columns["dgvPhone"].DataPropertyName = "Phone";
                guna2DataGridView1.Columns["dgvEmail"].DataPropertyName = "Email";

                // Đọc dữ liệu từ cơ sở dữ liệu
                var data = await new CustomerBL().LoadCustomers(); // Giả sử LoadCustomers trả về danh sách các đối tượng Customer
                guna2DataGridView1.DataSource = data;
                guna2DataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data into DataGridView: {ex.Message}");
                MessageBox.Show("An error occurred while loading the data. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void Guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == guna2DataGridView1.Columns["dgvSr"].Index)
            {
                // Gán số thứ tự cho cột "#Sr"
                e.Value = (e.RowIndex + 1).ToString();
            }
        }

        private async void Guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào cột Edit
            if (e.RowIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "dgvEdit")
            {
                // Lấy thông tin từ dòng hiện tại
                int id = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvid"].Value);
                string name = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvName"].Value.ToString();
                string phone = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvPhone"].Value.ToString();
                string email = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvEmail"].Value.ToString();

                // Hiển thị form chỉnh sửa và truyền dữ liệu
                PL.Edit.editCustomerForm editForm = new editCustomerForm(id, name, phone, email);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Load lại dữ liệu sau khi chỉnh sửa
                    guna2DataGridView1.DataSource = await new CustomerBL().LoadCustomers();
                }
            }
        }

        private async void Guna2DataGridView1_CellClick_Delete(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào cột Delete
            if (e.RowIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "dgvDel")
            {
                // Hiển thị hộp thoại xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) // Nếu người dùng chọn "Yes"
                {
                    int id = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvid"].Value);
                    bool deleteResult = await new CustomerBL().DeleteCustomer(id);

                    if (deleteResult)
                    {
                        MessageBox.Show("Xóa khách hàng thành công!");
                        LoadCustomerToGridViewFunction();
                    }
                    else
                    {
                        MessageBox.Show("Xóa khách hàng thất bại!");
                    }
                }
                else
                {
                    // Nếu người dùng chọn "No", không thực hiện xóa
                    MessageBox.Show("Hành động xóa đã bị hủy.");
                }
            }
        }

        private void btnAdd1_Click_1(object sender, EventArgs e)
        {
            CustomerAdd customerAdd = new CustomerAdd();
            customerAdd.ShowDialog();

        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }
    }
}
