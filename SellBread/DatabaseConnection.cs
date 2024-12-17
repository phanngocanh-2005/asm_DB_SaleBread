using Microsoft.Data.SqlClient;
using System;
using System.Windows.Forms;

namespace SellBread
{
    public class DatabaseConnection
    {
        private static string _conectionString = "Data Source=DELL\\SQLEXPRESS;Initial Catalog=Bread;Integrated Security=True;Trust Server Certificate=True";
        public static SqlConnection GetConnection()
        {
            // khoi tạo biến sql connection
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(_conectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Errox while connecting to the database",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
            return connection;

        }
    }
}








