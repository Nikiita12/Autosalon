using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace АИС_салона_по_аренде_автомобилей
{
    public partial class Form2 : Form
    {
        private SQLiteConnection connection;
        private SQLiteDataAdapter adapter;
        private DataTable dt;


        public Form2()
        {
            InitializeComponent();
            connection = new SQLiteConnection("Data Source=E:\\Autosalon.db;Version=3;");
            LoadAutomobileData();
        }

        private void LoadAutomobileData()
        {
            connection.Open();
            string query = "SELECT s.Stamp, s.Title, s.Color, co.Country AS CountryName, c.Price, c.Availability FROM Specifications s JOIN Cars c ON s.ID = c.ID_auto JOIN Country co ON c.ID_country = co.ID";
            adapter = new SQLiteDataAdapter(query, connection);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();
        }
    }
}
