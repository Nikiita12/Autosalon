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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "user" && textBox2.Text == "")
            {
                Form2 userForm = new Form2();
                userForm.ShowDialog();
                this.Close(); // Закрыть текущую форму после открытия формы пользователя
            }
            else if (textBox1.Text == "admin" && textBox2.Text == "admin")
            {
                Form3 adminForm = new Form3();
                adminForm.ShowDialog();
                this.Close(); // Закрыть текущую форму после открытия формы администратора
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль. Пожалуйста, попробуйте снова.");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Заменяем каждый символ вводимого пароля на '*'
            textBox2.PasswordChar = '*';
        }
    }
}
