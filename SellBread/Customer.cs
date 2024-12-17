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
    public partial class ManageCustomer : Form
    {
        private int customerId;
        private int employeeId;
        private string authorityLevel;
        private int userId;
        public ManageCustomer(string authorityLevel, int employeeId)
        {
            InitializeComponent();
            this.employeeId = employeeId;
            this.authorityLevel = authorityLevel;
        }

        private void ClearData()
        {
            FlushCustomerId();
            txtCustomerCode.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtPhonenumber.Text = string.Empty;
            txtCustomerAddress.Text = string.Empty;
            txtCustomerCode.Focus();
           
        }

        private void ChangeButtonStatus(bool buttonStatus)
        {
            // When customer is selected, button add will be disabled
            // button Update, Delete & Clear will be enabled and vice versa

            btnUpdate.Enabled = buttonStatus;
            btnDelete.Enabled = buttonStatus;
            btnClear.Enabled = buttonStatus;
            btnAdd.Enabled = !buttonStatus;
        }
        private bool ValidateData(string customerCode, string customerName, string customerAddress, string phoneNumber)
        {
            // Validate user input data
            bool isValid = true; // Initially assume all data is valid

            // Check each field
            if (string.IsNullOrEmpty(customerCode))
            {
                // Show error message if customer code is empty
                MessageBox.Show("Customer Code cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
                txtCustomerCode.Focus();
            }
            else if (string.IsNullOrEmpty(customerName))
            {
                // Show error message if customer name is empty
                MessageBox.Show("Customer Name cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
                txtCustomerName.Focus();
            }
            else if (string.IsNullOrEmpty(customerAddress))
            {
                // Show error message if customer address is empty
                MessageBox.Show("Customer Address cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
                lblCustomerAddress.Focus();
            }
            else if (string.IsNullOrEmpty(phoneNumber))
            {
                // Show error message if customer phone number is empty
                MessageBox.Show("Customer Phone Number cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
                txtPhonenumber.Focus();
            }

            return isValid;
        }
        private void FlushCustomerId()
        {
            // Flush customerID value to check button and setup status for buttons
            this.customerId = 0;
            ChangeButtonStatus(false);
        }
        private void LoadCustomerData()
        {
            // Open connection by call the GetConnection function in DatabaseConnection class
            SqlConnection connection = DatabaseConnection.GetConnection();

            // Check connection
            if (connection != null)
            {
                // Open the connection
                connection.Open();

                // Declare query to the database
                string query = "SELECT * FROM Customer";

                // Initialize SqlDataAdapter to translate query result to a data table
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                // Initialize data table
                DataTable table = new DataTable();

                // Fill the data table by data queried from the database
                adapter.Fill(table);

                // Set the datasource of data gridview by the dataset
                dtgCustomer.DataSource = table;

                // close the connection
                connection.Close();
            }
        }
        private bool CheckUserExistence(int customerId)
        {
            bool isExist = false;
            SqlConnection connection = DatabaseConnection.GetConnection();

            if (connection != null)
            {
                connection.Open();
                string query = "SELECT * FROM Customer WHERE CustomerID = @customerId";

                // Declare SqlCommand variable to add parameters to query and execute it
                SqlCommand command = new SqlCommand(query, connection);

                // Add parameters
                command.Parameters.AddWithValue("@customerId", customerId);

                // Declare SqlDataReader variable to read retrieved data
                SqlDataReader reader = command.ExecuteReader();

                // Check if reader has row (query success and return one row show user information)
                isExist = reader.HasRows;

                connection.Close();
            }

            return isExist;
        }
        private void AddCustomer(string customerCode, string customerName, string customerAddress, string phoneNumber)
        {
            SqlConnection connection = DatabaseConnection.GetConnection();

            if (connection != null)
            {
                connection.Open();
                string query = "INSERT INTO Customer (CustomerCode, CustomerName, PhoneNumber, Address) " +
                               "VALUES (@customerCode, @customerName, @phoneNumber, @customerAddress)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@customerCode", customerCode);
                command.Parameters.AddWithValue("@customerName", customerName);
                command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                command.Parameters.AddWithValue("@customerAddress", customerAddress);
                int result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    // Success message
                }
                else
                {
                    // Error message
                }
                connection.Close();

                // Clear all user input data and flush customerID
                ClearData();
                LoadCustomerData();
            }
        }
        private void updateCustomer(int customerId, string customerCode, string customerName, string customerAddress, string phoneNumber)
        {
            SqlConnection connection = DatabaseConnection.GetConnection();

            if (connection != null)
            {
                connection.Open();
                string query = "UPDATE Customer SET " +
                               "CustomerCode = @customerCode, " +
                               "CustomerName = @customerName, " +
                               "Address = @customerAddress, " +
                               "PhoneNumber = @phoneNumber " +
                               "WHERE CustomerID = @customerId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@customerCode", customerCode);
                command.Parameters.AddWithValue("@customerName", customerName);
                command.Parameters.AddWithValue("@address", customerAddress);
                command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                command.Parameters.AddWithValue("@customerId", customerId);
                int result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    // Success message
                }
                else
                {
                    // Error message
                }
                connection.Close();

                // Clear all user input data and flush customerID
                ClearData();
                LoadCustomerData();
            }
        }
        private void DeleteCustomer(int customerId)
        {
            // Initialize database connection by call GetConnection function from DatabaseConnection class
            SqlConnection connection = DatabaseConnection.GetConnection();

            if (connection != null)
            {
                connection.Open();
                string deleteOrderDetailQuery = "DELETE FROM OrderDetail WHERE OrderID IN " +
                                                "(SELECT OrderID FROM Orders WHERE CustomerID = @customerId)";
                SqlCommand command = new SqlCommand(deleteOrderDetailQuery, connection);
                command.Parameters.AddWithValue("@customerId", customerId);
                command.ExecuteNonQuery(); // This step is used to ensure no exception occur

                string deleteOrderQuery = "DELETE FROM Orders WHERE CustomerID = @customerId";
                command = new SqlCommand(deleteOrderQuery, connection);
                command.Parameters.AddWithValue("@customerId", customerId);
                command.ExecuteNonQuery(); // This step is used to ensure no exception occur

                string deleteCustomerQuery = "DELETE FROM Customer WHERE CustomerID = @customerId";
                command = new SqlCommand(deleteCustomerQuery, connection);
                command.Parameters.AddWithValue("@customerId", customerId);
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    // Success message
                }
                else
                {
                    // Error message
                }
                connection.Close();

                // Clear all user input data and flush customerID
                ClearData();
                LoadCustomerData();
            }
        }
        private void SearchCustomer(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                LoadCustomerData();
            }
            else
            {
                // Open connection by call the GetConnection function in DatabaseConnection
                SqlConnection connection = DatabaseConnection.GetConnection();

                if (connection != null)
                {
                    connection.Open();
                    string query = "SELECT * FROM Customer WHERE CustomerCode LIKE @search OR CustomerName LIKE @search OR PhoneNumber LIKE @search OR Address LIKE @search";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dtgCustomer.DataSource = table;
                    connection.Close();
                }
            }
        }

        private void ManageCustomer_Load(object sender, EventArgs e)
        {
            // Load data from database to the data gridview
            LoadCustomerData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Get data from user input
            string customerCode = txtCustomerCode.Text;
            string customerName = txtCustomerName.Text;
            string customerAddress = txtCustomerAddress.Text;
            string phoneNumber = txtPhonenumber.Text;

            // Validate data
            bool isValid = ValidateData(customerCode, customerName, customerAddress, phoneNumber);

            if (isValid)
            {
                AddCustomer(customerCode, customerName, customerAddress, phoneNumber);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Check customerId
            if (customerId > 8)
            {
                // Check user existence
                bool isUserExist = CheckUserExistence(customerId);
                if (isUserExist)
                {
                    // Get data from user input
                    string customerCode = txtCustomerCode.Text;
                    string customerName = txtCustomerName.Text;
                    string customerAddress = txtCustomerAddress.Text;
                    string phoneNumber = txtPhonenumber.Text;

                    // Validate data
                    bool isValid = ValidateData(customerCode, customerName, customerAddress, phoneNumber);
                    if (isValid)
                    {
                        updateCustomer(customerId, customerCode, customerName, customerAddress, phoneNumber);
                    }
                }
                else
                {
                    // Show error message
                    MessageBox.Show("No customer found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Check customer ID
            if (customerId > 8)
            {
                // Ask user for confirmation
                DialogResult result = MessageBox.Show("Do you want to delete this customer with all related data?",
                                                      "Warning",
                                                      MessageBoxButtons.OKCancel,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    // Check if customer exists
                    bool isUserExist = CheckUserExistence(customerId);
                    if (isUserExist)
                    {
                        DeleteCustomer(customerId);
                    }
                    else
                    {
                        MessageBox.Show("No customer found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            switch (authorityLevel)
            {
                case "Admin":
                    AdminForm adminForm = new AdminForm(this.authorityLevel, this.userId);
                    this.Hide();
                    adminForm.Show();
                    break;
                case "Warehouse Manager":
                    WarehouseManagerForm warehouseManagerForm = new WarehouseManagerForm(this.authorityLevel, this.userId);
                    this.Hide();
                    warehouseManagerForm.Show();
                    break;
                case "Sale":
                    SaleForm saleForm = new SaleForm(this.authorityLevel, this.userId);
                    this.Hide();
                    saleForm.Show();
                    break;
                default:
                    break;
            }
        }

        

        private void dtgCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = dtgCustomer.CurrentCell.RowIndex;
            if (index > -1)
            {
                customerId = (int)dtgCustomer.Rows[index].Cells[0].Value;
                txtCustomerCode.Text = dtgCustomer.Rows[index].Cells[1].Value.ToString();
                txtCustomerName.Text = dtgCustomer.Rows[index].Cells[2].Value.ToString();
                txtPhonenumber.Text = dtgCustomer.Rows[index].Cells[3].Value.ToString();
                txtCustomerAddress.Text = dtgCustomer.Rows[index].Cells[4].Value.ToString();
                ChangeButtonStatus(true);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            SearchCustomer(search);
        }
    }
}
