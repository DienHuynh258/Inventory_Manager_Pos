﻿using BL;
using DTO;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PL
{
    public partial class SignIn : Form
    {
        public SignIn()
        {
            InitializeComponent();

        }

        private void guna2PictureBox1_Click(object? sender, EventArgs e)
        {

        }

        private async void SignIn_btn_Click(object sender, EventArgs e)
        {
           await signIn();
        }

        private async Task signIn()
        {
            if (!ValidateInputs())
            {
                Guna2MessageDialog errorDialog = new Guna2MessageDialog
                {
                    Text = "Vui lòng nhập đúng thông tin tài khoản và mật khẩu.",
                    Caption = "Thông báo",
                    Buttons = MessageDialogButtons.OK,
                    Style = MessageDialogStyle.Dark,
                    Icon = MessageDialogIcon.Warning,
                    Parent = this
                };
                errorDialog.Show();
                return;
            }

            string username = Username_txb.Text.Trim();
            string password = Password_txb.Text.Trim();
            bool loginSuccess = false;

            try
            {
                loginSuccess = await new LoginBL().LoginAsync(username, password);
            }
            catch (SqlException ex)
            {
                Guna2MessageDialog sqlErrorDialog = new Guna2MessageDialog
                {
                    Text = $"Lỗi kết nối cơ sở dữ liệu: {ex.Message}",
                    Caption = "Lỗi",
                    Buttons = MessageDialogButtons.OK,
                    Style = MessageDialogStyle.Dark,
                    Icon = MessageDialogIcon.Error,
                    Parent = this
                };
                sqlErrorDialog.Show();
                return;
            }
            catch (Exception ex)
            {
                Guna2MessageDialog generalErrorDialog = new Guna2MessageDialog
                {
                    Text = $"Đã xảy ra lỗi không xác định: {ex.Message}",
                    Caption = "Lỗi",
                    Buttons = MessageDialogButtons.OK,
                    Style = MessageDialogStyle.Dark,
                    Icon = MessageDialogIcon.Error,
                    Parent = this
                };
                generalErrorDialog.Show();
                return;
            }

            if (loginSuccess)
            {
                Guna2MessageDialog successDialog = new Guna2MessageDialog
                {
                    Text = "Đăng nhập thành công!",
                    Caption = "Thông báo",
                    Buttons = MessageDialogButtons.OK,
                    Style = MessageDialogStyle.Light,
                    Parent = this
                };
                successDialog.Show();
                Main main = new Main();
                string imagePath = await new LoginBL().GetImagePathByUsernamePasssword(username, password);
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    try
                    {
                        main.pictureBox.Image = Image.FromFile(imagePath);
                        main.username_lbl.Text = username;
                        main.ShowDialog();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        Guna2MessageDialog imageErrorDialog = new Guna2MessageDialog
                        {
                            Text = $"Không thể tải hình ảnh: {ex.Message}",
                            Caption = "Lỗi",
                            Buttons = MessageDialogButtons.OK,
                            Style = MessageDialogStyle.Dark,
                            Icon = MessageDialogIcon.Error,
                            Parent = this
                        };
                        imageErrorDialog.Show();
                    }
                }
            }
            else
            {
                Guna2MessageDialog loginErrorDialog = new Guna2MessageDialog
                {
                    Text = "Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng thử lại.",
                    Caption = "Lỗi",
                    Buttons = MessageDialogButtons.OK,
                    Style = MessageDialogStyle.Dark,
                    Icon = MessageDialogIcon.Error,
                    Parent = this
                };
                loginErrorDialog.Show();

                Username_txb.Clear();
                Password_txb.Clear();
                Username_txb.Focus();
            }
            if (!ValidateInputs())
            {
                Guna2MessageDialog errorDialog = new Guna2MessageDialog
                {
                    Text = "Vui lòng nhập đúng thông tin tài khoản và mật khẩu.",
                    Caption = "Thông báo",
                    Buttons = MessageDialogButtons.OK,
                    Style = MessageDialogStyle.Dark,
                    Icon = MessageDialogIcon.Warning,
                    Parent = this
                };
                errorDialog.Show();
                return;
            }


        }







        private void MyButton_MouseEnter(object sender, EventArgs e)
        {
            // Đổi màu Button khi di chuột vào
            SignIn_label.BackColor = Color.LightBlue;
            // Đổi con trỏ chuột thành hình tay
            SignIn_label.Cursor = Cursors.Hand;
        }

        private void MyButton_MouseLeave(object sender, EventArgs e)
        {
            // Khôi phục màu Button khi di chuột ra
            SignIn_label.BackColor = SystemColors.Control;
            // Khôi phục con trỏ chuột mặc định
            SignIn_label.Cursor = Cursors.Default;
        }

        private bool isPasswordVisible = false;
        private void guna2PictureBox7_Click(object sender, EventArgs e)
        {
            if (isPasswordVisible)
            {
                Password_txb.PasswordChar = '*';
                isPasswordVisible = false;
            }
            else
            {
                Password_txb.PasswordChar = '\0'; // Hiển thị mật khẩu
                isPasswordVisible = true;
            }
        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Password_txb_TextChanged(object sender, EventArgs e)
        {

        }
        //điều kiện kiểm tra password

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);
        }
        private ErrorProvider errorProvider1 = new ErrorProvider();

        private bool ValidateInputs()
        {
            bool isValid = true;

            // Kiểm tra xem tất cả các TextBox có được nhập liệu hay không
            if (string.IsNullOrWhiteSpace(Username_txb.Text) || string.IsNullOrWhiteSpace(Password_txb.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin vào tất cả các ô.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra tính hợp lệ của mật khẩu
            if (!IsValidPassword(Password_txb.Text))
            {
                errorProvider1.SetError(Password_txb, "Mật khẩu phải chứa ít nhất một ký tự thường, một ký tự hoa và một số.");
                isValid = false;
            }
            else
            {
                errorProvider1.SetError(Password_txb, "");
            }

            return isValid;
        }

        private void Username_txb_TextChanged(object sender, EventArgs e)
        {

        }

        private void SignIn_Load(object sender, EventArgs e)
        {
            Username_txb.Text = "Ducci";  // Username mặc định
            Password_txb.Text = "Ducci123";  // Password mặc định
        }
        private void Username_txb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SignIn_btn_Click(sender, e);  // Gọi sự kiện nhấn nút đăng nhập khi Enter được nhấn trong trường Username
            }
        }

        private void Password_txb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SignIn_btn_Click(sender, e);  // Gọi sự kiện nhấn nút đăng nhập khi Enter được nhấn trong trường Password
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
