using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace АИС_салона_по_аренде_автомобилей
{
    public partial class Form3 : Form
    {
        private SQLiteConnection connection;
        private SQLiteDataAdapter adapter;
        private DataTable dt;

        public Form3()
        {
            InitializeComponent();
            connection = new SQLiteConnection("Data Source=E:\\Autosalon.db;Version=3;");
            LoadKlientData();
            LoadAutomobileData();
            LoadContractData();
            LoadPersonalData();
            LoadCountryData();
            LoadSpecificationsData();
            // Заполнение comboBox1 данными из таблицы specifications
            string query1 = "SELECT ID, Title FROM specifications";
            SQLiteDataAdapter da1 = new SQLiteDataAdapter(query1, connection);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            comboBox1.DataSource = dt1;
            comboBox1.DisplayMember = "Title"; // Название автомобиля
            comboBox1.ValueMember = "ID";        // ID автомобиля

            // Заполнение comboBox2 данными из таблицы country
            string query2 = "SELECT ID, Country FROM country";
            SQLiteDataAdapter da2 = new SQLiteDataAdapter(query2, connection);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);

            comboBox2.DataSource = dt2;
            comboBox2.DisplayMember = "Country"; // Название страны
            comboBox2.ValueMember = "ID";            // ID страны

            // Заполнение comboBox3 данными из таблицы klients
            string query3 = "SELECT ID, Surname || ' ' || Name || ' ' || LastName AS FullName, Seria, Number FROM klients";
            SQLiteDataAdapter da3 = new SQLiteDataAdapter(query3, connection);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);

            comboBox3.DataSource = dt3;
            comboBox3.DisplayMember = "FullName";
            comboBox3.ValueMember = "ID";

            // Заполнение comboBox4 данными из таблицы personal
            string query4 = "SELECT ID, Surname || ' ' || Name || ' ' || LastName AS FullName FROM personal";
            SQLiteDataAdapter da4 = new SQLiteDataAdapter(query4, connection);
            DataTable dt4 = new DataTable();
            da4.Fill(dt4);

            comboBox4.DataSource = dt4;
            comboBox4.DisplayMember = "FullName";
            comboBox4.ValueMember = "ID";

            // Заполнение comboBox5 данными из таблицы cars и specifications
            string query5 = "SELECT cars.ID, specifications.Title FROM cars JOIN specifications ON cars.ID_auto = specifications.ID";
            SQLiteDataAdapter da5 = new SQLiteDataAdapter(query5, connection);
            DataTable dt5 = new DataTable();
            da5.Fill(dt5);

            comboBox5.DataSource = dt5;
            comboBox5.DisplayMember = "Title";
            comboBox5.ValueMember = "ID";

            // Обработка события изменения выбранного клиента в comboBox3
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;


            comboBox6.Items.Clear();
            comboBox6.Items.Add("Name");
            comboBox6.Items.Add("Surname");
            comboBox6.Items.Add("LastName");
            comboBox6.Items.Add("Seria");
            comboBox6.Items.Add("Number");
            comboBox6.Items.Add("Telephone");

            comboBox7.Items.Clear();
            comboBox7.Items.Add("Name");
            comboBox7.Items.Add("Surname");
            comboBox7.Items.Add("LastName");
            comboBox7.Items.Add("Post");
            comboBox7.Items.Add("Telephone");
            comboBox7.Items.Add("Mail");

            comboBox8.Items.Clear();
            comboBox8.Items.Add("Stamp");
            comboBox8.Items.Add("Title");
            comboBox8.Items.Add("Color");
        }

        private void LoadKlientData()
        {
            connection.Open();
            string query = "SELECT * FROM Klients";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();
        }

        private void LoadAutomobileData()
        {
            connection.Open();
            string query = "SELECT * FROM Cars";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView2.DataSource = dt;
            connection.Close();
        }

        private void LoadContractData()
        {
            connection.Open();
            string query = "SELECT * FROM Contract";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView3.DataSource = dt;
            connection.Close();
        }

        private void LoadPersonalData()
        {
            connection.Open();
            string query = "SELECT * FROM Personal";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView4.DataSource = dt;
            connection.Close();
        }

        private void LoadCountryData()
        {
            connection.Open();
            string query = "SELECT * FROM Country";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView6.DataSource = dt;
            connection.Close();
        }

        private void LoadSpecificationsData()
        {
            connection.Open();
            string query = "SELECT * FROM Specifications";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView5.DataSource = dt;
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Klients (Surname, Name, LastName, Seria, Number, Telephone) VALUES (@Surname, @Name, @LastName, @Seria, @Number, @Telephone)";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Surname", textBox2.Text);
            cmd.Parameters.AddWithValue("@Name", textBox3.Text);
            cmd.Parameters.AddWithValue("@LastName", textBox4.Text);
            cmd.Parameters.AddWithValue("@Seria", textBox5.Text);
            cmd.Parameters.AddWithValue("@Number", textBox6.Text);
            cmd.Parameters.AddWithValue("@Telephone", textBox7.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            LoadKlientData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string clientId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "UPDATE Klients SET Surname=@Surname, Name=@Name, LastName=@LastName, Seria=@Seria, Number=@Number, Telephone=@Telephone";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", clientId);
                cmd.Parameters.AddWithValue("@Surname", textBox2.Text);
                cmd.Parameters.AddWithValue("@Name", textBox3.Text);
                cmd.Parameters.AddWithValue("@LastName", textBox4.Text);
                cmd.Parameters.AddWithValue("@Seria", textBox5.Text);
                cmd.Parameters.AddWithValue("@Number", textBox6.Text);
                cmd.Parameters.AddWithValue("@Telephone", textBox7.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadKlientData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для редактирования.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string clientId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "DELETE FROM Klients WHERE ID=@Id";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", clientId);
                connection.Open(); cmd.ExecuteNonQuery();
                connection.Close();
                LoadKlientData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для удаления.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO cars (ID_auto, ID_country, Price, Availability) VALUES (@ID_auto, @ID_country, @Price, @Availability)";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@ID_auto", comboBox1.SelectedValue);
            cmd.Parameters.AddWithValue("@ID_country", comboBox2.SelectedValue);
            cmd.Parameters.AddWithValue("@Price", textBox11.Text);
            cmd.Parameters.AddWithValue("@Availability", textBox12.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            LoadAutomobileData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                string carId = dataGridView2.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "UPDATE cars SET ID_auto=@ID_auto, ID_country=@ID_country, Price=@Price, Availability=@Availability WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", carId);
                cmd.Parameters.AddWithValue("@ID_auto", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@ID_country", comboBox2.SelectedValue);
                cmd.Parameters.AddWithValue("@Price", textBox11.Text);
                cmd.Parameters.AddWithValue("@Availability", textBox12.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadAutomobileData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите машину для редактирования.");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                string carId = dataGridView2.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "DELETE FROM cars WHERE ID=@Id";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", carId);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadAutomobileData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите машину для удаления.");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Personal (Surname, Name, LastName, Post, Telephone, Mail) VALUES (@Surname, @Name, @LastName, @Post, @Telephone, @Mail)";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Surname", textBox24.Text);
            cmd.Parameters.AddWithValue("@Name", textBox25.Text);
            cmd.Parameters.AddWithValue("@LastName", textBox26.Text);
            cmd.Parameters.AddWithValue("@Post", textBox27.Text);
            cmd.Parameters.AddWithValue("@Telephone", textBox28.Text);
            cmd.Parameters.AddWithValue("@Mail", textBox29.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            LoadPersonalData();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (dataGridView4.SelectedRows.Count > 0)
            {
                string personalId = dataGridView4.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "UPDATE Personal SET Surname=@Surname, Name=@Name, LastName=@LastName, Post=@Post, Telephone=@Telephone, Mail=@Mail";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", personalId);
                cmd.Parameters.AddWithValue("@Surname", textBox24.Text);
                cmd.Parameters.AddWithValue("@Name", textBox25.Text);
                cmd.Parameters.AddWithValue("@LastName", textBox26.Text);
                cmd.Parameters.AddWithValue("@Post", textBox27.Text);
                cmd.Parameters.AddWithValue("@Telephone", textBox28.Text);
                cmd.Parameters.AddWithValue("@Mail", textBox29.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadPersonalData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для редактирования.");
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (dataGridView4.SelectedRows.Count > 0)
            {
                string clientId = dataGridView4.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "DELETE FROM Personal WHERE ID=@Id";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", clientId);
                connection.Open(); cmd.ExecuteNonQuery();
                connection.Close();
                LoadPersonalData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для удаления.");
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Specifications (Stamp, Title, Color) VALUES (@Stamp, @Title, @Color)";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Stamp", textBox31.Text);
            cmd.Parameters.AddWithValue("@Title", textBox32.Text);
            cmd.Parameters.AddWithValue("@Color", textBox33.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            LoadSpecificationsData();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (dataGridView5.SelectedRows.Count > 0)
            {
                string SpId = dataGridView5.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "UPDATE Specifications SET Stamp=@Stamp, Title=@Title, Color=@Color";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", SpId);
                cmd.Parameters.AddWithValue("@Stamp", textBox31.Text);
                cmd.Parameters.AddWithValue("@Title", textBox32.Text);
                cmd.Parameters.AddWithValue("@Color", textBox33.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadSpecificationsData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автомобиль для редактирования.");
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (dataGridView5.SelectedRows.Count > 0)
            {
                string SpId = dataGridView5.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "DELETE FROM Specifications WHERE ID=@Id";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", SpId);
                connection.Open(); cmd.ExecuteNonQuery();
                connection.Close();
                LoadSpecificationsData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автомобиль для удаления.");
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Country (Country) VALUES (@Country)";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Country", textBox35.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            LoadCountryData();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (dataGridView6.SelectedRows.Count > 0)
            {
                string CoId = dataGridView6.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "UPDATE Country SET Country=@Country";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", CoId);
                cmd.Parameters.AddWithValue("@Country", textBox35.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadCountryData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите страну для редактирования.");
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (dataGridView6.SelectedRows.Count > 0)
            {
                string CoId = dataGridView6.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "DELETE FROM Country WHERE ID=@Id";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", CoId);
                connection.Open(); cmd.ExecuteNonQuery();
                connection.Close();
                LoadCountryData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите страну для удаления.");
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRowView selectedRow = (DataRowView)comboBox3.SelectedItem;
            textBox17.Text = selectedRow["Seria"].ToString();
            textBox20.Text = selectedRow["Number"].ToString();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO contract (ID_klient, ID_car, ID_personal, Date, Summa, SeriaKlient, NumberKlient, BeginArenda, EndArenda) VALUES (@ID_klient, @ID_car, @ID_personal, @Date, @Summa, @SeriaKlient, @NumberKlient, @BeginArenda, @EndArenda)";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@ID_klient", comboBox3.SelectedValue);
            cmd.Parameters.AddWithValue("@ID_car", comboBox5.SelectedValue);
            cmd.Parameters.AddWithValue("@ID_personal", comboBox4.SelectedValue);
            cmd.Parameters.AddWithValue("@Date", textBox19.Text);
            cmd.Parameters.AddWithValue("@Summa", textBox18.Text);
            cmd.Parameters.AddWithValue("@SeriaKlient", textBox17.Text);
            cmd.Parameters.AddWithValue("@NumberKlient", textBox20.Text);
            cmd.Parameters.AddWithValue("@BeginArenda", textBox21.Text);
            cmd.Parameters.AddWithValue("@EndArenda", textBox22.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            LoadContractData();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0)
            {
                string contractId = dataGridView3.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "UPDATE contract SET ID_klient=@ID_klient, ID_car=@ID_car, ID_personal=@ID_personal, Date=@Date, Summa=@Summa, SeraiKlient=@SeraiKlient, Numberklients=@Numberklients, BeginArenda=@BeginArenda, EndArenda=@EndArenda WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", contractId);
                cmd.Parameters.AddWithValue("@ID_klient", comboBox3.SelectedValue);
                cmd.Parameters.AddWithValue("@ID_car", comboBox5.SelectedValue);
                cmd.Parameters.AddWithValue("@ID_personal", comboBox4.SelectedValue);
                cmd.Parameters.AddWithValue("@Date", textBox19.Text);
                cmd.Parameters.AddWithValue("@Summa", textBox18.Text);
                cmd.Parameters.AddWithValue("@SeraiKlient", textBox17.Text);
                cmd.Parameters.AddWithValue("@Numberklients", textBox20.Text);
                cmd.Parameters.AddWithValue("@BeginArenda", textBox21.Text);
                cmd.Parameters.AddWithValue("@EndArenda", textBox22.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadContractData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите контракт для редактирования.");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0)
            {
                string contractId = dataGridView3.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "DELETE FROM contract WHERE ID=@Id";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", contractId);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadContractData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите контракт для удаления.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string query;
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                // Если поле поиска пустое, загрузить все данные
                query = "SELECT * FROM Klients";
            }
            else
            {
                //Поиск по выбранному критерию
                string selectedField = comboBox6.SelectedItem.ToString();
                string searchText = textBox1.Text;
                query = $"SELECT * FROM Klients WHERE {selectedField} LIKE '%{searchText}%'";
            }
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string query;
            if (string.IsNullOrEmpty(textBox8.Text))
            {
                // Если поле поиска пустое, загрузить все данные
                query = "SELECT * FROM Personal";
            }
            else
            {
                //Поиск по выбранному критерию
                string selectedField = comboBox7.SelectedItem.ToString();
                string searchText = textBox8.Text;
                query = $"SELECT * FROM Personal WHERE {selectedField} LIKE '%{searchText}%'";
            }
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView4.DataSource = dt;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string query;
            if (string.IsNullOrEmpty(textBox9.Text))
            {
                // Если поле поиска пустое, загрузить все данные
                query = "SELECT * FROM Specifications";
            }
            else
            {
                //Поиск по выбранному критерию
                string selectedField = comboBox8.SelectedItem.ToString();
                string searchText = textBox9.Text;
                query = $"SELECT * FROM Specifications WHERE {selectedField} LIKE '%{searchText}%'";
            }
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView5.DataSource = dt;
        }
    }
}
