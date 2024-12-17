using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SellBread
{
    public partial class ManageProduct : Form

    {



        // Variable to store the selected product
        private int productId;

        // Variable to store the authority level of user, so that we are able to navigate back
        private string authorityLevel;

        // Variable to store logged in userId
        private int userId;

        public void InitializeComboBox()
        {
            // Add items to the authority level combo box
            cbCategory.Items.Add(" Sweet Bread");
            cbCategory.Items.Add("Salty Bread");
            cbCategory.Items.Add("Vegetarian bread");

            // Set the selected item to "Admin" by default
            cbCategory.SelectedIndex = 0;
        }

        public ManageProduct(string authorityLevel, int userId)
        {
            this.authorityLevel = authorityLevel;
            this.userId = userId;
            InitializeComponent();
            productId = 0;
        }

        private void Manage_Product_Load(object sender, EventArgs e)
        {
            // Bind data to the data grid view
            LoadProductData();

            // Bind data to the combo box
            LoadCategoryCombobox();

            // Set button status to ensure a good user experience
            ChangeButtonStatus(false);
        }

        private void LoadCategoryCombobox()
        {
            SqlConnection connection = DatabaseConnection.GetConnection();

            if (connection != null)
            {
                connection.Open();
                string query = "SELECT CategoryID, CategoryName FROM Category";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                cbCategory.DataSource = dataTable;
                cbCategory.DisplayMember = "CategoryName";
                cbCategory.ValueMember = "CategoryID";

                connection.Close();

            }
        }

        private bool ValidateData(string productCode,
                          string productName,
                          string productPrice,
                          string productQuantity)
        {
            double temp;
            int temp2;

            if (string.IsNullOrEmpty(productName)) { return false; }
            if (string.IsNullOrEmpty(productPrice)) { return false; }
            if (double.TryParse(productPrice, out temp) == false) { return false; }
            if (string.IsNullOrEmpty(productQuantity)) { return false; }
            return int.TryParse(productQuantity, out temp2);
        }

        private void UploadFile(string filter, string path)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Optional: Set file filters (e.g., only allow certain file types)
            // Examples:
            // Text Files (*.txt)|*.txt
            // Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png
            // You can combine these options to upload multiple type of data
            openFileDialog.Filter = filter;

            // Set title of the dialog
            openFileDialog.Title = "Select a file to upload";

            // Show the dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the path of the selected file
                string sourceFilePath = openFileDialog.FileName;

                // Specify the target path relative to the project directory
                string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");

                // Combine the target directory with the file name to get the destination path
                string targetFilePath = Path.Combine(targetDirectory, Path.GetFileName(sourceFilePath));

                try
                {
                    // Ensure the target directory exists
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    // Copy the file to the target directory
                    File.Copy(sourceFilePath, targetFilePath, overwrite: true);

                    // Inform the user
                    MessageBox.Show("File uploaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the file upload
                    MessageBox.Show("Error uploading file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadProductData()
        {
            SqlConnection connection = DatabaseConnection.GetConnection();

            if (connection != null)
            {
                connection.Open();
                string query = "SELECT * FROM Product";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dtgProduct.DataSource = dataTable;
                connection.Close();
            }
        }
        private void ChangeButtonStatus(bool buttonStatus)
        {
            // When an employee is selected, the add button will be disabled
            // and the update, delete, and clear buttons will be enabled, and vice versa
            btnUpdate.Enabled = buttonStatus;
            btnDelete.Enabled = buttonStatus;
            btnClear.Enabled = buttonStatus;
            btnAdd.Enabled = !buttonStatus;
        }
        private void FlushProductId()
        {
            this.productId = 0;
            ChangeButtonStatus(false);
        }

        private void ClearData()
        {
            FlushProductId();
            txtProductCode.Text = string.Empty;
            txtProductName.Text = string.Empty;
            txtProductImg.Text = string.Empty;
            txtProductPrice.Text = string.Empty;
            txtProductQuantity.Text = string.Empty;
            txtSearch.Text = string.Empty;
        }

        private void AddProduct()
        {
            // Open connection by call the GetConnection function in DatabaseConnection class
            SqlConnection connection = DatabaseConnection.GetConnection();

            // Check connection
            if (connection != null)
            {
                // Open the connection
                connection.Open();

                // Get product data from textboxes
                string productCode = txtProductCode.Text;
                string productName = txtProductName.Text;
                string productImg = txtProductImg.Text;
                string productPrice = txtProductPrice.Text;
                string productQuantity = txtProductQuantity.Text;
                int categoryId = Convert.ToInt32(cbCategory.SelectedValue);

                // Validate data
                if (ValidateData(productCode, productName, productPrice, productQuantity))
                {
                    // Declare query and variables to populate query
                    string sql = "INSERT INTO Product VALUES (@productCode, @productName, @productPrice, @productQuantity, @productImg, @categoryId)";
                    SqlCommand command = new SqlCommand(sql, connection);

                    // Add parameters
                    command.Parameters.AddWithValue("@productCode", productCode);
                    command.Parameters.AddWithValue("@productName", productName);
                    command.Parameters.AddWithValue("@productPrice", 1 * Convert.ToDouble(productPrice));
                    command.Parameters.AddWithValue("@productQuantity", Convert.ToInt32(productQuantity));
                    command.Parameters.AddWithValue("@productImg", productImg);
                    command.Parameters.AddWithValue("@categoryId", categoryId);

                    // Execute query and get the result
                    int result = command.ExecuteNonQuery();

                    // Check the result
                    if (result > 0)
                    {
                        MessageBox.Show("Successfully add new product", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearData();
                        LoadProductData();
                    }
                    else
                    {
                        MessageBox.Show("Cannot add new product", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Close the connection
                connection.Close();
            }
        }

        private void UpdateProduct()
        {
            // Open connection by call the GetConnection function in DatabaseConnection class
            SqlConnection connection = DatabaseConnection.GetConnection();

            // Check connection
            if (connection != null)
            {
                // Open the connection
                connection.Open();

                // Get product data from textboxes
                string productCode = txtProductCode.Text;
                string productName = txtProductName.Text;
                string productImg = txtProductImg.Text;
                string productPrice = txtProductPrice.Text;
                string productQuantity = txtProductQuantity.Text;
                int categoryId = Convert.ToInt32(cbCategory.SelectedValue);

                // Validate data
                if (ValidateData(productCode, productName, productPrice, productQuantity))
                {
                    // Declare query and variables to populate query
                    string sql = "UPDATE Product SET ProductCode = @productCode, ProductName = @productName, " +
                                "ProductImage = @productImg, ProductPrice = @productPrice, " +
                                "InventoryQuantity = @productQuantity, CategoryID = @categoryId " +
                                "WHERE ProductID = @productId";
                    SqlCommand command = new SqlCommand(sql, connection);

                    // Add parameters
                    command.Parameters.AddWithValue("@productCode", productCode);
                    command.Parameters.AddWithValue("@productName", productName);
                    command.Parameters.AddWithValue("@productImg", productImg);
                    command.Parameters.AddWithValue("@productPrice", Convert.ToDouble(productPrice));
                    command.Parameters.AddWithValue("@productQuantity", Convert.ToInt32(productQuantity));
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                    command.Parameters.AddWithValue("@productId", this.productId);

                    // Execute query and get the result
                    int result = command.ExecuteNonQuery();

                    // Check the result
                    if (result > 0)
                    {
                        MessageBox.Show("Successfully update product", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearData();
                        LoadProductData();
                    }
                    else
                    {
                        MessageBox.Show("Cannot update product", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Close the connection
                connection.Close();
            }
        }

        private void DeleteProduct()
        {
            // Ask for confirmation
            DialogResult dialogResult = MessageBox.Show("Do you want to delete the product?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                // Check if product in any order
                // If it save, deny this action because this can cause exception while running
                if (IsProductInOrder(this.productId))
                {
                    MessageBox.Show("Product is in another order\nCannot delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Open connection by call the GetConnection function in DatabaseConnection class
                    SqlConnection connection = DatabaseConnection.GetConnection();

                    // Check connection
                    if (connection != null)
                    {
                        // Open the connection
                        connection.Open();

                        // declare query
                        string sql = "DELETE FROM Product WHERE ProductID = @productId";

                        // declare sqlcommand variable to manipulate query
                        SqlCommand command = new SqlCommand(sql, connection);

                        // add params
                        command.Parameters.AddWithValue("@productId", this.productId);

                        // execute query and get the result
                        int result = command.ExecuteNonQuery();

                        // check result
                        if (result > 0)
                        {
                            MessageBox.Show("Successfully delete product", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearData();
                            LoadProductData();
                        }
                        else
                        {
                            MessageBox.Show("Cannot delete product", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // close the connection
                        connection.Close();
                    }
                }
            }
        }

        private bool IsProductInOrder(int productId)
        {
            // Open connection by call the GetConnection function in DatabaseConnection class
            SqlConnection connection = DatabaseConnection.GetConnection();

            // Check connection
            if (connection != null)
            {
                // Open the connection
                connection.Open();

                // declare query to get number of record have productId equal productId
                string sql = "SELECT COUNT(*) FROM OrderDetail WHERE ProductID = @productId";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@productId", productId);

                // execute query and get the result
                int result = (int)command.ExecuteScalar();
                connection.Close();

                return result > 0;
            }

            return false;
        }

        private void SearchProduct(string search)
        {
            // If the search string is empty, load all products
            if (string.IsNullOrEmpty(search))
            {
                LoadProductData();
            }
            else
            {
                // Open connection by call the GetConnection function in DatabaseConnection class
                SqlConnection connection = DatabaseConnection.GetConnection();

                // Check connection
                if (connection != null)
                {
                    // Open the connection
                    connection.Open();

                    // Declare query to get the product, product code, category name, price, 
                    // inventory quantity, product image, category name
                    string sql = "SELECT p.ProductID, p.ProductCode, p.ProductName, p.Price, " +
                                "p.InventoryQuantity, p.ProductImage, c.CategoryName " +
                                "FROM Product p " +
                                "INNER JOIN Category c ON p.CategoryID = c.CategoryID " +
                                "WHERE p.ProductCode LIKE @search " +
                                "OR p.ProductName LIKE @search " +
                                "OR c.CategoryName LIKE @search";

                    // Initialize SqlDataAdapter to translate query result to a data table
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                    // Add params to query
                    adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");

                    // Initialize data table
                    DataTable data = new DataTable();

                    // Fill the data table with data queried from database
                    adapter.Fill(data);

                    // Set the data source for data table
                    dtgProduct.DataSource = data;

                    // Close the connection
                    connection.Close();
                }
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            UploadFile("Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png", @"C:\Uploads"); ;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddProduct();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateProduct();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteProduct();
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

        private void dtgProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            // Set row index based on current cell clicked
            int rowIndex = dtgProduct.CurrentCell.RowIndex;

            // Check index >= 0 && currentRow.IsNewRow
            if (rowIndex >= 0 && !dtgProduct.Rows[rowIndex].IsNewRow)
            {
                // Get value of each cell based on row index 
                // (use this query to execute in SSMS to imagine the datagridview:
                // SELECT ProductID, ProductCode, ProductName, Price, InventoryQuantity, ProductImage, ProductCategoryName
                // FROM Product)

                // Change button status to true (update, delete, clear is enable when productId is assigned with value > 0)
                ChangeButtonStatus(true);

                // Get the Product Code (index is 1)
                txtProductCode.Text = dtgProduct.Rows[rowIndex].Cells[1].Value.ToString();

                // Get the Product Name (index is 2)
                txtProductName.Text = dtgProduct.Rows[rowIndex].Cells[2].Value.ToString();

                // Get the Product Price (index is 3)
                txtProductPrice.Text = dtgProduct.Rows[rowIndex].Cells[3].Value.ToString();

                // Get the Product Quantity (index is 4)
                txtProductQuantity.Text = dtgProduct.Rows[rowIndex].Cells[4].Value.ToString();

                // Get the Img URL (index is 5)
                txtProductImg.Text = dtgProduct.Rows[rowIndex].Cells[5].Value.ToString();

                // Get the CategoryName (index is 6) and check in combobox to select the equal value
                string categoryName = dtgProduct.Rows[rowIndex].Cells[6].Value.ToString();
                for (int i = 0; i < cbCategory.Items.Count; i++)
                {
                    if (cbCategory.Items[i].ToString() == categoryName)
                    {
                        cbCategory.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            SearchProduct(search);
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            
           

        }



        private void gbProductInformation_Enter(object sender, EventArgs e)
        {

        }


    }

        
    
}
