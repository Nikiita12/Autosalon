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
using АИС_салона_по_аренде_автомобилей;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace АИС_салона_по_аренде_автомобилей
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            LoadDeletedContractData();
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Клиент");
            comboBox1.Items.Add("Персонал");
            comboBox1.Items.Add("Автомобиль");
            comboBox1.Items.Add("Дата");
            comboBox1.Items.Add("Сумма");
            comboBox1.Items.Add("Начало аренды");
            comboBox1.Items.Add("Конец аренды");
            comboBox1.SelectedIndex = 0;
        }

        public void LoadDeletedContractData()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;"))
            {
                connection.Open();
                string query = @"
        SELECT
            ID,
            Klient AS 'Клиент',
            Personal AS 'Персонал',
            Avto AS 'Автомобиль',
            Date AS 'Дата',
            Summa AS 'Сумма',
            BeginArenda AS 'Начало аренды',
            EndArenda AS 'Конец аренды'
        FROM 
            DeletedContracts";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.Columns["ID"].Visible = false;
                connection.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query;
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                query = @"
        SELECT
            ID,
            Klient AS 'Клиент',
            Personal AS 'Персонал',
            Avto AS 'Автомобиль',
            Date AS 'Дата',
            Summa AS 'Сумма',
            BeginArenda AS 'Начало аренды',
            EndArenda AS 'Конец аренды'
        FROM 
            DeletedContracts";
            }
            else
            {
                string selectedField = comboBox1.SelectedItem.ToString();
                string searchText = textBox1.Text;
                query = $@"
        SELECT
            ID,
            Klient AS 'Клиент',
            Personal AS 'Персонал',
            Avto AS 'Автомобиль',
            Date AS 'Дата',
            Summa AS 'Сумма',
            BeginArenda AS 'Начало аренды',
            EndArenda AS 'Конец аренды'
        FROM 
            DeletedContracts
        WHERE 
            [{selectedField}] LIKE '%{searchText}%'";
            }

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;"))
            {
                connection.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Записи не найдены.");
                }
                else
                {
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string selectedId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;"))
                {
                    connection.Open();
                    string query = "DELETE FROM DeletedContracts WHERE ID=@ID";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ID", selectedId);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                MessageBox.Show("Запись успешно удалена");
                LoadDeletedContractData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Создаем экземпляр первой формы
            Form3 form3 = new Form3();

            // Показываем первую форму
            form3.Show();

            // Закрываем текущую форму (Form3)
            this.Close();
        }
    }
}

