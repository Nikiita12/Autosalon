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
using System.Text.RegularExpressions;
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
            connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;");
            LoadKlientData();
            LoadAutomobileData();
            LoadContractData();
            LoadPersonalData();
            LoadCountryData();
            LoadSpecificationsData();
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
            dataGridView3.SelectionChanged += dataGridView3_SelectionChanged;
            dataGridView4.SelectionChanged += dataGridView4_SelectionChanged;
            dataGridView5.SelectionChanged += dataGridView5_SelectionChanged;
            dataGridView6.SelectionChanged += dataGridView6_SelectionChanged;
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
            comboBox5.SelectedIndexChanged += сomboBox5_SelectedIndexChanged;
            textBox21.Leave += UpdateRentSum;
            textBox22.Leave += UpdateRentSum;

            // Заполнение comboBox1 данными из таблицы specifications
            string query1 = "SELECT ID, Stamp || ' ' || Title AS Full FROM specifications";
            SQLiteDataAdapter da1 = new SQLiteDataAdapter(query1, connection);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            comboBox1.DataSource = dt1;
            comboBox1.DisplayMember = "Full"; // Отображаемое значение в комбобоксе
            comboBox1.ValueMember = "ID";     // ID автомобиля

            // Заполнение comboBox2 данными из таблицы country
            string query2 = "SELECT ID, Country FROM country";
            SQLiteDataAdapter da2 = new SQLiteDataAdapter(query2, connection);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            comboBox2.DataSource = dt2;
            comboBox2.DisplayMember = "Country"; // Название страны
            comboBox2.ValueMember = "ID";        // ID страны

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

            
            string query5 = @"
SELECT 
    cars.ID, 
    specifications.Stamp || ' ' || specifications.Title AS 'Автомобиль',
    cars.Price AS 'Цена'
FROM 
    cars 
