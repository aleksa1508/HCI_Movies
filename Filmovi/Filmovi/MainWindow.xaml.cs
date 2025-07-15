using Filmovi.Helpers;
using Filmovi.Models;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Filmovi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotificationManager notificationManager;
        private string usernamePlaceHolder = "Insert username";
        public ObservableCollection<Users> Users;
        private DataIO serializer = new DataIO();
        public MainWindow()
        {
            InitializeComponent();
            notificationManager = new NotificationManager();
            TextBoxUsername.Text = usernamePlaceHolder;
            Users = serializer.DeSerializeObject<ObservableCollection<Users>>("UsersAccounts.xml");
            if (Users == null)
            {
                Users = new ObservableCollection<Users>();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to exit", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SaveDataAsXML();
                this.Close();
            }

        }
        private void SaveDataAsXML()
        {
            serializer.SerializeObject<ObservableCollection<Users>>(Users, "UsersAccounts.xml");
        }
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        public void ShowToastNotification(ToastNotification toastNotification)
        {
            notificationManager.Show(toastNotification.Title, toastNotification.Message, toastNotification.Type, "WindowNotificationArea");
        }

        private void TextBoxUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxUsername.Text.Trim().Equals(usernamePlaceHolder))
            {
                TextBoxUsername.Text = string.Empty;
            }
        }

        private void TextBoxUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxUsername.Text.Trim().Equals(string.Empty))
            {
                TextBoxUsername.Text = usernamePlaceHolder;
            }
        }

        private void ButtonLogIn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (Validation())
            {
                //ako je admin,proslijediti admin rolu zbog profila a i ne mora,a za korisnika korisnik rolu
                mainWindow.ShowToastNotification(new ToastNotification("Successful", "You are log in", NotificationType.Success));
                if (TextBoxUsername.Text.Trim().Equals("admin"))
                {
                    MovieWindow movieWindow = new MovieWindow(true);
                    movieWindow.Closed += MovieWindow_Closed;
                    movieWindow.Show();
                    this.Hide(); // Sakrije trenutni p
                }
                else
                {
                    MovieWindow movieWindow = new MovieWindow(false);
                    movieWindow.Closed += MovieWindow_Closed;
                    movieWindow.Show();
                    this.Hide(); // Sakrije trenutni p
                }
                //da li prikazivati ovaj notification success

            }
            else
            {
                mainWindow.ShowToastNotification(new ToastNotification("Unsuccessuful", "Username or password is not correct", NotificationType.Error));
            }
        }
        private void MovieWindow_Closed(object sender, EventArgs e)
        {
            this.Show(); // Kad se novi prozor zatvori, ovaj se ponovo prikazuje
        }

        private bool Validation()
        {
            bool validate = true;
            if (TextBoxUsername.Text.Trim().Equals(string.Empty) || TextBoxUsername.Text.Trim().Equals(usernamePlaceHolder))
            {
                validate = false;
                ErrorUsernameLabel.Content = "*Field cannot be empty";
                TextBoxUsername.BorderBrush = Brushes.Red;
            }
            else
            {
                ErrorUsernameLabel.Content = string.Empty;
                TextBoxUsername.BorderBrush = Brushes.LightGray;
            }
            if (PasswordBox.Password.Trim().Equals(string.Empty))
            {
                validate = false;
                ErrorPasswordLabel.Content = "*Field cannot be empty";
                PasswordBox.BorderBrush = Brushes.Red;
            }
            else
            {
                ErrorPasswordLabel.Content = string.Empty;
                PasswordBox.BorderBrush = Brushes.LightGray;
            }

            if (!SearchUsers())
            {
                validate = false;
            }

            return validate;
        }
        private bool SearchUsers()
        {
            foreach (var u in Users)
            {
                if (u.username.Equals(TextBoxUsername.Text) && u.password.Equals(PasswordBox.Password))
                {
                    return true;
                }
            }
            return false;
        }
    }
}