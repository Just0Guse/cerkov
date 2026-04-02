using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;


namespace организация_церковных_мероприятий
{
    public partial class UsersWindow : Window
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public UsersWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT UserId, Login, RoleId FROM Users", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                UsersGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void AddUserClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserLoginBox.Text) || RoleCombo.SelectedItem == null) return;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "INSERT INTO Users (Login, Password, RoleId) VALUES (@log, @pass, @role)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@log", UserLoginBox.Text);
                cmd.Parameters.AddWithValue("@pass", UserPassBox.Text);

                var selectedItem = (ComboBoxItem)RoleCombo.SelectedItem;
                cmd.Parameters.AddWithValue("@role", selectedItem.Tag);

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Пользователь добавлен!");
                LoadUsers();
            }
        }

        private void DeleteUserClick(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)UsersGrid.SelectedItem;
            int id = (int)row["UserId"];

            if (MessageBox.Show("Удалить пользователя?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    LoadUsers();
                }
            }
        }
    }
}
