using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel; // Добавлено пространство имен для работы с ClosedXML

namespace uchet2
{
    public partial class Form2 : Form
    {
        // Строка подключения к локальной БД mdf (замените путь на свой, если нужно)
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EquipmentDB.mdf;Integrated Security=True";

        private SqlDataAdapter adapter;
        private DataTable dataTable;
        private SqlCommandBuilder commandBuilder;

        public Form2()
        {
            InitializeComponent();
            LoadData();
        }
        // Загрузка данных в DataGridView
        private void LoadData(string searchQuery = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Computers";

                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        query += " WHERE Fio LIKE @search OR Phone LIKE @search OR IpAddress LIKE @search";
                    }

                    adapter = new SqlDataAdapter(query, connection);
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + searchQuery + "%");
                    }

                    // Автоматическая генерация команд INSERT, UPDATE, DELETE для адаптера
                    commandBuilder = new SqlCommandBuilder(adapter);

                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;

                    // Скрываем столбец Id
                    if (dataGridView1.Columns["Id"] != null)
                        dataGridView1.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }
        // Кнопка "Добавить"
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddForm addForm = new AddForm(connectionString);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadData(); // Обновляем данные после добавления
            }
        }
        // Кнопка "Удалить"
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Удаляем строку из DataGridView (она автоматически помечается как удаленная в dataTable)
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                    }

                    // Сохраняем удаление в БД с помощью НОВОГО подключения
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            SqlDataAdapter updateAdapter = new SqlDataAdapter("SELECT * FROM Computers", connection);
                            SqlCommandBuilder builder = new SqlCommandBuilder(updateAdapter);

                            // Обновляем базу данных на основе изменений в dataTable
                            updateAdapter.Update(dataTable);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении из БД: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления (нажмите на заголовок строки слева).");
            }

        }
        // Кнопка "Редактировать" (Сохранение изменений из DataGridView)
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем новое подключение для сохранения изменений
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter updateAdapter = new SqlDataAdapter("SELECT * FROM Computers", connection);
                    SqlCommandBuilder builder = new SqlCommandBuilder(updateAdapter);

                    // Отправляем все изменения из таблицы в базу данных
                    updateAdapter.Update(dataTable);
                }
                MessageBox.Show("Изменения успешно сохранены.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
        // Кнопка "Обновить" (Перезагрузка данных из БД)
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Очищаем текстовое поле поиска, чтобы визуально всё было логично
            txtSearch.Clear();

            // Вызываем наш метод загрузки данных без параметров, 
            // чтобы он стянул всю базу данных целиком
            LoadData();
        }
        // Кнопка "Поиск"
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim());
        }
        // Кнопка "Экспорт в Excel"
        private void button2_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли данные в нашей таблице
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта!", "Внимание");
                return;
            }

            // Открываем диалоговое окно для выбора места сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Файлы Excel (*.xlsx)|*.xlsx"; // Доступен только формат Excel
            saveFileDialog.FileName = "Отчет_Оргтехника.xlsx";      // Имя файла по умолчанию
            saveFileDialog.Title = "Сохранить отчет в Excel";

            // Если пользователь выбрал место и нажал "Сохранить"
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Создаем новый документ Excel
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        // Добавляем лист с названием "Оргтехника"
                        var worksheet = workbook.Worksheets.Add("Оргтехника");

                        // Магия ClosedXML: автоматически загружаем всю нашу DataTable на лист,
                        // начиная с первой ячейки (A1). Она сама сделает заголовки и автофильтры.
                        worksheet.Cell(1, 1).InsertTable(dataTable);

                        // Автоматически подстраиваем ширину столбцов под длину текста
                        worksheet.Columns().AdjustToContents();

                        // Сохраняем файл по выбранному пользователем пути
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Данные успешно экспортированы в Excel!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Ошибка может возникнуть, например, если пользователь пытается перезаписать файл, который сейчас открыт в Excel
                    MessageBox.Show("Ошибка при экспорте. Возможно, файл открыт в другой программе.\n\nДетали: " + ex.Message, "Ошибка");
                }
            }
        }
        //Кнопка "Выход"
        private void butExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
