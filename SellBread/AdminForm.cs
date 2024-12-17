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
    public partial class AdminForm : Form
    {
        private string authorityLevel;
        private int employeeId;
        private string employeePosition;
         
        public AdminForm(string authorityLevel, int employeeId)
        {
            InitializeComponent();
            this.authorityLevel = authorityLevel;
            this.employeeId = employeeId;
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {

        }

        private void btnManageEmployee_Click(object sender, EventArgs e)
        {
            ManageEmployee manageEmployee = new ManageEmployee(employeePosition);
            this.Hide();
            manageEmployee.Show();
        }

        private void btnManageProduct_Click(object sender, EventArgs e)
        {
            // Create a new instance of the ManageCategory form and show it
            ManageProduct manageProduct = new ManageProduct(this.authorityLevel, this.employeeId);
            this.Hide();
            manageProduct.Show();
        }

        private void btnManageCategory_Click(object sender, EventArgs e)
        {

        }

        private void btnManageOrder_Click(object sender, EventArgs e)
        {

        }

        private void btnManageImport_Click(object sender, EventArgs e)
        {

        }

        private void btnManageCustomer_Click(object sender, EventArgs e)
        {
            ManageCustomer manageCustomer = new ManageCustomer(authorityLevel, employeeId);
            this.Hide();
            manageCustomer.Show();
        }
    }
 }

