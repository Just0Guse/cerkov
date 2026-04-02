using System.Windows;

namespace организация_церковных_мероприятий
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string pass = PassBox.Password;

            var user = DatabaseHelper.Authenticate(login, pass);

            if (user != null)
            {
                App.CurrentRole = user.Role;
                EventsWindow ev = new EventsWindow(user.Role);
                ev.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }

        private void GuestClick(object sender, RoutedEventArgs e)
        {
            App.CurrentRole = DatabaseHelper.UserRole.Guest;
            EventsWindow ev = new EventsWindow(DatabaseHelper.UserRole.Guest);
            ev.Show();
            this.Close();
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
