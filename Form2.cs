﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            textBox1.Validating += textBox1_Validating;
            textBox2.Validating += textBox2_Validating;
            textBox3.Validating += textBox3_Validating;
            textBox4.Validating += textBox4_Validating;
            textBox5.Validating += textBox5_Validating;
            textBox6.Validating += textBox6_Validating;
            textBox7.Validating += textBox7_Validating;

            // Заполняем ComboBox1 доступными параметрами для поиска на русском языке
            comboBox1.Items.Add("Марка");
            comboBox1.Items.Add("Название");
            comboBox1.Items.Add("Цена");
            comboBox1.Items.Add("Страна");
            comboBox1.SelectedIndex = 0; // Устанавливаем "Название" в качестве выбранного параметра по умолчаниюию
        }

        private void LoadAutomobileData(string searchTerm = "", string searchColumn = "Title")
        {
            connection.Open();
            string query = "SELECT s.ID AS CarID, s.Stamp, s.Title, s.Color, co.Country AS CountryName, c.Price, c.Availability FROM Specifications s JOIN Cars c ON s.ID = c.ID_auto JOIN Country co ON c.ID_country = co.ID";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                switch (searchColumn)
                {
                    case "Название":
                        query += $" WHERE s.Title LIKE '%{searchTerm}%'";
                        break;
                    case "Цена":
                        query += $" WHERE c.Price = {searchTerm}";
                        break;
                    case "Страна":
                        query += $" WHERE co.Country LIKE '%{searchTerm}%'";
                        break;
                    case "Марка":
                        query += $" WHERE s.Stamp LIKE '%{searchTerm}%'";
                        break;
                }
            }
            adapter = new SQLiteDataAdapter(query, connection);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string availability = selectedRow.Cells["Availability"].Value.ToString();

                if (availability == "No")
                {
                    MessageBox.Show("Автомобиль недоступен");
                    return;
                }

                // Ввод данных пользователя и количество дней аренды
                string surname = textBox1.Text;
                string name = textBox2.Text;
                string lastName = textBox3.Text;
                string seria = textBox4.Text;
                string number = textBox5.Text;
                string telephone = textBox6.Text;
                int days = int.Parse(textBox7.Text);
                double pricePerDay = double.Parse(selectedRow.Cells["Price"].Value.ToString());
                double totalAmount = days * pricePerDay;

                // Добавление записи в таблицу клиентов
                string queryClient = "INSERT INTO Klients (Surname, Name, LastName, Seria, Number, Telephone) VALUES (@Surname, @Name, @LastName, @Seria, @Number, @Telephone)";
                SQLiteCommand cmdClient = new SQLiteCommand(queryClient, connection);
                connection.Open();
                cmdClient.Parameters.AddWithValue("@Surname", surname);
                cmdClient.Parameters.AddWithValue("@Name", name);
                cmdClient.Parameters.AddWithValue("@LastName", lastName);
                cmdClient.Parameters.AddWithValue("@Seria", seria);
                cmdClient.Parameters.AddWithValue("@Number", number);
                cmdClient.Parameters.AddWithValue("@Telephone", telephone);
                cmdClient.ExecuteNonQuery();

                // Получение ID клиента
                string queryGetClientID = "SELECT last_insert_rowid()";
                SQLiteCommand cmdGetClientID = new SQLiteCommand(queryGetClientID, connection);
                int clientId = Convert.ToInt32(cmdGetClientID.ExecuteScalar());

                // Получение ID автомобиля
                int carId = Convert.ToInt32(selectedRow.Cells["CarID"].Value);

                // Обновление статуса доступности автомобиля
                string queryUpdateAvailability = "UPDATE Cars SET Availability='No' WHERE ID=@ID";
                SQLiteCommand cmdUpdateAvailability = new SQLiteCommand(queryUpdateAvailability, connection);
                cmdUpdateAvailability.Parameters.AddWithValue("@ID", carId);
                cmdUpdateAvailability.ExecuteNonQuery();

                // Получение текущей даты и времени
                DateTime now = DateTime.Now;
                string currentDate = now.ToString("dd.MM.yyyy");
                string beginArenda = now.ToString("dd.MM.yyyy HH:mm");
                string endArenda = now.AddDays(days).ToString("dd.MM.yyyy HH:mm");

                // Добавление записи в таблицу контрактов
                string queryContract = "INSERT INTO Contract (ID_Klient, ID_Car, Summa, Date, BeginArenda, EndArenda, SeriaKlient, NumberKlient) VALUES (@ID_Klient, @ID_Car, @Summa, @Date, @BeginArenda, @EndArenda, @Seria_Client, @Number_Client)";
                SQLiteCommand cmdContract = new SQLiteCommand(queryContract, connection);
                cmdContract.Parameters.AddWithValue("@ID_Klient", clientId);
                cmdContract.Parameters.AddWithValue("@ID_Car", carId);
                cmdContract.Parameters.AddWithValue("@Summa", totalAmount);
                cmdContract.Parameters.AddWithValue("@Date", currentDate);
                cmdContract.Parameters.AddWithValue("@BeginArenda", beginArenda);
                cmdContract.Parameters.AddWithValue("@EndArenda", endArenda);
                cmdContract.Parameters.AddWithValue("@Seria_Client", seria);
                cmdContract.Parameters.AddWithValue("@Number_Client", number);
                cmdContract.ExecuteNonQuery();
                connection.Close();

                MessageBox.Show("Автомобиль успешно арендован");
                LoadAutomobileData(); // Обновление данных в таблице автомобилей
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автомобиль для аренды.");
            }
        }


        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (!IsValidName(textBox1.Text))
            {
                MessageBox.Show("Неправильно введена фамилия.");
                e.Cancel = true;
            }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            if (!IsValidName(textBox2.Text))
            {
                MessageBox.Show("Неправильно введено имя.");
                e.Cancel = true;
            }
        }

        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
            if (!IsValidName(textBox3.Text))
            {
                MessageBox.Show("Неправильно введено отчество.");
                e.Cancel = true;
            }
        }

        private void textBox4_Validating(object sender, CancelEventArgs e)
        {
            if (textBox4.Text.Length != 4 || !IsNumeric(textBox4.Text))
            {
                MessageBox.Show("Серия паспорта должна содержать 4 цифры.");
                e.Cancel = true;
            }
        }

        private void textBox5_Validating(object sender, CancelEventArgs e)
        {
            if (textBox5.Text.Length != 6 || !IsNumeric(textBox5.Text))
            {
                MessageBox.Show("Номер паспорта должен содержать 6 цифр.");
                e.Cancel = true;
            }
        }

        private void textBox6_Validating(object sender, CancelEventArgs e)
        {
            string phonePattern = @"^(\+7|8)\d{10}$";
            if (!Regex.IsMatch(textBox6.Text, phonePattern))
            {
                MessageBox.Show("Неверный формат номера телефона.");
                e.Cancel = true;
            }
        }

        private void textBox7_Validating(object sender, CancelEventArgs e)
        {
            if (!IsNumeric(textBox7.Text))
            {
                MessageBox.Show("Количество дней аренды должно быть целым числом.");
                e.Cancel = true;
            }
        }

        private bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.All(char.IsLetter);
        }

        private bool IsNumeric(string value)
        {
            return int.TryParse(value, out _);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string searchTerm = textBox8.Text;
            string searchColumn = (string)comboBox1.SelectedItem;
            LoadAutomobileData(searchTerm, searchColumn);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Обновляем текст надписи над текстовым полем в зависимости от выбранного параметра
            string selectedParameter = (string)comboBox1.SelectedItem;
            label8.Text = $"Введите {selectedParameter}:";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Создаем экземпляр первой формы
            Form1 form1 = new Form1();

            // Показываем первую форму
            form1.Show();

            // Закрываем текущую форму (Form3)
            this.Close();
        }
    }
}    

