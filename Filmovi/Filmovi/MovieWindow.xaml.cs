using Filmovi.Helpers;
using Filmovi.Models;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
namespace Filmovi
{
    /// <summary>
    /// Interaction logic for MovieWindow.xaml
    /// </summary>
    public partial class MovieWindow : Window
    {
        public ObservableCollection<Movie> Movies { get; set; }
        private DataIO serializer = new DataIO();
        private NotificationManager notificationManager;

        public MovieWindow(bool isAdmin)
        {
            InitializeComponent();
            notificationManager = new NotificationManager();

            Movies = serializer.DeSerializeObject<ObservableCollection<Movie>>("Movies.xml");
            if (Movies == null)
            {
                Movies = new ObservableCollection<Movie>();
            }

            //DataGridMovie.ItemsSource = Movies;
            DataContext = this;
            if (isAdmin)
            {
                StackPanelForModification.Visibility = Visibility.Visible;
                TextBlockUser.Text = "ADMIN";
            }
            else
            {
                TextBlockUser.Text = "VISITOR";
                StackPanelForModification.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to exit", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (var movie in Movies)
                {
                    movie.IsSelected = false;
                }
                SaveMoviesAsXml();
                this.Close();
            }

        }
        public void ShowToastNotification(ToastNotification toastNotification)
        {
            notificationManager.Show(toastNotification.Title, toastNotification.Message, toastNotification.Type, "WindowNotificationArea");
        }
        private void SaveMoviesAsXml()
        {
            serializer.SerializeObject<ObservableCollection<Movie>>(Movies, "Movies.xml");
        }
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

            AddMovieWindow addMovieWindow = new AddMovieWindow(Movies);
            addMovieWindow.ShowDialog();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (Movies.Count > 0)
            {
                List<Movie> checkedMovies = new List<Movie>();
                foreach (var movies in Movies)
                {
                    if (movies.IsSelected == true)
                    {
                        checkedMovies.Add(movies);
                    }
                }

                if (checkedMovies.Count > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (var m in checkedMovies)
                        {
                            Movies.Remove(m);
                        }
                        //mozda da stoji ovako samo jednom nottification a ne na svako brisanje
                        mainWindow.ShowToastNotification(new ToastNotification("Success", "Movie removed from the Data Table", NotificationType.Success));
                    }

                }
                else
                {
                    mainWindow.ShowToastNotification(new ToastNotification("Error", "You must select CheckBox for delete", NotificationType.Error));
                }


                /* if (DataGridMovie.SelectedIndex != -1)
                 {
                     if(DataGridMovie.SelectedItems.Count== 1)
                     {
                         Movie m = (Movie)DataGridMovie.SelectedItem;
                         if (m.IsSelected == true)
                         {
                             Movies.RemoveAt(DataGridMovie.SelectedIndex);
                             mainWindow.ShowToastNotification(new ToastNotification("Success", "Student removed from the Data Table", NotificationType.Success));
                         }
                         else
                         {
                             mainWindow.ShowToastNotification(new ToastNotification("Error", "You must select CheckBox for delete", NotificationType.Error));

                         }

                     }
                     else
                     {
                         mainWindow.ShowToastNotification(new ToastNotification("Error", "You can delete only one movie at a time!", NotificationType.Error));
                     }
                 }
                 else
                 {
                     mainWindow.ShowToastNotification(new ToastNotification("Error", "A student must be selected first!", NotificationType.Error));
                 }*/
            }
            else
            {
                mainWindow.ShowToastNotification(new ToastNotification("Error", "Cannot delete from empty table!", NotificationType.Error));
            }
        }

        private void CheckBoxAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var movies in Movies)
            {
                movies.IsSelected = true;
            }
        }

        private void CheckBoxAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var movies in Movies)
            {
                movies.IsSelected = false;
            }
        }




        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink)
            {
                Movie m = ((Movie)hyperlink.DataContext);
                if (TextBlockUser.Text.Equals("VISITOR"))
                {
                    ViewMovieWindow viewMovieWindow = new ViewMovieWindow(m);
                    viewMovieWindow.ShowDialog();
                }
                else
                {
                    EditMovieWindow editMovieWindow = new EditMovieWindow(m, Movies);
                    editMovieWindow.ShowDialog();
                }
            }
        }
    }
}
