using System;
using System.Data.SqlClient;
using System.Configuration;

namespace организация_церковных_мероприятий
{
    public class DatabaseHelper
    {
        public enum UserRole { Admin = 1, Operator = 2, Guest = 3 }

        public class User
        {
            public string Login { get; set; }
            public UserRole Role { get; set; }
        }

        public static User Authenticate(string login, string password)
        {
            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT RoleId FROM Users WHERE Login=@login AND Password=@pass";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@pass", password);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return new User { Login = login, Role = (UserRole)Convert.ToInt32(result) };
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка БД: " + ex.Message);
                }
            }
            return null;
        }
    }
}
