using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SellBread
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            InitializeComboBox();

        }
        private void InitializeComboBox()
        {
            // Setup for combobox
            cbRole.Items.Add("Admin");
            cbRole.Items.Add("Warehouse Manager");
            cbRole.Items.Add("Sale");
            // Set the selected index to the first item of the array (Admin)
            cbRole.SelectedIndex = 0;
        }

        private void cbRole_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void InitializeCombobox()
        {
            // Setup for combobox
            cbRole.Items.Add("Admin");
            cbRole.Items.Add("Warehouse Manager");
            cbRole.Items.Add("Sale");
            // Set the selected index to the first item of the array (Admin)
            cbRole.SelectedIndex = 0;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            
        }

        private void ClearData()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            cbRole.SelectedIndex = 0;
            txtUsername.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }
        private bool ValidateData(string username, string password, string role)
        {
            bool isValid = true;

            if (username == null || password == string.Empty )
            {
                MessageBox.Show(
                    "Password cannot be blank",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtUsername.Focus();
                isValid = false;
            }
            else if (role == null || role == string.Empty)
            {
                MessageBox.Show(
                    "No role selected",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtPassword.Focus();
                isValid = false;
            }
            else if (role == null || role == string.Empty)
            {
                MessageBox.Show(
                    "No role selected", 
                    "Warning", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                cbRole.Focus();
                isValid = false;
            }

            return isValid;
        }
        private void RedirectPage(string selectedRole, int employeeId)
        {
            if (selectedRole != null)
            {
                // Redirect user based on role
                if (selectedRole == "Admin")
                {
                    AdminForm adminForm = new AdminForm(selectedRole, employeeId);
                    this.Hide();
                    adminForm.Show();
                }
                else if (selectedRole == "Warehouse Manager")
                {
                    WarehouseManagerForm warehouseManagerForm = new WarehouseManagerForm(selectedRole, employeeId);
                    this.Hide();
                    warehouseManagerForm.Show();
                }
                else if (selectedRole == "Sale")
                {
                    SaleForm saleForm = new SaleForm(selectedRole, employeeId);
                    this.Hide();
                    saleForm.Show();
                }
            }
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            
                // Get user input from the form
                string username = txtUsername.Text;
                string password = txtPassword.Text;
                string role = cbRole.SelectedItem.ToString();

                // Validate the user input
                bool isValid = ValidateData(username, password, role);

                if (isValid)
                {
                    // Open a connection to the database
                    SqlConnection connection = DatabaseConnection.GetConnection();

                    if (connection != null)
                    {
                        // SQL query to retrieve employee information
                        string query = "SELECT EmployeeId FROM Employee WHERE Username = @username AND Password = @password AND AuthorityLevel = @role";

                        // Execute query
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@role", role);
                        SqlDataReader reader = command.ExecuteReader();

                        int employeeId = 0;
                        while (reader.Read())
                        {
                            employeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));
                        }

                        // Check if login is successful
                        if (employeeId > 0)
                        {
                            MessageBox.Show("Login success", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RedirectPage(role, employeeId);  // Directly redirect to the selected form
                        }
                        else
                        {
                            MessageBox.Show("Invalid login credentials", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ClearData();  // Clear input fields
                        }

                        reader.Close();
                        connection.Close();
                    }
                }
            }

        }
    }
        

    

    




           


