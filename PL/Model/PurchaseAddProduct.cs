﻿using BL.ProductsBL;
using BL.Purchase;
using Guna.UI2.WinForms;
using PL.View;
using System.Windows.Forms;

namespace PL.Model
{
    public partial class PurchaseAddProduct : Form
    {
        public PurchaseAddProduct()
        {
            InitializeComponent();
            this.Load += LoadToComboBox;
            this.Load += LoadProducts;
            cb_Supplier.SelectedIndexChanged += cb_Supplier_select;
            dataGridViewCart.CellClick += AddToCartForClickImageCellClick;
            dataGridViewCart.CellClick += SubtractToCartCellClick;
            ConfigureFlowLayoutPanel();
            txtsearch.TextChanged += txtsearch_TextChanged;

        }
        private void ConfigureFlowLayoutPanel()
        {


            // Bật tính năng cuộn
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.HorizontalScroll.Enabled = false;
            flowLayoutPanel1.HorizontalScroll.Visible = false;
            flowLayoutPanel1.VerticalScroll.Enabled = true;
            flowLayoutPanel1.VerticalScroll.Visible = true;

            flowLayoutPanel1.WrapContents = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.AutoSize = false;

            flowLayoutPanel1.Padding = new Padding(10);
            flowLayoutPanel1.Margin = new Padding(0);

            flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel1.BackColor = Color.White;


        }




        private async void LoadToComboBox(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ cơ sở dữ liệu (ví dụ, danh sách employees)
            var CreatedBy = await new BL.Purchase.PurchaseBL().loadUsersToComboBox();
            // Giả sử bạn có một DataGridView để hiển thị danh sách
            cb_CreatedBy.DataSource = CreatedBy;
            cb_CreatedBy.DisplayMember = "UserName";
            cb_CreatedBy.ValueMember = "Id";

            var supplier = await new ProductsBL().LoadSupplierIntoComboBox();

            cb_Supplier.DataSource = supplier;
            cb_Supplier.DisplayMember = "Name";
            cb_Supplier.ValueMember = "Id";
        }


        private void PurchaseAddProduct_Load(object sender, EventArgs e)
        {

        }

