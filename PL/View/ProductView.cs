﻿using BL.Category;
using BL.ProductsBL;
using BL.Purchase;
using DL.Category;
using Guna.UI2.WinForms.Suite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Guna.UI2.WinForms;
using System.Windows.Forms;

namespace PL.View
{
    public partial class ProductView : SampleView
    {
        public ProductView()
        {
            InitializeComponent();
            this.Load += LoadProductsToGridView;
            guna2DataGridView1.CellClick += guna2DataGridView1_CellClick;
            guna2DataGridView1.CellClick += guna2DataGridView1_CellClick_delete;
            guna2DataGridView1.CellClick += guna2DataGridView1_AllInformation;
            txtsearch.TextChanged += txtsearch_TextChanged;
            guna2DataGridView1.CellFormatting += guna2DataGridView1_CellFormatting_Sr;
            ConfigureDataGridView();


        }
        private void ConfigureDataGridView()
        {
            // Cấu hình cơ bản cho DGV
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            guna2DataGridView1.RowTemplate.Height = 60;

            // Theme và màu sắc mới (94,71,204)
            guna2DataGridView1.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            guna2DataGridView1.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(94, 71, 204);
            guna2DataGridView1.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            guna2DataGridView1.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            // Style cho rows
            guna2DataGridView1.ThemeStyle.RowsStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(245, 243, 252); // màu nhạt của tím
            guna2DataGridView1.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            guna2DataGridView1.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(235, 232, 247); // màu tím nhạt khi select
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;

            // Border và Grid
            guna2DataGridView1.ThemeStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            guna2DataGridView1.BorderStyle = BorderStyle.None;

            // Thiết lập ScrollBars để tránh xung đột
            guna2DataGridView1.ScrollBars = ScrollBars.Both;

            // Đảm bảo không gây xung đột khi scroll
            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.AllowUserToResizeColumns = false;

            // Đăng ký sự kiện Scroll để kiểm tra
            guna2DataGridView1.Scroll += Guna2DataGridView1_Scroll;
        }

        private void Guna2DataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            Console.WriteLine($"Scrolled: Orientation={e.ScrollOrientation}, NewValue={e.NewValue}");
        }



        private void ProductView_Load(object sender, EventArgs e)
        {
        }

        private void btnAdd1_Click_1(object sender, EventArgs e)
        {
            ProductAdd productAdd = new ProductAdd();
            productAdd.ShowDialog();
            if (productAdd.DialogResult == DialogResult.OK)
            {
                LoadProductsToGridViewFunction();
            }

        }

        private async void guna2DataGridView1_AllInformation(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Kiểm tra xem người dùng có nhấp vào cột dgvAllInformation hay không
                if (e.ColumnIndex == guna2DataGridView1.Columns["dgvAllInformation"].Index)
                {
                    // Lấy ID từ cột dgvid
                    int id = (int)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvid"].Value;

                    // Lấy thông tin chi tiết của Purchase từ ID
                    string details = await new ProductsBL().LoadProductDetailsByIdAsString(id);

                    // Sử dụng Guna2MessageDialog thay cho MessageBox
                    Guna.UI2.WinForms.Guna2MessageDialog messageDialog = new Guna.UI2.WinForms.Guna2MessageDialog();
                    messageDialog.Text = details;  // Sử dụng Text thay cho Message
                    messageDialog.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    messageDialog.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    messageDialog.Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CellClick: {ex.Message}");

                // Thông báo lỗi với Guna2MessageDialog
                Guna.UI2.WinForms.Guna2MessageDialog errorDialog = new Guna.UI2.WinForms.Guna2MessageDialog();
                errorDialog.Text = "Đã xảy ra lỗi khi lấy thông tin. Vui lòng thử lại.";  // Sử dụng Text thay cho Message
                errorDialog.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                errorDialog.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                errorDialog.Show();
            }
        }

