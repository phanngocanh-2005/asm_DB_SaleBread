
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SellBread
{
    public partial class SaleForm : Form
    {
        private string authorityLevel;
        private int employeeId;
        public SaleForm(string authorityLevel, int employeeId)
        {
            InitializeComponent();
            this.authorityLevel = authorityLevel;
            this.employeeId = employeeId;
        }




        private void SaleForm_Load(object sender, EventArgs e)
        {
             



        }

        private void btnManageCustomer_Click(object sender, EventArgs e)
        {
            ManageCustomer manageCustomer = new ManageCustomer(authorityLevel, employeeId);
            this.Hide();
            manageCustomer.Show();
        }

        private void btnManageOrder_Click(object sender, EventArgs e)
        {

            OrderHistory orderHistory = new OrderHistory();
            this.Hide();
            orderHistory.Show();
        }



    }

}
