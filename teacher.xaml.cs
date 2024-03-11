using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;
using System.Collections.ObjectModel;
using System.Globalization;

namespace WpfApp1
{
      public partial class teacher : Window
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=Roxy1996;Initial Catalog=school;
        Integrated Security=True;User ID=schoolTeacher;Password=12345");
        public teacher()
        {
            InitializeComponent();
            LoadSubjects();
        }
        //запрос о студенте
        private void btnStudentInform_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();

                string firstName = FirstName.Text;
                string lastName = LastName.Text;
                string groupNum = GroupNum.Text;

                string query = "SELECT Email, ContactNumber FROM students WHERE FirstName = @FirstName AND LastName = @LastName AND GroupNumber = @GroupNumber";

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@GroupNumber", groupNum);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    string email = dataTable.Rows[0]["Email"].ToString();
                    string contactNumber = dataTable.Rows[0]["ContactNumber"].ToString();

                    StudentInfo.Text = $"Email: {email}\nContact Number: {contactNumber}";
                }
                else
                {
                    StudentInfo.Text = "Студент не найден.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        //вывод оценок
        private void btnMarks_Click_1(object sender, RoutedEventArgs e)
        {
            richTextBox.Document.Blocks.Clear();

            try
            {
                int studentID = GetStudentID(FirstName.Text, LastName.Text, GroupNum.Text);
                string selectedSubject = SubjectComboBox.SelectedItem.ToString();
                int subjectID = GetSubjectID(selectedSubject);

                StudentMarksBySubject(studentID, subjectID); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //вывод оценок
        private void StudentMarksBySubject(int studentID, int subjectID)
        {
            try
            {
                sqlConnection.Open();

                string query = "EXEC GetStudentMarksBySubject @StudentID, @SubjectID";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@StudentID", studentID);
                command.Parameters.AddWithValue("@SubjectID", subjectID);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int marks = Convert.ToInt32(reader["Marks"]);
                    DateTime date = Convert.ToDateTime(reader["Date"]);

                    richTextBox.AppendText($"Оценка: {marks}, Дата: {date}\n");
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
         private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        //предметы
        private void LoadSubjects()
        {
            try
            {
                sqlConnection.Open();

                string query = "SELECT SubjectName FROM subjects";

                SqlCommand command = new SqlCommand(query, sqlConnection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                List<string> subjects = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    subjects.Add(row["SubjectName"].ToString());
                }

                SubjectComboBox.ItemsSource = subjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        private int GetStudentID(string firstName, string lastName, string groupNum)
        {
            int studentID = -1;

            try
            {
                sqlConnection.Open();

                string query = @"SELECT StudentID FROM students WHERE FirstName = @FirstName AND LastName = @LastName AND GroupNumber = @GroupNumber";

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@GroupNumber", groupNum);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    studentID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return studentID;
        }
        private void SubjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedSubject = (string)comboBox.SelectedItem;
            int subjectID = GetSubjectID(selectedSubject);         
            int studentID = GetStudentID(FirstName.Text, LastName.Text, GroupNum.Text);          
            DisplayStudentMarks(studentID, subjectID);
        }
        private int GetSubjectID(string subjectName)
        {
            int subjectID = -1;

            try
            {
                sqlConnection.Open();

                string query = "SELECT SubjectID FROM subjects WHERE SubjectName = @SubjectName";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@SubjectName", subjectName);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    subjectID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return subjectID;
        }

        private void DisplayStudentMarks(int studentID, int subjectID)
        {
            try
            {
                sqlConnection.Open();

                string query = "EXEC GetStudentMarksBySubject @StudentID, @SubjectID";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@StudentID", studentID);
                command.Parameters.AddWithValue("@SubjectID", subjectID);

                SqlDataReader reader = command.ExecuteReader();
                richTextBox.Document.Blocks.Clear();

                while (reader.Read())
                {
                    int marks = Convert.ToInt32(reader["Marks"]);
                    DateTime date = Convert.ToDateTime(reader["Date"]);

                    richTextBox.AppendText($"Оценка: {marks}, Дата: {date}\n");
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
       private void btnAddMark_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentDate = DateTime.Today;
            string formattedDate = currentDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            string connectionString = @"Data Source=Roxy1996;Initial Catalog=school;Integrated Security=True;User ID=schoolTeacher;Password=12345";

            string firstName = FirstName.Text;
            string lastName = LastName.Text;
            string groupNum = GroupNum.Text;
            string selectedSubject = SubjectComboBox.SelectedItem.ToString();
            int marks = int.Parse(markInput.Text);
             try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                     int studentID = GetStudentID(firstName, lastName, groupNum);
                      int subjectID = GetSubjectID(selectedSubject);

                    if (studentID != -1 && subjectID != -1)
                    {
                        // Вставка оценки в marks
                        string query = "INSERT INTO marks (StudentID, SubjectID, Marks, Date) VALUES (@StudentID, @SubjectID, @Marks, @Date)";
                        SqlCommand command = new SqlCommand(query, sqlConnection);
                        command.Parameters.AddWithValue("@StudentID", studentID);
                        command.Parameters.AddWithValue("@SubjectID", subjectID);
                        command.Parameters.AddWithValue("@Marks", marks);
                        command.Parameters.AddWithValue("@Date", currentDate);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Оценка успешно добавлена.");
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить оценку.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Студент или предмет не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
         private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            // Получение нового значения номера группы из TextBox
            string newGroupNumber = newGroupNum.Text;
            string firstName = FirstName.Text;
            string lastName = LastName.Text;

            // Обновление записи в таблице БД
            UpdateStudentGroupNumber(firstName, lastName, newGroupNumber);
        }

        private void UpdateStudentGroupNumber(string firstName, string lastName, string newGroupNumber)
        {
            string connectionString = @"Data Source=Roxy1996;Initial Catalog=school;Integrated Security=True;User ID=schoolTeacher;Password=12345";
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    string query = "UPDATE students SET GroupNumber = @NewGroupNumber WHERE FirstName = @FirstName AND LastName = @LastName";
                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@NewGroupNumber", newGroupNumber);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        GroupNum.Text = newGroupNumber;
                        MessageBox.Show("Номер группы успешно изменен.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось изменить номер группы.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}


