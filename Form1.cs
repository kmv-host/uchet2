using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uchet2
{
    public partial class Form1 : Form
    {
        // Задаем правильные логин и пароль
        private const string CorrectLogin = "admin";
        private const string CorrectPassword = "123";

        public Form1()
        {
            InitializeComponent();

            // Скрываем символы пароля звездочками
            textBoxPassword.PasswordChar = '*';

            // Назначаем кнопку buttonLogin срабатывающей по нажатию Enter
            this.AcceptButton = buttonLogin;
        }
        // Обработчик события нажатия кнопки "Войти"
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Проверяем введенные данные
            if (textBoxLogin.Text == CorrectLogin && textBoxPassword.Text == CorrectPassword)
            {
                // Создаем экземпляр второй формы
                Form2 form2 = new Form2();

                // Подписываемся на событие закрытия второй формы, 
                // чтобы при её закрытии полностью закрывалась первая форма (и всё приложение)
                form2.FormClosed += (s, args) => this.Close();

                // Прячем первую форму
                this.Hide();

                // Открываем вторую форму
                form2.Show();
            }
            else
            {
                // Выводим сообщение об ошибке, если данные неверны
                MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
