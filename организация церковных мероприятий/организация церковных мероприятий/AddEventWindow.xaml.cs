using System.Configuration;
using System.Data.SqlClient;
using System.Windows;


namespace организация_церковных_мероприятий
{
    public partial class AddEventWindow : Window
    {
        public AddEventWindow()
        {
            InitializeComponent();
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "INSERT INTO Events (Title, Location, EventTime, Organizer, Contacts, IsApproved) " +
                               "VALUES (@t, @l, @d, @o, @c, @a)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@t", TitleTxt.Text);
                cmd.Parameters.AddWithValue("@l", LocationTxt.Text);
                cmd.Parameters.AddWithValue("@d", EventTime.Text);
                cmd.Parameters.AddWithValue("@o", OrgTxt.Text);
                cmd.Parameters.AddWithValue("@c", ContactTxt.Text);
                cmd.Parameters.AddWithValue("@a", (App.CurrentRole == DatabaseHelper.UserRole.Admin) ? 1 : 0);

                conn.Open();
                cmd.ExecuteNonQuery();  
                this.Close();
            }
        }
    }
}
