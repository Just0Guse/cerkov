using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows;

namespace организация_церковных_мероприятий
{
    public partial class ApproveWindow : Window
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ApproveWindow()
        {
            InitializeComponent();
            LoadUnapproved();
        }

        private void LoadUnapproved()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT EventId, Title, Location, EventTime, Organizer FROM Events WHERE IsApproved = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                UnapprovedGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void ApproveClick(object sender, RoutedEventArgs e)
        {
            if (UnapprovedGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите мероприятие из списка!");
                return;
            }

            DataRowView row = (DataRowView)UnapprovedGrid.SelectedItem;
            int eventId = Convert.ToInt32(row["EventId"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "UPDATE Events SET IsApproved = 1 WHERE EventId = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", eventId);

                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Мероприятие одобрено!");
                LoadUnapproved(); 
            }
        }

        private void RejectClick(object sender, RoutedEventArgs e)
        {
            if (UnapprovedGrid.SelectedItem == null) return;

            DataRowView row = (DataRowView)UnapprovedGrid.SelectedItem;
            int eventId = Convert.ToInt32(row["EventId"]);

            if (MessageBox.Show("Удалить эту заявку?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Events WHERE EventId = @id", conn);
                    cmd.Parameters.AddWithValue("@id", eventId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    LoadUnapproved();
                }
            }
        }
    }
}