        private void guna2DataGridView1_CellFormatting_Sr(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == guna2DataGridView1.Columns["dgvSr"].Index)
            {
                // Gán số thứ tự cho cột "#Sr"
                e.Value = (e.RowIndex + 1).ToString();
            }

        }
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private async void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào cột Edit
            if (e.RowIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "dgvEdit")
            {
                // Lấy thông tin từ dòng hiện tại
                int id = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvid"].Value);
                string name = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvName"].Value.ToString();
                string barcode = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvBarcode"].Value.ToString();
                int categoryID = (int)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvCatID"].Value;
                int supplierId = (int)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvSupplierID"].Value;
                int quantityInStock = (int)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvQuantityInStock"].Value;
                decimal price = (decimal)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvCost"].Value;
                decimal costPrice = (decimal)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvSalePrice"].Value;
                decimal discount = (decimal)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvDiscount"].Value;
                string description = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvDescription"].Value.ToString();
                string image = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvImage"].Value.ToString();

                // Hiển thị form chỉnh sửa và truyền dữ liệu
                PL.Edit.editProductsForm editProductsForm = new PL.Edit.editProductsForm(id, name, barcode, categoryID, quantityInStock, price, costPrice, discount, supplierId, description, image);
                if (editProductsForm.ShowDialog() == DialogResult.OK)
                {
                    // Load lại dữ liệu sau khi chỉnh sửa
                    LoadProductsToGridViewFunction();
                }
            }
        }

        private async void guna2DataGridView1_CellClick_delete(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào cột Delete
            if (e.RowIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "dgvDel")
            {
                // Hiển thị hộp thoại xác nhận trước khi xóa
                Guna.UI2.WinForms.Guna2MessageDialog confirmDialog = new Guna.UI2.WinForms.Guna2MessageDialog();
                confirmDialog.Text = "Bạn có chắc chắn muốn xóa danh mục này?";  // Sử dụng Text thay cho Message
                confirmDialog.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;
                confirmDialog.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;

                var result = confirmDialog.Show();

                if (result == DialogResult.Yes) // Nếu người dùng chọn "Yes"
                {
                    int id = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvid"].Value);
                    bool deleteResult = await new ProductsBL().DeleteProducts(id);

                    if (deleteResult)
                    {
                        // Hiển thị thông báo thành công
                        Guna.UI2.WinForms.Guna2MessageDialog successDialog = new Guna.UI2.WinForms.Guna2MessageDialog();
                        successDialog.Text = "Xóa danh mục thành công!";  // Sử dụng Text thay cho Message
                        successDialog.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                        successDialog.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                        successDialog.Show();
                        int categoryId = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvcatID"].Value);
                        bool updateCat = await new ProductsBL().subtractQuantityCategory(categoryId, 1);
                        if (updateCat)
                        {
                            LoadProductsToGridViewFunction();
                        }
                    }
                    else
                    {
                        // Thông báo thất bại
                        Guna.UI2.WinForms.Guna2MessageDialog failureDialog = new Guna.UI2.WinForms.Guna2MessageDialog();
                        failureDialog.Text = "Xóa danh mục thất bại!";  // Sử dụng Text thay cho Message
                        failureDialog.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                        failureDialog.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error;
                        failureDialog.Show();
                    }
                }
                else
                {
                    // Nếu người dùng chọn "No", không thực hiện xóa
                    Guna.UI2.WinForms.Guna2MessageDialog cancelDialog = new Guna.UI2.WinForms.Guna2MessageDialog();
                    cancelDialog.Text = "Hành động xóa đã bị hủy.";  // Sử dụng Text thay cho Message
                    cancelDialog.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    cancelDialog.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    cancelDialog.Show();
                }
            }
        }



        private async void LoadProductsToGridViewFunction()
        {
            try
            {
                // Tắt tự động tạo cột
                guna2DataGridView1.AutoGenerateColumns = false;

                // Ánh xạ cột với dữ liệu từ cơ sở dữ liệu
                guna2DataGridView1.Columns["dgvid"].DataPropertyName = "ProductId";
                guna2DataGridView1.Columns["dgvName"].DataPropertyName = "Name";
                guna2DataGridView1.Columns["dgvCatID"].DataPropertyName = "CategoryID";
                guna2DataGridView1.Columns["dgvBarcode"].DataPropertyName = "Barcode";
                guna2DataGridView1.Columns["dgvCost"].DataPropertyName = "CostPrice";
                guna2DataGridView1.Columns["dgvsalePrice"].DataPropertyName = "Price";
                guna2DataGridView1.Columns["dgvQuantityInStock"].DataPropertyName = "QuantityInStock";
                guna2DataGridView1.Columns["dgvDiscount"].DataPropertyName = "Discount";
                guna2DataGridView1.Columns["dgvSupplierID"].DataPropertyName = "SupplierID";
                guna2DataGridView1.Columns["dgvDescription"].DataPropertyName = "Description";
                guna2DataGridView1.Columns["dgvCreateDate"].DataPropertyName = "CreatedAt";
                guna2DataGridView1.Columns["dgvUpdateDate"].DataPropertyName = "UpdatedAt";
                guna2DataGridView1.Columns["dgvImage"].DataPropertyName = "Image";

                // Đọc dữ liệu từ cơ sở dữ liệu
                var data = await new ProductsBL().LoadProducts();
                guna2DataGridView1.DataSource = data;
                guna2DataGridView1.Refresh();

                // Sử dụng sự kiện CellFormatting để hiển thị hình ảnh từ đường dẫn
                guna2DataGridView1.CellFormatting += async (s, e) =>
                {
                    try
                    {
                        if (e.ColumnIndex == guna2DataGridView1.Columns["dgvImageShow"].Index)
                        {
                            string imagePath = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvImage"].Value?.ToString()?.Trim();

                            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            {
                                try
                                {
                                    e.Value = Image.FromFile(imagePath);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error loading image: {ex.Message}");
                                    e.Value = null;
                                }
                            }
                            else
                            {
                                e.Value = null;
                                Console.WriteLine($"No image found at path: {imagePath}");
                            }

                        }
                        else if (e.ColumnIndex == guna2DataGridView1.Columns["dgvCategory"].Index)
                        {
                            int id = (int)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvCatID"].Value;

                            if (id != 0)
                            {
                                string categoryName = await new ProductsBL().GetCategoryNameById(id);
                                guna2DataGridView1.Rows[e.RowIndex].Cells["dgvCategory"].Value = categoryName; // Gán giá trị hiển thị vào e.Value
                            }
                        }
                        else if (e.ColumnIndex == guna2DataGridView1.Columns["dgvSupplier"].Index)
                        {

                            int cellValue = (int)guna2DataGridView1.Rows[e.RowIndex].Cells["dgvSupplierID"].Value;
                            if (cellValue != 0)
                            {
                                string supplierName = await new ProductsBL().GetSupplierNameById(cellValue);
                                guna2DataGridView1.Rows[e.RowIndex].Cells["dgvSupplier"].Value = supplierName;
                            }
                            else
                            {
                                MessageBox.Show("Invalid Supplier ID");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"CellFormatting error: {ex.Message}");
                    }

                };


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data into DataGridView: {ex.Message}");
                MessageBox.Show("An error occurred while loading the data. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadProductsToGridView(object sender, EventArgs e)
        {
            LoadProductsToGridViewFunction();
        }

        private void guna2DataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private async void txtsearch_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtsearch.Text ?? string.Empty;
                var filteredData = await new ProductsBL().SearchProducts(searchText);

                // Tắt tự động tạo cột
                guna2DataGridView1.AutoGenerateColumns = false;

                // Ánh xạ cột với dữ liệu từ cơ sở dữ liệu
                guna2DataGridView1.Columns["dgvid"].DataPropertyName = "ProductId";
                guna2DataGridView1.Columns["dgvName"].DataPropertyName = "Name";
                guna2DataGridView1.Columns["dgvCatID"].DataPropertyName = "CategoryID";
                guna2DataGridView1.Columns["dgvBarcode"].DataPropertyName = "Barcode";
                guna2DataGridView1.Columns["dgvCost"].DataPropertyName = "CostPrice";
                guna2DataGridView1.Columns["dgvsalePrice"].DataPropertyName = "Price";
                guna2DataGridView1.Columns["dgvQuantityInStock"].DataPropertyName = "QuantityInStock";
                guna2DataGridView1.Columns["dgvDiscount"].DataPropertyName = "Discount";
                guna2DataGridView1.Columns["dgvSupplierID"].DataPropertyName = "SupplierID";
                guna2DataGridView1.Columns["dgvDescription"].DataPropertyName = "Description";
                guna2DataGridView1.Columns["dgvCreateDate"].DataPropertyName = "CreatedAt";
                guna2DataGridView1.Columns["dgvUpdateDate"].DataPropertyName = "UpdatedAt";
                guna2DataGridView1.Columns["dgvImage"].DataPropertyName = "Image";

                // Remove existing handler before adding a new one to avoid duplicates
                guna2DataGridView1.CellFormatting -= DataGridView_CellFormatting;

                // Gán dữ liệu mới
                guna2DataGridView1.DataSource = filteredData;

                // Add the new handler
                guna2DataGridView1.CellFormatting += DataGridView_CellFormatting;

                // Refresh DataGridView
                guna2DataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
                MessageBox.Show("An error occurred while searching.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handler riêng cho CellFormatting
        private async void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;

            try
            {
                if (e.ColumnIndex == guna2DataGridView1.Columns["dgvImageShow"]?.Index)
                {
                    string? imagePath = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvImage"].Value?.ToString()?.Trim();

                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        try
                        {
                            using (var image = Image.FromFile(imagePath))
                            {
                                // Tạo một bản sao của hình ảnh để tránh lỗi file access
                                e.Value = new Bitmap(image);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error loading image: {ex.Message}");
                            e.Value = null;
                        }
                    }
                    else
                    {
                        e.Value = null;
                        Console.WriteLine($"No image found at path: {imagePath}");
                    }
                }
                else if (e.ColumnIndex == guna2DataGridView1.Columns["dgvCategory"]?.Index)
                {
                    var cell = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvCatID"];
                    if (cell?.Value != null && cell.Value is int categoryId && categoryId != 0)
                    {
                        string categoryName = await new ProductsBL().GetCategoryNameById(categoryId);
                        if (!string.IsNullOrEmpty(categoryName))
                        {
                            guna2DataGridView1.Rows[e.RowIndex].Cells["dgvCategory"].Value = categoryName;
                        }
                    }
                }
                else if (e.ColumnIndex == guna2DataGridView1.Columns["dgvSupplier"]?.Index)
                {
                    var cell = guna2DataGridView1.Rows[e.RowIndex].Cells["dgvSupplierID"];
                    if (cell?.Value != null && cell.Value is int supplierId && supplierId != 0)
                    {
                        string supplierName = await new ProductsBL().GetSupplierNameById(supplierId);
                        if (!string.IsNullOrEmpty(supplierName))
                        {
                            guna2DataGridView1.Rows[e.RowIndex].Cells["dgvSupplier"].Value = supplierName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CellFormatting error: {ex.Message}");
            }


        }

        private void usermanual_Click(object sender, EventArgs e)
        {
            // Tạo và cấu hình Guna2MessageDialog
            Guna.UI2.WinForms.Guna2MessageDialog messageDialog = new Guna.UI2.WinForms.Guna2MessageDialog
            {
                Caption = "Chức năng quản lý sản phẩm",
                Text = "Chức năng dùng để quản lý thông tin sản phẩm một cách hiệu quả.\n" +
                       "Chức năng bao gồm:\n" +
                       "- Thêm: Thêm mới một sản phẩm.\n" +
                       "- Xóa: Xóa các sản phẩm không còn kinh doanh.\n" +
                       "- Sửa: Thay đổi thông tin sản phẩm hiện có.\n\n" +
                       "Ngoài ra, chức năng tìm kiếm giúp dễ dàng tìm kiếm thông tin sản phẩm theo tên, mã vạch hoặc danh mục.",
                Buttons = MessageDialogButtons.OK,
                Icon = MessageDialogIcon.Information, // Biểu tượng thông tin
                Style = MessageDialogStyle.Light // Phong cách sáng mặc định
            };

            // Hiển thị hộp thoại
            messageDialog.Show();
        }
    }
}