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
    public partial class students : Window
    {

        SqlConnection sqlConnection = new SqlConnection(@"Data Source=Roxy1996;Initial Catalog=school; 
        Integrated Security=True;User ID=student;Password=12345");
        public students()
        {
            InitializeComponent();
            LoadSubjects();
        }
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
         private void SubjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedSubject = (string)SubjectComboBox.SelectedItem;

            string teacherInfo = GetTeacherInfo(selectedSubject);

          
            TeacherInfoTextBox.Text = teacherInfo;
        }

        private string GetTeacherInfo(string subjectName)
        {
            string teacherInfo = "";

            try
            {
                sqlConnection.Open();

                string query = "SELECT FirstName, LastName, Email, ContactNumber FROM teachers WHERE SubjectId IN (SELECT SubjectId FROM subjects WHERE SubjectName = @SubjectName)";
                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@SubjectName", subjectName);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string firstName = reader["FirstName"].ToString();
                        string lastName = reader["LastName"].ToString();
                        string email = reader["Email"].ToString();
                        string phone = reader["ContactNumber"].ToString();

                        teacherInfo = $"Имя: {firstName}\nФамилия: {lastName}\nEmail: {email}\nКонтактный телефон: {phone}";
                    }
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

            return teacherInfo;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