JOIN 
    specifications ON cars.ID_auto = specifications.ID";

            SQLiteDataAdapter da5 = new SQLiteDataAdapter(query5, connection);
            DataTable dt5 = new DataTable();
            da5.Fill(dt5);
            comboBox5.DataSource = dt5;
            comboBox5.DisplayMember = "Автомобиль";
            comboBox5.ValueMember = "ID";

            // Обработка события изменения выбранного клиента в comboBox3
            comboBox6.Items.Clear();
            comboBox6.Items.Add("Имя");
            comboBox6.Items.Add("Фамилия");
            comboBox6.Items.Add("Отчество");
            comboBox6.Items.Add("Серия");
            comboBox6.Items.Add("Номер");
            comboBox6.Items.Add("Телефон");
            comboBox6.SelectedIndex = 0;

            comboBox7.Items.Clear();
            comboBox7.Items.Add("Имя");
            comboBox7.Items.Add("Фамилия");
            comboBox7.Items.Add("Отчество");
            comboBox7.Items.Add("Должность");
            comboBox7.Items.Add("Телефон");
            comboBox7.Items.Add("Электронная почта");
            comboBox7.SelectedIndex = 0;

            comboBox8.Items.Clear();
            comboBox8.Items.Add("Марка");
            comboBox8.Items.Add("Название");
            comboBox8.Items.Add("Цвет");
            comboBox8.SelectedIndex = 0;

            comboBox9.Items.Clear();
            comboBox9.Items.Add("Марка");
            comboBox9.Items.Add("Название");
            comboBox9.Items.Add("Цвет");
            comboBox9.Items.Add("Цена");
            comboBox9.Items.Add("Наличие");
            comboBox9.Items.Add("Страна");
            comboBox9.SelectedIndex = 0;

            comboBox10.Items.Clear();
            comboBox10.Items.Add("Клиент");
            comboBox10.Items.Add("Персонал");
            comboBox10.Items.Add("Автомобиль");
            comboBox10.Items.Add("Дата");
            comboBox10.Items.Add("Сумма");
            comboBox10.Items.Add("Начало аренды");
            comboBox10.Items.Add("Конец аренды");
            comboBox10.SelectedIndex = 0;
        }

        private void LoadKlientData()
        {
            connection.Open();
            string query = "SELECT ID, Surname AS 'Фамилия', Name AS 'Имя', LastName AS 'Отчество', Seria AS 'Серия', Number AS 'Номер', Telephone AS 'Телефон' FROM Klients";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();
            dataGridView1.Columns["ID"].Visible = false;
        }

        private void LoadAutomobileData()
        {
            connection.Open();
            string query = @"
        SELECT
            cars.ID,
            specifications.Title AS 'Название',
            specifications.Stamp AS 'Марка',
            country.Country AS 'Страна',
            cars.Price AS 'Цена',
            cars.Availability AS 'Наличие'
        FROM 
            cars
        JOIN 
            specifications ON cars.ID_auto = specifications.ID
        JOIN 
            country ON cars.ID_country = country.ID";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView2.DataSource = dt;
            dataGridView2.Columns["ID"].Visible = false;
            connection.Close();
        }

        
        private void LoadContractData()
        {
            connection.Open();
            string query = @"
    SELECT 
        Contract.ID, -- Сохраняем ID для внутренних нужд
        klients.Surname || ' ' || klients.Name || ' ' || klients.LastName AS 'Клиент',
        personal.Surname || ' ' || personal.Name || ' ' || personal.LastName AS 'Персонал',
        specifications.Stamp || ' ' || specifications.Title AS 'Автомобиль',
        contract.Date AS 'Дата',
        contract.Summa AS 'Сумма',
        contract.BeginArenda AS 'Начало аренды',
        contract.EndArenda AS 'Конец аренды'
    FROM 
        contract
    LEFT JOIN 
        klients ON contract.ID_klient = klients.ID
    LEFT JOIN 
        personal ON contract.ID_personal = personal.ID
    LEFT JOIN 
        cars ON contract.ID_car = cars.ID
    LEFT JOIN 
        specifications ON cars.ID_auto = specifications.ID";

            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView3.DataSource = dt;
            dataGridView3.Columns["ID"].Visible = false;
            connection.Close();

            

            // Добавляем событие RowPrePaint для изменения цвета строки
            dataGridView3.RowPrePaint += DataGridView3_RowPrePaint;
        }

        private void DataGridView3_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            DateTime currentDate = DateTime.Now;

            if (dgv.Rows[e.RowIndex].Cells["Конец аренды"].Value != null)
            {
                DateTime endArenda = Convert.ToDateTime(dgv.Rows[e.RowIndex].Cells["Конец аренды"].Value);
                TimeSpan difference = endArenda - currentDate;

                if (difference.TotalDays < 0)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red; // Аренда закончилась
                                                                                 // Обновляем доступность автомобиля
                    UpdateCarAvailabilityInDatabase(dgv.Rows[e.RowIndex].Cells["ID"].Value.ToString(), "Yes"); // Обновляем доступность
                }
                else if (difference.TotalDays <= 1)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow; // Менее 1 дня
                }
                else
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green; // Более 1 дня
                }
            }
        }

        private void UpdateCarAvailabilityInDatabase(string carId, string availability)
        {
            connection.Open();
            string query = "UPDATE cars SET Availability = @availability WHERE ID = @carId";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@availability", availability);
                command.Parameters.AddWithValue("@carId", carId);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        
        private void LoadPersonalData()
        {
            connection.Open();
            string query = "SELECT ID, Surname AS 'Фамилия', Name AS 'Имя', LastName AS 'Отчество', Post AS 'Должность', Telephone AS 'Телефон', Mail AS 'Электронная почта' FROM Personal";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView4.DataSource = dt;
            dataGridView4.Columns["ID"].Visible = false;
            connection.Close();
        }

        private void LoadCountryData()
        {
            connection.Open();
            string query = "SELECT ID, Country AS 'Страна' FROM Country";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView6.DataSource = dt;
            dataGridView6.Columns["ID"].Visible = false;
            connection.Close();
        }

        private void LoadSpecificationsData()
        {
            connection.Open();
            string query = "SELECT ID, Stamp AS 'Марка', Title AS 'Название', Color AS 'Цвет' FROM Specifications";
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            dataGridView5.DataSource = dt;
            dataGridView5.Columns["ID"].Visible = false;
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsValidName(textBox2.Text) || !IsValidName(textBox3.Text) || !IsValidName(textBox4.Text))
            {
                MessageBox.Show("Фамилия, Имя и Отчество должны содержать только буквы.");
                return;
            }

            if (textBox5.Text.Length != 4 || !int.TryParse(textBox5.Text, out _))
            {
                MessageBox.Show("Серия паспорта должна содержать 4 цифры.");
                return;
            }

            if (textBox6.Text.Length != 6 || !int.TryParse(textBox6.Text, out _))
            {
                MessageBox.Show("Номер паспорта должен содержать 6 цифр.");
                return;
            }

            if (!IsUniqueSeriaNumber(textBox5.Text, textBox6.Text))
            {
                MessageBox.Show("Такое сочетание серии и номера паспорта уже существует.");
                return;
            }

            string phonePattern = @"^(\+7|8)\d{10}$";
            if (!Regex.IsMatch(textBox7.Text, phonePattern))
            {
                MessageBox.Show("Неверный формат номера телефона.");
                return;
            }

            if (!IsUniqueTelephone(textBox7.Text))
            {
                MessageBox.Show("Такой номер телефона уже существует.");
                return;
            }

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
            MessageBox.Show("Запись успешно добавлена");
            LoadKlientData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string clientId = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();

                if (!IsValidName(textBox2.Text) || !IsValidName(textBox3.Text) || !IsValidName(textBox4.Text))
                {
                    MessageBox.Show("Фамилия, Имя и Отчество должны содержать только буквы.");
                    return;
                }

                if (textBox5.Text.Length != 4 || !int.TryParse(textBox5.Text, out _))
                {
                    MessageBox.Show("Серия паспорта должна содержать 4 цифры.");
                    return;
                }

                if (textBox6.Text.Length != 6 || !int.TryParse(textBox6.Text, out _))
                {
                    MessageBox.Show("Номер паспорта должен содержать 6 цифр.");
                    return;
                }

                string phonePattern = @"^(\+7|8)\d{10}$";
                if (!Regex.IsMatch(textBox7.Text, phonePattern))
                {
                    MessageBox.Show("Неверный формат номера телефона.");
                    return;
                }

                string query = "UPDATE Klients SET Surname=@Surname, Name=@Name, LastName=@LastName, Seria=@Seria, Number=@Number, Telephone=@Telephone WHERE ID=@ID";
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
                MessageBox.Show("Запись успешно обновлена");
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
                string query = "DELETE FROM Klients WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", clientId);
                connection.Open();
                cmd.ExecuteNonQuery();
                SQLiteCommand reorderCmd = new SQLiteCommand("UPDATE Klients SET ID = (SELECT COUNT(*) FROM Klients k2 WHERE k2.ID < Klients.ID) + 1", connection);
                reorderCmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно удалена");
                LoadKlientData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для удаления.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!IsNumeric(textBox11.Text))
            {
                MessageBox.Show("Стоимость должна содержать только цифры.");
                return;
            }

            if (!IsYesOrNo(textBox12.Text))
            {
                MessageBox.Show("Доступность должна быть либо 'Yes' либо 'No'.");
                return;
            }

            string query = "INSERT INTO Cars (ID_auto, ID_country, Price, Availability) VALUES (@ID_auto, @ID_country, @Price, @Availability)";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@ID_auto", comboBox1.SelectedValue);
            cmd.Parameters.AddWithValue("@ID_country", comboBox2.SelectedValue);
            cmd.Parameters.AddWithValue("@Price", textBox11.Text);
            cmd.Parameters.AddWithValue("@Availability", textBox12.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show("Запись успешно добавлена");
            LoadAutomobileData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                string carId = dataGridView2.SelectedRows[0].Cells["ID"].Value.ToString();

                if (!IsNumeric(textBox11.Text))
                {
                    MessageBox.Show("Стоимость должна содержать только цифры.");
                    return;
                }

                if (!IsYesOrNo(textBox12.Text))
                {
                    MessageBox.Show("Доступность должна быть либо 'Yes' либо 'No'.");
                    return;
                }

                string query = "UPDATE Cars SET ID_auto=@ID_auto, ID_country=@ID_country, Price=@Price, Availability=@Availability WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", carId);
                cmd.Parameters.AddWithValue("@ID_auto", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@ID_country", comboBox2.SelectedValue);
                cmd.Parameters.AddWithValue("@Price", textBox11.Text);
                cmd.Parameters.AddWithValue("@Availability", textBox12.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно обновлена");
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
                string query = "DELETE FROM Cars WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", carId);
                connection.Open();
                cmd.ExecuteNonQuery(); // Пересчет ID
                SQLiteCommand reorderCmd = new SQLiteCommand("UPDATE Cars SET ID = (SELECT COUNT(*) FROM Cars c2 WHERE c2.ID < Cars.ID) + 1", connection);
                reorderCmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно удалена");
                LoadAutomobileData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите машину для удаления.");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (!IsValidName(textBox24.Text) || !IsValidName(textBox25.Text) || !IsValidName(textBox26.Text) || !IsValidName(textBox27.Text))
            {
                MessageBox.Show("Фамилия, Имя, Отчество и Должность должны содержать только буквы.");
                return;
            }

            string phonePattern = @"^(\+7|8)\d{10}$";
            if (!Regex.IsMatch(textBox28.Text, phonePattern))
            {
                MessageBox.Show("Неверный формат номера телефона.");
                return;
            }

            if (!IsUniqueTelephonePer(textBox28.Text))
            {
                MessageBox.Show("Такой номер телефона уже существует.");
                return;
            }

            if (!IsValidMail(textBox29.Text))
            {
                MessageBox.Show("Неверный формат электронной почты.");
                return;
            }

            if (!IsUniqueMail(textBox29.Text))
            {
                MessageBox.Show("Такой адрес электронной почты уже существует.");
                return;
            }

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
            MessageBox.Show("Запись успешно добавлена");
            LoadPersonalData();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (dataGridView4.SelectedRows.Count > 0)
            {
                string personalId = dataGridView4.SelectedRows[0].Cells["ID"].Value.ToString();

                if (!IsValidName(textBox24.Text) || !IsValidName(textBox25.Text) || !IsValidName(textBox26.Text) || !IsValidName(textBox27.Text))
                {
                    MessageBox.Show("Фамилия, Имя, Отчество и Должность должны содержать только буквы.");
                    return;
                }

                string phonePattern = @"^(\+7|8)\d{10}$";
                if (!Regex.IsMatch(textBox28.Text, phonePattern))
                {
                    MessageBox.Show("Неверный формат номера телефона.");
                    return;
                }

                if (!IsValidMail(textBox29.Text))
                {
                    MessageBox.Show("Неверный формат электронной почты.");
                    return;
                }

                string query = "UPDATE Personal SET Surname=@Surname, Name=@Name, LastName=@LastName, Post=@Post, Telephone=@Telephone, Mail=@Mail WHERE ID=@ID";
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
                MessageBox.Show("Запись успешно обновлена");
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
                string personalId = dataGridView4.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "DELETE FROM Personal WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", personalId);
                connection.Open();
                cmd.ExecuteNonQuery();
                SQLiteCommand reorderCmd = new SQLiteCommand("UPDATE Personal SET ID = (SELECT COUNT(*) FROM Personal p2 WHERE p2.ID < Personal.ID) + 1", connection);
                reorderCmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно удалена");
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
            MessageBox.Show("Запись успешно добавлена");
            LoadSpecificationsData();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (dataGridView5.SelectedRows.Count > 0)
            {
                string SpId = dataGridView5.SelectedRows[0].Cells["ID"].Value.ToString();
                string query = "UPDATE Specifications SET Stamp=@Stamp, Title=@Title, Color=@Color WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", SpId);
                cmd.Parameters.AddWithValue("@Stamp", textBox31.Text);
                cmd.Parameters.AddWithValue("@Title", textBox32.Text);
                cmd.Parameters.AddWithValue("@Color", textBox33.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно обновлена");
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
                string query = "DELETE FROM Specifications WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", SpId);
                connection.Open();
                cmd.ExecuteNonQuery(); // Пересчет ID
                SQLiteCommand reorderCmd = new SQLiteCommand("UPDATE Specifications SET ID = (SELECT COUNT(*) FROM Specifications s2 WHERE s2.ID < Specifications.ID) + 1", connection);
                reorderCmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно удалена");
                LoadSpecificationsData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автомобиль для удаления.");
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (!IsTextOnly(textBox35.Text))
            {
                MessageBox.Show("Название страны должно содержать только буквы.");
                return;
            }

            if (!IsUniqueCountry(textBox35.Text))
            {
                MessageBox.Show("Такая страна уже существует.");
                return;
            }

            string query = "INSERT INTO Country (Country) VALUES (@Country)";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Country", textBox35.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show("Запись успешно добавлена");
            LoadCountryData();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (dataGridView6.SelectedRows.Count > 0)
            {
                string CoId = dataGridView6.SelectedRows[0].Cells["ID"].Value.ToString();

                if (!IsTextOnly(textBox35.Text))
                {
                    MessageBox.Show("Название страны должно содержать только буквы.");
                    return;
                }

                string query = "UPDATE Country SET Country=@Country WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", CoId);
                cmd.Parameters.AddWithValue("@Country", textBox35.Text);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно обновлена");
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
                string query = "DELETE FROM Country WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", CoId);
                connection.Open();
                cmd.ExecuteNonQuery(); // Пересчет ID
                SQLiteCommand reorderCmd = new SQLiteCommand("UPDATE Country SET ID = (SELECT COUNT(*) FROM Country c2 WHERE c2.ID < Country.ID) + 1", connection);
                reorderCmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно удалена");
                LoadCountryData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите страну для удаления.");
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem is DataRowView selectedClient)
            {
                // Проверяем, есть ли нужные поля
                if (selectedClient.Row.Table.Columns.Contains("Seria") && selectedClient.Row.Table.Columns.Contains("Number"))
                {
                    string seria = selectedClient["Seria"]?.ToString();
                    string number = selectedClient["Number"]?.ToString();

                    // Заполняем текстовые поля
                    textBox17.Text = seria;
                    textBox20.Text = number;
                }
                else
                {
                    MessageBox.Show("Обратите внимание: столбцы 'Seria' или 'Number' отсутствуют в источнике данных.");
                }
            }
        }

        private void сomboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedItem is DataRowView selectedCar)
            {
                if (selectedCar.Row.Table.Columns.Contains("Цена")) // Обратите внимание на правильное имя столбца
                {
                    decimal price = Convert.ToDecimal(selectedCar["Цена"]);
                    textBox18.Text = price.ToString("F2");
                }
                else
                {
                    MessageBox.Show("Столбец 'Цена' отсутствует в данных.");
                }
            }
        }

        private void UpdateRentSum(object sender, EventArgs e)
        {
            // Параметры для хранения значений
            decimal dailyRate = 0m;
            DateTime startDate;
            DateTime endDate;

            // Получаем цену автомобиля, если выбран элемент в comboBox5
            if (comboBox5.SelectedValue != null)
            {
                DataRowView selectedCar = (DataRowView)comboBox5.SelectedItem;
                if (decimal.TryParse(selectedCar["Цена"].ToString(), out dailyRate))
                {
                    // Заполняем текстовое поле для начальной суммы
                    textBox18.Text = dailyRate.ToString("F2");
                }
            }

            // Проверяем ввод даты начала аренды
            if (DateTime.TryParse(textBox21.Text, out startDate) &&
                DateTime.TryParse(textBox22.Text, out endDate))
            {
                // Рассчитываем количество дней аренды
                int rentalDays = (endDate - startDate).Days;

                // Проверяем корректность дат
                if (rentalDays > 0 && dailyRate > 0)
                {
                    decimal totalSum = rentalDays * dailyRate;
                    // Обновляем текстовое поле с общей стоимостью
                    textBox18.Text = totalSum.ToString("F2");
                }
                else if (rentalDays <= 0)
                {
                    MessageBox.Show("Дата окончания аренды должна быть позже даты начала аренды.");
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!IsDateValid(textBox19.Text))
            {
                MessageBox.Show("Дата должна быть в формате дд.мм.гггг.");
                return;
            }

            if (!IsNumeric(textBox18.Text))
            {
                MessageBox.Show("Сумма должна содержать только цифры.");
                return;
            }

            if (textBox17.Text.Length != 4 || !int.TryParse(textBox17.Text, out _))
            {
                MessageBox.Show("Серия клиента должна содержать 4 цифры.");
                return;
            }

            if (textBox20.Text.Length != 6 || !int.TryParse(textBox20.Text, out _))
            {
                MessageBox.Show("Номер клиента должен содержать 6 цифр.");
                return;
            }

            if (!IsUniqueSeriaNumberContract(textBox17.Text, textBox20.Text))
            {
                MessageBox.Show("Такое сочетание серии и номера уже существует.");
                return;
            }

            if (!IsDateTimeValid(textBox21.Text))
            {
                MessageBox.Show("Начало аренды должно быть в формате дд.мм.гггг чч:мм.");
                return;
            }

            if (!IsDateTimeValid(textBox22.Text))
            {
                MessageBox.Show("Конец аренды должен быть в формате дд.мм.гггг чч:мм.");
                return;
            }

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
            MessageBox.Show("Запись успешно добавлена");
            LoadContractData();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0)
            {
                string contractId = dataGridView3.SelectedRows[0].Cells["ID"].Value.ToString();

                if (!IsDateValid(textBox19.Text))
                {
                    MessageBox.Show("Дата должна быть в формате дд.мм.гггг.");
                    return;
                }

                if (!IsNumeric(textBox18.Text))
                {
                    MessageBox.Show("Сумма должна содержать только цифры.");
                    return;
                }

                if (textBox17.Text.Length != 4 || !int.TryParse(textBox17.Text, out _))
                {
                    MessageBox.Show("Серия клиента должна содержать 4 цифры.");
                    return;
                }

                if (textBox20.Text.Length != 6 || !int.TryParse(textBox20.Text, out _))
                {
                    MessageBox.Show("Номер клиента должен содержать 6 цифр.");
                    return;
                }

                if (!IsDateTimeValid(textBox21.Text))
                {
                    MessageBox.Show("Начало аренды должно быть в формате дд.мм.гггг чч:мм.");
                    return;
                }

                if (!IsDateTimeValid(textBox22.Text))
                {
                    MessageBox.Show("Конец аренды должен быть в формате дд.мм.гггг чч:мм.");
                    return;
                }

                string query = "UPDATE contract SET ID_klient=@ID_klient, ID_car=@ID_car, ID_personal=@ID_personal, Date=@Date, Summa=@Summa, SeriaKlient=@SeriaKlient, NumberKlient=@NumberKlient, BeginArenda=@BeginArenda, EndArenda=@EndArenda WHERE ID=@ID";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID", contractId);
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
                MessageBox.Show("Запись успешно обновлена");
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
                DataGridViewRow selectedRow = dataGridView3.SelectedRows[0];
                string contractId = selectedRow.Cells["ID"].Value.ToString();

                // Сохранение данных удаленного контракта
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;"))
                {
                    connection.Open();
                    string insertQuery = @"
                INSERT INTO DeletedContracts (ID, Klient, Personal, Avto, Date, Summa, BeginArenda, EndArenda)
                SELECT 
                    contract.ID,
                    klients.Surname || ' ' || klients.Name || ' ' || klients.LastName,
                    personal.Surname || ' ' || personal.Name || ' ' || personal.LastName,
                    specifications.Stamp || ' ' || specifications.Title,
                    contract.Date,
                    contract.Summa,
                    contract.BeginArenda,
                    contract.EndArenda
                FROM contract
                LEFT JOIN klients ON contract.ID_klient = klients.ID
                LEFT JOIN personal ON contract.ID_personal = personal.ID
                LEFT JOIN cars ON contract.ID_car = cars.ID
                LEFT JOIN specifications ON cars.ID_auto = specifications.ID
                WHERE contract.ID = @ID";

                    SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@ID", contractId);
                    insertCmd.ExecuteNonQuery();
                }

                // Удаление контракта из таблицы contract
                string deleteQuery = "DELETE FROM contract WHERE ID=@ID";
                SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, connection);
                deleteCmd.Parameters.AddWithValue("@ID", contractId);
                connection.Open();
                deleteCmd.ExecuteNonQuery(); // Пересчет ID
                SQLiteCommand reorderCmd = new SQLiteCommand("UPDATE contract SET ID = (SELECT COUNT(*) FROM contract c2 WHERE c2.ID < contract.ID) + 1", connection);
                reorderCmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Запись успешно удалена");
                LoadContractData();

                // Обновление данных в форме 4
                Form4 form4 = new Form4();
                form4.LoadDeletedContractData();
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
                query = "SELECT ID, Surname AS 'Фамилия', Name AS 'Имя', LastName AS 'Отчество', Seria AS 'Серия', Number AS 'Номер', Telephone AS 'Телефон' FROM Klients";
            }
            else
            {
                string selectedField = comboBox6.SelectedItem.ToString();
                string searchText = textBox1.Text;
                query = $"SELECT ID, Surname AS 'Фамилия', Name AS 'Имя', LastName AS 'Отчество', Seria AS 'Серия', Number AS 'Номер', Telephone AS 'Телефон' FROM Klients WHERE [{selectedField}] LIKE '%{searchText}%'";
            }
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
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

        private void button7_Click(object sender, EventArgs e)
        {
            string query;
            if (string.IsNullOrEmpty(textBox8.Text))
            {
                query = "SELECT ID, Surname AS 'Фамилия', Name AS 'Имя', LastName AS 'Отчество', Post AS 'Должность', Telephone AS 'Телефон', Mail AS 'Электронная почта' FROM Personal";
            }
            else
            {
                string selectedField = comboBox7.SelectedItem.ToString();
                string searchText = textBox8.Text;
                query = $"SELECT ID, Surname AS 'Фамилия', Name AS 'Имя', LastName AS 'Отчество', Post AS 'Должность', Telephone AS 'Телефон', Mail AS 'Электронная почта' FROM Personal WHERE [{selectedField}] LIKE '%{searchText}%'";
            }
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Записи не найдены.");
            }
            else
            {
                dataGridView4.DataSource = dt;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string query;
            if (string.IsNullOrEmpty(textBox9.Text))
            {
                query = "SELECT ID, Stamp AS 'Марка', Title AS 'Название', Color AS 'Цвет' FROM Specifications";
            }
            else
            {
                string selectedField = comboBox8.SelectedItem.ToString();
                string searchText = textBox9.Text;
                query = $"SELECT ID, Stamp AS 'Марка', Title AS 'Название', Color AS 'Цвет' FROM Specifications WHERE [{selectedField}] LIKE '%{searchText}%'";
            }
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Записи не найдены.");
            }
            else
            {
                dataGridView5.DataSource = dt;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string query;
            if (string.IsNullOrEmpty(textBox10.Text))
            {
                // Если поле поиска пустое, загрузить все данные
                query = @"
            SELECT 
                cars.ID,
                specifications.Title AS 'Название',
                specifications.Stamp AS 'Марка',
                country.Country AS 'Страна',
                cars.Price AS 'Цена',
                cars.Availability AS 'Наличие'
            FROM 
                cars
            JOIN 
                specifications ON cars.ID_auto = specifications.ID
            JOIN 
                country ON cars.ID_country = country.ID";
            }
            else
            {
                // Поиск по выбранному критерию
                string selectedField = comboBox9.SelectedItem.ToString();
                string searchText = textBox10.Text;
                query = $@"
            SELECT 
                cars.ID,
                specifications.Title AS 'Название',
                specifications.Stamp AS 'Марка',
                specifications.Color AS 'Цвет',
                cars.Price AS 'Цена',
                cars.Availability AS 'Наличие',
                country.Country AS 'Страна'
            FROM 
                cars
            JOIN 
                specifications ON cars.ID_auto = specifications.ID
            JOIN 
                country ON cars.ID_country = country.ID
            WHERE 
                {selectedField} LIKE '%{searchText}%'";
            }
            adapter = new SQLiteDataAdapter(query, connection);
            dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Записи не найдены.");
            }
            else
            {
                dataGridView2.DataSource = dt;
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            // Начинаем с базового запроса
            string baseQuery = @"
SELECT 
    contract.ID,
    klients.Surname || ' ' || klients.Name || ' ' || klients.LastName AS 'Клиент',
    personal.Surname || ' ' || personal.Name || ' ' || personal.LastName AS 'Персонал',
    specifications.Stamp || ' ' || specifications.Title AS 'Автомобиль',
    contract.Date AS 'Дата',
    contract.Summa AS 'Сумма',
    contract.BeginArenda AS 'Начало аренды',
    contract.EndArenda AS 'Конец аренды'
FROM 
    contract
LEFT JOIN 
    klients ON contract.ID_klient = klients.ID
LEFT JOIN 
    personal ON contract.ID_personal = personal.ID
LEFT JOIN 
    cars ON contract.ID_car = cars.ID
LEFT JOIN 
    specifications ON cars.ID_auto = specifications.ID";

            // Начинаем формировать условия
            List<string> conditions = new List<string>();

            // Добавляем условие, если текстовое поле поиска не пустое
            if (!string.IsNullOrEmpty(textBox13.Text))
            {
                string selectedField = comboBox10.SelectedItem.ToString();
                string searchText = textBox13.Text;

                // Проверяем возможность пустых значений в полях таблицы
                conditions.Add($"({selectedField} LIKE '%{searchText}%' OR {selectedField} IS NULL)");
            }

            // Объединяем условия в WHERE
            string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

            // Полный запрос
            string query = $"{baseQuery} {whereClause}";

            // Выполняем запрос и заполняем DataGridView
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Записи не найдены.");
                }
                else
                {
                    dataGridView3.DataSource = dt;
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                textBox2.Text = selectedRow.Cells["Фамилия"].Value.ToString();
                textBox3.Text = selectedRow.Cells["Имя"].Value.ToString();
                textBox4.Text = selectedRow.Cells["Отчество"].Value.ToString();
                textBox5.Text = selectedRow.Cells["Серия"].Value.ToString();
                textBox6.Text = selectedRow.Cells["Номер"].Value.ToString();
                textBox7.Text = selectedRow.Cells["Телефон"].Value.ToString();
            }
        }

        private void dataGridView4_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView4.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView4.SelectedRows[0];
                textBox24.Text = selectedRow.Cells["Фамилия"].Value.ToString();
                textBox25.Text = selectedRow.Cells["Имя"].Value.ToString();
                textBox26.Text = selectedRow.Cells["Отчество"].Value.ToString();
                textBox27.Text = selectedRow.Cells["Должность"].Value.ToString();
                textBox28.Text = selectedRow.Cells["Телефон"].Value.ToString();
                textBox29.Text = selectedRow.Cells["Электронная почта"].Value.ToString();
            }
        }

        private void dataGridView5_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView5.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView5.SelectedRows[0];
                textBox31.Text = selectedRow.Cells["Марка"].Value.ToString();
                textBox32.Text = selectedRow.Cells["Название"].Value.ToString();
                textBox33.Text = selectedRow.Cells["Цвет"].Value.ToString();
            }
        }

        private void dataGridView6_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView6.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView6.SelectedRows[0];
                textBox35.Text = selectedRow.Cells["Страна"].Value.ToString();
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                textBox11.Text = selectedRow.Cells["Цена"].Value?.ToString() ?? "";
                textBox12.Text = selectedRow.Cells["Наличие"].Value?.ToString() ?? "";

                string selectedStamp = selectedRow.Cells["Марка"].Value?.ToString() ?? "";
                string selectedTitle = selectedRow.Cells["Название"].Value?.ToString() ?? "";
                string selectedAuto = !string.IsNullOrEmpty(selectedStamp) && !string.IsNullOrEmpty(selectedTitle)
                    ? $"{selectedStamp} {selectedTitle}": "";

                if (!string.IsNullOrEmpty(selectedAuto))
                {
                    for (int i = 0; i < comboBox1.Items.Count; i++)
                    {
                        DataRowView item = (DataRowView)comboBox1.Items[i];
                        if (item["Full"].ToString() == selectedAuto)
                        {
                            comboBox1.SelectedIndex = i;
                            break;
                        }
                    }
                }

                string selectedCountry = selectedRow.Cells["Страна"].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(selectedCountry))
                {
                    for (int i = 0; i < comboBox2.Items.Count; i++)
                    {
                        if (((DataRowView)comboBox2.Items[i])["Country"].ToString() == selectedCountry)
                        {
                            comboBox2.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        // Обработчик события выбора строки
        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView3.SelectedRows[0];

                // Получаем ID выбранного контракта
                int contractID = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                // Запрос для получения данных контракта для DataGridView7
                string query7 = @"
SELECT 
    COALESCE(klients.Surname || ' ' || klients.Name || ' ' || klients.LastName, '') AS 'Клиент',
    COALESCE(personal.Surname || ' ' || personal.Name || ' ' || personal.LastName, '') AS 'Сотрудник',
    COALESCE(specifications.Stamp, '') || ' ' || COALESCE(specifications.Title, '') AS 'Автомобиль',
    Contract.Date AS 'Дата',
    Contract.Summa AS 'Сумма',
    Contract.BeginArenda AS 'Начало Аренды',
    Contract.EndArenda AS 'Конец Аренды',
    Contract.SeriaKlient AS 'Серия паспорта клиента',
    Contract.NumberKlient AS 'Номер паспорта клиента'
FROM 
    Contract
LEFT JOIN 
    klients ON Contract.ID_klient = klients.ID
LEFT JOIN
    personal ON Contract.ID_personal = personal.ID  
LEFT JOIN
    cars ON Contract.ID_car = cars.ID
LEFT JOIN
    specifications ON cars.ID_auto = specifications.ID
WHERE 
    Contract.ID = @contractID";

                DataTable dt7 = new DataTable();
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(query7, connection))
                {
                    da.SelectCommand.Parameters.AddWithValue("@contractID", contractID);
                    da.Fill(dt7);
                    dataGridView7.DataSource = dt7;
                }

                // Автозаполнение TextBoxes
                if (dt7.Rows.Count > 0)
                {
                    DataRow row = dt7.Rows[0];

                    textBox17.Text = row["Серия паспорта клиента"]?.ToString() ?? "";
                    textBox20.Text = row["Номер паспорта клиента"]?.ToString() ?? "";
                    textBox21.Text = row["Начало Аренды"]?.ToString() ?? "";
                    textBox22.Text = row["Конец Аренды"]?.ToString() ?? "";
                    textBox18.Text = row["Сумма"]?.ToString() ?? "";
                    textBox19.Text = row["Дата"]?.ToString() ?? "";

                    // Обновление ComboBox значений
                    string klientName = selectedRow.Cells["Клиент"].Value?.ToString() ?? ""; // Обратите внимание, что по-прежнему может возникнуть ошибка здесь
                    if (!string.IsNullOrEmpty(klientName))
                    {
                        for (int i = 0; i < comboBox3.Items.Count; i++)
                        {
                            if (((DataRowView)comboBox3.Items[i])["FullName"].ToString() == klientName)
                            {
                                comboBox3.SelectedIndex = i;
                                break;
                            }
                        }
                    }

                    string personalName = selectedRow.Cells["Персонал"].Value?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(personalName))
                    {
                        for (int i = 0; i < comboBox4.Items.Count; i++)
                        {
                            if (((DataRowView)comboBox4.Items[i])["FullName"].ToString() == personalName)
                            {
                                comboBox4.SelectedIndex = i;
                                break;
                            }
                        }
                    }

                    string carName = selectedRow.Cells["Автомобиль"].Value?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(carName))
                    {
                        for (int i = 0; i < comboBox5.Items.Count; i++)
                        {
                            if (((DataRowView)comboBox5.Items[i])["Автомобиль"].ToString() == carName)
                            {
                                comboBox5.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // Если данные не найдены, очищаем все поля
                    textBox17.Text = "";
                    textBox20.Text = "";
                    textBox21.Text = "";
                    textBox22.Text = "";
                    textBox18.Text = "";
                    textBox19.Text = "";

                    comboBox3.SelectedIndex = -1;
                    comboBox4.SelectedIndex = -1;
                    comboBox5.SelectedIndex = -1;

                    dataGridView7.DataSource = null; // Очищаем DataGridView
                }
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            LoadKlientData();
            LoadAutomobileData();
            LoadPersonalData();
            LoadSpecificationsData();
            LoadCountryData();
            LoadContractData();

            // Обновление данных в ComboBox
            LoadKlientComboBoxData();
            LoadAutomobileComboBoxData();
            LoadPersonalComboBoxData();
            LoadCountryComboBoxData();
            MessageBox.Show("Все данные обновлены.");
        }

        private void button26_Click(object sender, EventArgs e)
        {
            // Создаем экземпляр первой формы
            Form1 form1 = new Form1();

            // Показываем первую форму
            form1.Show();

            // Закрываем текущую форму (Form3)
            this.Close();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Form4 adminForm = new Form4();
            adminForm.ShowDialog();
            this.Hide(); // Закрыть текущую форму после открытия формы администратора
        }

        private bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.All(char.IsLetter);
        }

        private bool IsUniqueSeriaNumberContract(string seria, string number)
        {
            string query = "SELECT COUNT(*) FROM Contract WHERE SeriaKlient=@Seria AND NumberKlient=@Number";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Seria", seria);
            cmd.Parameters.AddWithValue("@Number", number);
            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            return count == 0;
        }

        private bool IsUniqueSeriaNumber(string seria, string number)
        {
            string query = "SELECT COUNT(*) FROM Klients WHERE Seria=@Seria AND Number=@Number";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Seria", seria);
            cmd.Parameters.AddWithValue("@Number", number);
            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            return count == 0;
        }

        private bool IsUniqueTelephone(string telephone)
        {
            string query = "SELECT COUNT(*) FROM Klients WHERE Telephone=@Telephone";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Telephone", telephone);
            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            return count == 0;
        }

        private bool IsUniqueTelephonePer(string telephone)
        {
            string query = "SELECT COUNT(*) FROM Personal WHERE Telephone=@Telephone";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Telephone", telephone);
            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            return count == 0;
        }

        private bool IsValidMail(string mail)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(mail, pattern);
        }


        private bool IsUniqueMail(string mail)
        {
            string query = "SELECT COUNT(*) FROM Personal WHERE Mail=@Mail";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Mail", mail);
            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            return count == 0;
        }

        private bool IsNumeric(string value)
        {
            return double.TryParse(value, out _);
        }

        private bool IsYesOrNo(string value)
        {
            return value == "Yes" || value == "No";
        }

        private bool IsTextOnly(string value)
        {
            return !string.IsNullOrEmpty(value) && value.All(char.IsLetter);
        }

        private bool IsUniqueCountry(string country)
        {
            string query = "SELECT COUNT(*) FROM Country WHERE Country=@Country";
            SQLiteCommand cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Country", country);
            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            return count == 0;
        }

        private bool IsDateValid(string date)
        {
            return DateTime.TryParseExact(date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out _);
        }

        private bool IsDateTimeValid(string dateTime)
        {
            return DateTime.TryParseExact(dateTime, "dd.MM.yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out _);
        }

        private void LoadKlientComboBoxData()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;"))
            {
                connection.Open();
                string query = "SELECT ID, Surname || ' ' || Name || ' ' || LastName AS FullName, Seria, Number FROM Klients";
                SQLiteDataAdapter da = new SQLiteDataAdapter(query, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                comboBox3.DataSource = dt;
                comboBox3.DisplayMember = "FullName";
                comboBox3.ValueMember = "ID";
                connection.Close();
            }
        }

        private void LoadAutomobileComboBoxData()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;"))
            {
                connection.Open();

                // Новый SQL-запрос, который вы предоставили
                string query5 = @"
        SELECT 
            cars.ID, 
            specifications.Stamp || ' ' || specifications.Title AS 'Автомобиль',
            cars.Price AS 'Цена'
        FROM 
            cars 
        JOIN 
            specifications ON cars.ID_auto = specifications.ID";

                SQLiteDataAdapter da5 = new SQLiteDataAdapter(query5, connection);
                DataTable dt5 = new DataTable();
                da5.Fill(dt5);

                // Связываем данные с комбобоксом
                comboBox5.DataSource = dt5;
                comboBox5.DisplayMember = "Автомобиль"; // Отображаем в комбобоксе
                comboBox5.ValueMember = "ID"; // Задаем значение для каждого элемента

                connection.Close();
            }
        }

        private void LoadPersonalComboBoxData()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;"))
            {
                connection.Open();
                string query = "SELECT ID, Surname || ' ' || Name || ' ' || LastName AS FullName FROM Personal";
                SQLiteDataAdapter da = new SQLiteDataAdapter(query, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                comboBox4.DataSource = dt;
                comboBox4.DisplayMember = "FullName";
                comboBox4.ValueMember = "ID";
                connection.Close();
            }
        }

        private void LoadCountryComboBoxData()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=Autosalon.db;Version=3;"))
            {
                connection.Open();
                string query = "SELECT ID, Country FROM Country";
                SQLiteDataAdapter da = new SQLiteDataAdapter(query, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                comboBox2.DataSource = dt;
                comboBox2.DisplayMember = "Country";
                comboBox2.ValueMember = "ID";
                connection.Close();
            }
        }
    }
}



