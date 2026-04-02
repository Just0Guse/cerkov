using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows;

namespace организация_церковных_мероприятий
{
    public partial class EventsWindow : Window
    {
        private DatabaseHelper.UserRole _role;

        public EventsWindow(DatabaseHelper.UserRole role)
        {
            InitializeComponent(); 
            _role = role;
            SetupAccess();
            LoadEvents();
        }

        private void SetupAccess()
        {
            AddBtn.Visibility = Visibility.Collapsed;
            ApproveBtn.Visibility = Visibility.Collapsed;
            UsersBtn.Visibility = Visibility.Collapsed;

            if (_role == DatabaseHelper.UserRole.Admin || _role == DatabaseHelper.UserRole.Operator)
                AddBtn.Visibility = Visibility.Visible;
                DeleteBtn.Visibility = Visibility.Visible;

            if (_role == DatabaseHelper.UserRole.Admin)
            {
                ApproveBtn.Visibility = Visibility.Visible;
                UsersBtn.Visibility = Visibility.Visible;
            }
        }
        private void BackClick(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void AddEventClick(object sender, RoutedEventArgs e)
        {
            AddEventWindow addWin = new AddEventWindow();

            if (addWin.ShowDialog() == true)
            {
                LoadEvents(); 
            }
        }
        private void LoadEvents()
        {
            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = (App.CurrentRole == DatabaseHelper.UserRole.Admin)
                    ? "SELECT * FROM Events ORDER BY EventId DESC"
                    : "SELECT * FROM Events WHERE IsApproved = 1 ORDER BY EventId DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                EventsGrid.ItemsSource = null;
                EventsGrid.ItemsSource = dt.DefaultView;
            }
        }
        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении: " + ex.Message);
            }
        }
        private void UsersBtnClick(object sender, RoutedEventArgs e)
        {
            UsersWindow uw = new UsersWindow();
            uw.ShowDialog(); 
        }
        private void ApproveBtnClick(object sender, RoutedEventArgs e) 
        {
            ApproveWindow aw = new ApproveWindow();
            aw.ShowDialog();
            LoadEvents();
        }
        private void DeleteEventClick(object sender, RoutedEventArgs e)
        {
            if (EventsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите мероприятие для удаления!");
                return;
            }

            DataRowView row = (DataRowView)EventsGrid.SelectedItem;
            int eventId = Convert.ToInt32(row["EventId"]);
            string eventTitle = row["Title"].ToString();

            var result = MessageBox.Show($"Вы уверены, что хотите удалить мероприятие '{eventTitle}'?",
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    try
                    {
                        string query = "DELETE FROM Events WHERE EventId = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", eventId);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Мероприятие удалено.");
                        LoadEvents(); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении: " + ex.Message);
                    }
                }
            }
        }
    }
}