        private void txtsearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtsearch.Text.ToLower();
            foreach (Control control in flowLayoutPanel1.Controls) // Changed guna2Panel1 to flowLayoutPanel1
            {
                if (control is Panel productPanel)
                {
                    bool isVisible = false;
                    foreach (Control panelControl in productPanel.Controls)
                    {
                        if (panelControl is Label productNameLabel && productNameLabel.Text.ToLower().Contains(searchText))
                        {
                            isVisible = true;
                            break;
                        }
                    }
                    productPanel.Visible = isVisible;
                }
            }
        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }
        decimal total_Amount = 0;
        private void UpdateGrandTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dataGridViewCart.Rows)
            {
                total += Convert.ToDecimal(row.Cells["dgvAmount"].Value);
            }
            // Hiển thị tổng tiền lên giao diện
            lbl_Total.Text = total.ToString("N0") + " VNĐ";
            total_Amount = total;
        }

        private async void AddToCart(int productId)
        {
            // Lấy sản phẩm từ database
            var product = await new ProductsBL().getProducts(productId);

            if (product != null)
            {
                // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
                bool isProductInCart = false;
                foreach (DataGridViewRow row in dataGridViewCart.Rows)
                {
                    // Kiểm tra ProductID trong giỏ hàng
                    if ((int)row.Cells["dgvSupplierId"].Value != 0 && (int)row.Cells["dgvSupplierId"].Value != product.SupplierID)
                    {
                        ShowMessage("Không thể thêm sản phẩm khác nhà cung cấp để tạo đơn hàng", "Lỗi", MessageDialogIcon.Error);
                        isProductInCart = false;
                        return;
                    }

                    else if (row.Cells["dgvId"].Value != null && (int)row.Cells["dgvId"].Value == product.ProductID)
                    {
                        // Nếu có, cập nhật số lượng và Amount
                        int currentQuantity = Convert.ToInt32(row.Cells["dgvQuantity"].Value);
                        row.Cells["dgvQuantity"].Value = currentQuantity + 1;  // Tăng số lượng lên 1

                        // Cập nhật lại Amount
                        decimal price = Convert.ToDecimal(row.Cells["dgvPrice"].Value);
                        row.Cells["dgvAmount"].Value = price * (currentQuantity + 1);

                        // Đánh dấu là sản phẩm đã có trong giỏ hàng
                        isProductInCart = true;
                        break;
                    }
                }

                // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
                if (!isProductInCart)
                {
                    // Thêm sản phẩm vào DataGridView (lưu ý thêm ProductID, Name, Quantity, CostPrice, Price, Amount)
                    dataGridViewCart.Rows.Add(product.Name, product.ProductID, product.SupplierID, 1, product.CostPrice, product.CostPrice);


                    // Cập nhật tổng tiền (có thể tính lại tổng tiền ở đây)
                    UpdateGrandTotal();
                }
                else
                {
                    // Cập nhật tổng tiền nếu sản phẩm đã có trong giỏ hàng
                    UpdateGrandTotal();
                }




            }
        }


        private async void AddToCartForClickImage(int productId)
        {
            // Lấy sản phẩm từ database
            var product = await new ProductsBL().getProducts(productId);

            if (product != null)
            {
                // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
                foreach (DataGridViewRow row in dataGridViewCart.Rows)
                {
                    // Kiểm tra ProductID trong giỏ hàng (xử lý null)
                    if (row.Cells["dgvId"].Value != null && Convert.ToInt32(row.Cells["dgvId"].Value) == product.ProductID)
                    {
                        // Nếu có, cập nhật số lượng và Amount
                        int currentQuantity = Convert.ToInt32(row.Cells["dgvQuantity"].Value);
                        row.Cells["dgvQuantity"].Value = currentQuantity + 1;  // Tăng số lượng lên 1

                        // Cập nhật lại Amount
                        decimal price = Convert.ToDecimal(row.Cells["dgvPrice"].Value);
                        row.Cells["dgvAmount"].Value = price * (currentQuantity + 1);

                        // Cập nhật tổng tiền
                        UpdateGrandTotal();
                        return; // Dừng vòng lặp
                    }
                }

            }
        }

        private async void AddToCartForClickImageCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridViewCart.Columns[e.ColumnIndex].Name == "dgvAdd")
            {
                // Lấy ProductID từ dòng hiện tại
                int productId = Convert.ToInt32(dataGridViewCart.Rows[e.RowIndex].Cells["dgvId"].Value);

                // Gọi phương thức thêm sản phẩm
                AddToCartForClickImage(productId);
            }
        }



        private async void SubtractToCart(int productId)
        {
            // Lấy sản phẩm từ database
            var product = await new ProductsBL().getProducts(productId);

            if (product != null)
            {
                foreach (DataGridViewRow row in dataGridViewCart.Rows)
                {
                    // Kiểm tra ProductID trong giỏ hàng
                    if (row.Cells["dgvId"].Value != null && Convert.ToInt32(row.Cells["dgvId"].Value) == product.ProductID)
                    {
                        // Nếu sản phẩm được tìm thấy, cập nhật số lượng và Amount
                        int currentQuantity = Convert.ToInt32(row.Cells["dgvQuantity"].Value);

                        if (currentQuantity > 1)
                        {
                            row.Cells["dgvQuantity"].Value = currentQuantity - 1;  // Giảm số lượng
                            decimal price = Convert.ToDecimal(row.Cells["dgvPrice"].Value);
                            row.Cells["dgvAmount"].Value = price * (currentQuantity - 1); // Cập nhật Amount
                        }
                        else
                        {
                            // Nếu số lượng là 1, xóa sản phẩm khỏi giỏ hàng
                            dataGridViewCart.Rows.Remove(row);
                        }

                        UpdateGrandTotal();
                        return; // Dừng vòng lặp sau khi tìm thấy và xử lý sản phẩm
                    }
                }
            }
        }

        private async void SubtractToCartCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridViewCart.Columns[e.ColumnIndex].Name == "dgvSubtract")
            {
                // Lấy ProductID từ dòng hiện tại
                int productId = Convert.ToInt32(dataGridViewCart.Rows[e.RowIndex].Cells["dgvId"].Value);

                // Gọi phương thức thêm sản phẩm
                SubtractToCart(productId);
            }
        }
        private async void cb_Supplier_select(object sender, EventArgs e)
        {
            // Kiểm tra nếu có giá trị được chọn trong ComboBox
            if (cb_Supplier.SelectedValue != null)
            {
                // Lấy giá trị của Id từ SelectedValue và ép kiểu sang int
                if (cb_Supplier.SelectedValue is int selectedSupplierId)
                {
                    var products = await new PurchaseBL().LoadProductFromSupplier(selectedSupplierId);
                    //var products = await new ProductsBL().LoadProducts();
                    flowLayoutPanel1.Controls.Clear();

                    foreach (var product in products)
                    {
                        // Tạo Panel chính chứa thông tin sản phẩm
                        Panel productPanel = new Panel
                        {
                            Width = 150,
                            Height = 200,
                            Padding = new Padding(5),
                            BackColor = Color.White,
                            BorderStyle = BorderStyle.FixedSingle
                        };

                        // Hình ảnh sản phẩm
                        PictureBox productImage = new PictureBox
                        {
                            Width = 100,
                            Height = 100,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Location = new Point((productPanel.Width - 100) / 2, 10)
                        };

                        if (!string.IsNullOrEmpty(product.Image) && File.Exists(product.Image))
                        {
                            productImage.Image = Image.FromFile(product.Image);
                        }
                        productPanel.Controls.Add(productImage);

                        // Tên sản phẩm
                        Label productName = new Label
                        {
                            Text = product.Name,
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Size = new Size(productPanel.Width - 10, 25),
                            Location = new Point(5, productImage.Bottom + 10)
                        };
                        productPanel.Controls.Add(productName);

                        // Giá sản phẩm
                        Label productPrice = new Label
                        {
                            Text = product.Price.ToString("N0") + " VNĐ",
                            Font = new Font("Segoe UI", 12, FontStyle.Bold),
                            ForeColor = Color.FromArgb(94, 71, 204),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Size = new Size(productPanel.Width - 10, 25),
                            Location = new Point(5, productName.Bottom + 5)
                        };
                        productPanel.Controls.Add(productPrice);

                        // Gắn sự kiện thêm vào giỏ hàng
                        var controls = new List<Control> { productPanel, productName, productPrice, productImage };
                        foreach (var control in controls)
                        {
                            control.Click += (s, evt) => AddToCart(product.ProductID);
                            control.Cursor = Cursors.Hand;
                        }
                        // Hiệu ứng hover với màu tím nhạt
                        Color hoverColor = Color.FromArgb(235, 232, 247); // Màu tím nhạt khi hover
                        productPanel.MouseEnter += (s, evt) =>
                        {
                            productPanel.BackColor = hoverColor;
                            foreach (Control control in productPanel.Controls)
                            {
                                control.BackColor = hoverColor;
                            }
                        };
                        productPanel.MouseLeave += (s, evt) =>
                        {
                            productPanel.BackColor = Color.White;
                            foreach (Control control in productPanel.Controls)
                            {
                                control.BackColor = Color.White;
                            }
                        };
                        // Thêm sản phẩm vào FlowLayoutPanel
                        flowLayoutPanel1.Controls.Add(productPanel);




                    }
                }
                else
                {
                        ShowMessage("Giá trị được chọn không phải là loại mong đợi.", "Lỗi", MessageDialogIcon.Error);
                    
                }
            }
            else
            {
                ShowMessage("Không có nhà cung cấp nào được chọn.", "Lỗi", MessageDialogIcon.Error);
            }
        }

        private void ShowMessage(string message, string title, MessageDialogIcon icon)
        {
            Guna2MessageDialog messageDialog = new Guna2MessageDialog();
            messageDialog.Caption = title;
            messageDialog.Text = message;
            messageDialog.Icon = icon;
            messageDialog.Buttons = MessageDialogButtons.OK;
            messageDialog.Style = MessageDialogStyle.Default;
            messageDialog.Parent = this;
            messageDialog.Show();
        }

        private async void LoadProducts(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear(); // Xóa các controls cũ nếu có

            // Tải toàn bộ sản phẩm từ cơ sở dữ liệu
            var products = await new ProductsBL().LoadProducts();

            foreach (var product in products)
            {
                // Tạo Panel chính chứa thông tin sản phẩm
                Panel productPanel = new Panel
                {
                    Width = 150,
                    Height = 200,
                    Padding = new Padding(5),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Hình ảnh sản phẩm
                PictureBox productImage = new PictureBox
                {
                    Width = 100,
                    Height = 100,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point((productPanel.Width - 100) / 2, 10)
                };

                if (!string.IsNullOrEmpty(product.Image) && File.Exists(product.Image))
                {
                    productImage.Image = Image.FromFile(product.Image);
                }
                productPanel.Controls.Add(productImage);

                // Tên sản phẩm
                Label productName = new Label
                {
                    Text = product.Name,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(productPanel.Width - 10, 25),
                    Location = new Point(5, productImage.Bottom + 10)
                };
                productPanel.Controls.Add(productName);

                // Giá sản phẩm
                Label productPrice = new Label
                {
                    Text = product.Price.ToString("N0") + " VNĐ",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(94, 71, 204),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(productPanel.Width - 10, 25),
                    Location = new Point(5, productName.Bottom + 5)
                };
                productPanel.Controls.Add(productPrice);

                // Gắn sự kiện thêm vào giỏ hàng
                var controls = new List<Control> { productPanel, productName, productPrice, productImage };
                foreach (var control in controls)
                {
                    control.Click += (s, evt) => AddToCart(product.ProductID);
                    control.Cursor = Cursors.Hand;
                }
                // Hiệu ứng hover với màu tím nhạt
                Color hoverColor = Color.FromArgb(235, 232, 247); // Màu tím nhạt khi hover
                productPanel.MouseEnter += (s, evt) =>
                {
                    productPanel.BackColor = hoverColor;
                    foreach (Control control in productPanel.Controls)
                    {
                        control.BackColor = hoverColor;
                    }
                };
                productPanel.MouseLeave += (s, evt) =>
                {
                    productPanel.BackColor = Color.White;
                    foreach (Control control in productPanel.Controls)
                    {
                        control.BackColor = Color.White;
                    }
                };

                // Thêm sản phẩm vào FlowLayoutPanel
                flowLayoutPanel1.Controls.Add(productPanel);
            }
        }



        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void txt_Name_TextChanged(object sender, EventArgs e)
        {

        }

        private void cb_Status_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cb_CreatedBy_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void btn_Save_Click(object sender, EventArgs e)
        {
            int supplierId = (int)cb_Supplier.SelectedValue;
            string createdBy = cb_CreatedBy.Text;
            string notes = txt_notes.Text;

            // Hỏi quản lý xác nhận đã nhận hàng chưa
            DialogResult confirmResult = MessageBox.Show("Quản lý đã xác nhận nhận hàng chưa?", "Xác nhận", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                bool result = await new BL.Purchase.PurchaseBL().addPurchase(supplierId, total_Amount, createdBy, notes);
                if (result)
                {
                    ShowMessage("Tạo đơn hàng thành công!", "Thông báo", MessageDialogIcon.Information);

                    foreach (DataGridViewRow row in dataGridViewCart.Rows)
                    {
                        if (row.Cells["dgvId"].Value != null && row.Cells["dgvQuantity"].Value != null)
                        {
                            int productId = Convert.ToInt32(row.Cells["dgvId"].Value);
                            int quantity = Convert.ToInt32(row.Cells["dgvQuantity"].Value);
                            // Add your logic here to handle productId and quantity
                            bool resultAdd = await new ProductsBL().AddQuantityProduct(productId, quantity);
                        }
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            else
            {
                ShowMessage("Quản lý chưa xác nhận nhận hàng. Không thể tạo đơn hàng.", "Cảnh báo", MessageDialogIcon.Warning);
            }
        }



        private void dataGridViewCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cb_Supplier_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
