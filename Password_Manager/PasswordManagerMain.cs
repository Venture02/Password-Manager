using Password_Manager.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Password_Manager
{
    public partial class PasswordManagerMain : Form
    {
        PasswordRepository passwordRepository = new PasswordRepository();
        public PasswordManagerMain()
        {
            InitializeComponent();
        }

        private void btnAddEntry_Click(object sender, EventArgs e)
        {
            string website, username, password, encryptedPassword;
            website = txtBoxWebsite.Text;
            username = txtBoxUsername.Text;
            password = txtBoxPassword.Text;

            // If the textbox is empty show a message box
            if (website == "")
            {
                MessageBox.Show($"Fill in the website");
                txtBoxWebsite.Focus();
            }
            else if (username == "")
            {
                MessageBox.Show($"Fill in the username");
                txtBoxUsername.Focus();
            }
            else if (password == "")
            {
                MessageBox.Show($"Fill in the password");
                txtBoxPassword.Focus();
            }
            else
            {
                encryptedPassword = EncryptionHelper.Encrypt(password);
                passwordRepository.AddPassword(website, username, encryptedPassword); // if everything is filled add it to the database
                txtBoxWebsite.Clear();
                txtBoxUsername.Clear();
                txtBoxPassword.Clear();
                MaskPasswords();
                MessageBox.Show($"Entry added!");
            }
        }

        private void btnShowPasswords_Click(object sender, EventArgs e)
        {
            if (dataGridViewPasswords.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a password first.");
                return;
            }
            DataGridViewRow row = dataGridViewPasswords.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["Id"].Value);

            DataBaseManager db = new DataBaseManager();

            string encryptedPassword = db.GetEncryptedPasswordById(id);

            if (encryptedPassword ==  null)
            {
                MessageBox.Show("Password not found.");
                return;
            }

            string decryptedPassword = EncryptionHelper.Decrypt(encryptedPassword);

            MessageBox.Show(decryptedPassword, "Password");
        }

        private void PasswordManagerMain_Load(object sender, EventArgs e)
        {
            MaskPasswords();
        }

        private void LoadPasswords()
        {
            dataGridViewPasswords.DataSource = passwordRepository.GetPasswords(); // Get the passwords from the db and show it
        }

        private void MaskPasswords()
        {
            LoadPasswords();
            foreach (DataGridViewRow row in dataGridViewPasswords.Rows)
            {
                if (row.IsNewRow) continue;

                row.Cells["PasswordEncrypted"].Value = "*******";
            }
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (dataGridViewPasswords.SelectedRows.Count == 0) // If a row has not been selected give feedback
            {
                MessageBox.Show($"Please select a row");
                return;
            }
            // If it's selected delete the row from the database
            DataGridViewRow row = dataGridViewPasswords.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["Id"].Value);
            passwordRepository.DeletePassword(id);
            MaskPasswords();
        }
    }
}
