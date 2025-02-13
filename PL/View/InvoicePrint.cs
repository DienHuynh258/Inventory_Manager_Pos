﻿using BL.Invoice;
using DTO.Invoice;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL.View
{
    public partial class Invoice_Print : Form
    {
        private InvoiceClass Invoice;
        public Invoice_Print(InvoiceClass invoice)
        {
            InitializeComponent();
            Invoice = invoice;
            this.Load += loadGridviewFunction;
        }
        private void InitializeQRComponents()
        {

            button_QRPay.Click += button_QRPay_Click_1;


        }
        private async void button_QRPay_Click_1(object sender, EventArgs e)
        {
            using (var qrForm = new QR_Payment(Invoice))
            {
                if (qrForm.ShowDialog() == DialogResult.OK)
                {
                    var bl = new InvoiceBL();
                    string qrData = bl.GenerateQRCodeData(Invoice); // Tạo QR data
                    await bl.UpdatePaymentStatus(Invoice.InvoiceID, "Completed", "QR", qrData);
                    MessageBox.Show("Thanh toán thành công!", "Thông báo");
                    await RefreshInvoiceData();
                }
            }
        }
        private async Task RefreshInvoiceData()
        {
            var bl = new InvoiceBL();
            Invoice = await bl.SaleIdGetInvoice(Invoice.SaleID);
            loadGridviewFunction(null, EventArgs.Empty);
        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public static string NormalizeText(string input)
        {
            return string.IsNullOrEmpty(input) ? input : input.Normalize(NormalizationForm.FormC);
        }

        private void loadGridviewFunction(object? sender, EventArgs e)
        {
            if (Invoice == null)
            {

                MessageBox.Show("Dữ liệu hóa đơn không có sẵn.");
                return;
            }

            // Giả sử 'invoiceName' là chuỗi tên sản phẩm được phân tách bằng dấu phẩy
            string invoiceName = Invoice.ProductNameList;
            string invoiceQuantity = Invoice.ProductQuantityList;
            string invoicePrice = Invoice.ProductPriceList;

            // Tách chuỗi hóa đơn thành các tên sản phẩm riêng lẻ
            string[] productnameList = invoiceName.Split(',').Select(p => p.Trim()).ToArray();
            string[] productQuantityList = invoiceQuantity.Split(',').Select(q => q.Trim()).ToArray();
            string[] productPriceList = invoicePrice.Split(',').Select(p => p.Trim()).ToArray();

            // Hiển thị từng phần tử trong danh sách

            // Xóa các hàng hiện có
            guna2DataGridView1.Rows.Clear();

            // Thêm từng chi tiết sản phẩm như một hàng mới trong DataGridView
            for (int i = 0; i < productnameList.Length; i++)
            {
                string productName = NormalizeText(productnameList[i]);
                int quantity = int.Parse(productQuantityList[i]);
                decimal price = decimal.Parse(productPriceList[i]);
                decimal total = quantity * price;

                guna2DataGridView1.Rows.Add(productName, price, quantity, total);
            }

            lbl_MaDH.Text = "Mã Đơn Hàng: " + Invoice.SaleID.ToString();
            lbl_MaKH.Text = "Mã Khách Hàng: " + Invoice.CustomerID.ToString();
            lbl_TongDonHang.Text = Invoice.TotalAmount.ToString();
            lbl_TongTienTT.Text = Invoice.TotalAmount.ToString();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2HtmlLabel14_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel7_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_Print_Click(object sender, EventArgs e)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // Thiết lập phông chữ
            Font headerFont = new Font("Arial", 16, FontStyle.Bold);
            Font subHeaderFont = new Font("Arial", 12, FontStyle.Regular);
            Font tableFont = new Font("Arial", 10, FontStyle.Regular);
            Font totalFont = new Font("Arial", 12, FontStyle.Bold);

            float x = 50; // Lề trái
            float y = 50; // Lề trên
            float lineHeight = 25; // Chiều cao mỗi dòng

            // Vẽ tiêu đề
            g.DrawString("Tiện ích khi đến hệ thống chúng tôi", headerFont, Brushes.Purple, x, y);
            y += lineHeight;

            // Vẽ thông tin khách hàng và đơn hàng
            g.DrawString("Mã khách hàng: " + lbl_MaKH.Text, subHeaderFont, Brushes.Black, x, y);
            y += lineHeight;
            g.DrawString("Mã đơn hàng: " + lbl_MaDH.Text, subHeaderFont, Brushes.Black, x, y);
            y += lineHeight * 2;

            // Vẽ bảng
            float tableStartX = x;
            float tableStartY = y;
            float col1Width = 200; // Chiều rộng cột "Mục"
            float col2Width = 100; // Chiều rộng cột "Số Lượng"
            float col3Width = 100; // Chiều rộng cột "Đơn Giá"
            float col4Width = 100; // Chiều rộng cột "Thành Tiền"

            // Header của bảng
            g.DrawString("Mục", subHeaderFont, Brushes.Purple, tableStartX, y);
            g.DrawString("Số Lượng", subHeaderFont, Brushes.Purple, tableStartX + col1Width, y);
            g.DrawString("Đơn Giá", subHeaderFont, Brushes.Purple, tableStartX + col1Width + col2Width, y);
            g.DrawString("Thành Tiền", subHeaderFont, Brushes.Purple, tableStartX + col1Width + col2Width + col3Width, y);
            y += lineHeight;

            // Duyệt qua dữ liệu trong DataGridView và in từng dòng
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.Cells[0].Value != null) // Kiểm tra giá trị hợp lệ
                {
                    g.DrawString(row.Cells[0].Value.ToString(), tableFont, Brushes.Black, tableStartX, y); // Mục
                    g.DrawString(row.Cells[1].Value.ToString(), tableFont, Brushes.Black, tableStartX + col1Width, y); // Số Lượng
                    g.DrawString(row.Cells[2].Value.ToString(), tableFont, Brushes.Black, tableStartX + col1Width + col2Width, y); // Đơn Giá
                    g.DrawString(row.Cells[3].Value.ToString(), tableFont, Brushes.Black, tableStartX + col1Width + col2Width + col3Width, y); // Thành Tiền
                    y += lineHeight;
                }
            }

            y += lineHeight;

            // Vẽ tổng tiền, thuế, và tổng thanh toán
            g.DrawString("Tổng tiền đơn hàng: " + lbl_TongDonHang.Text, totalFont, Brushes.Purple, x, y);
            y += lineHeight;
            g.DrawString("Thuế (%): " + lbl_Thue.Text, totalFont, Brushes.Purple, x, y);
            y += lineHeight;
            g.DrawString("Tổng tiền thanh toán: " + lbl_TongTienTT.Text, totalFont, Brushes.Purple, x, y);
        }

        private void guna2DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

     
    }
}
