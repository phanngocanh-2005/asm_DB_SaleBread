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
    public partial class WarehouseManagerForm : Form
    {
        private string authorityLevel;
        private int employeeId;
        public WarehouseManagerForm(string authorityLevel, int employeeId)
        {
            InitializeComponent();
            this.authorityLevel = authorityLevel;
            this.employeeId = employeeId;
        }

        private void btnManageProduct_Click(object sender, EventArgs e)
        {
            // Create a new instance of the ManageCategory form and show it
            ManageProduct manageProduct = new ManageProduct(this.authorityLevel, this.employeeId);
            this.Hide();
            manageProduct.Show();
        }

        private void WarehouseManagerForm_Load(object sender, EventArgs e)
        {

        }
    }
}
