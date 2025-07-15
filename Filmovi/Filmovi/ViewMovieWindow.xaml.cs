using Filmovi.Models;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Filmovi
{
    /// <summary>
    /// Interaction logic for ViewMovieWindow.xaml
    /// </summary>
    public partial class ViewMovieWindow : Window
    {
        private Movie movie;
        public ViewMovieWindow(Movie m)
        {
            InitializeComponent();
            movie = m;
            SetText();
        }
        private void SetText()
        {
            TextBlockTitle.Text = movie.Title;
            TextBlockDuration.Text = movie.Duration.ToString();
            string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, movie.ImagePath);

            //MessageBox.Show("movie.imagePath->" + movie.ImagePath + "\nimagePath->" + imagePath);
            if (File.Exists(imagePath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImageMovie.Source = bitmap;
            }
            TextBlockRates.Text = movie.Rate.ToString() + "/10";
            TextBlockDateAdded.Text = movie.DateAdded.Day.ToString() + "-" + movie.DateAdded.Month.ToString() + "-" + movie.DateAdded.Year.ToString();

            if (File.Exists(movie.RtfFilePath))
            {
                using (FileStream fs = new FileStream(movie.RtfFilePath, FileMode.Open))
                {
                    TextRange range = new TextRange(
                        EditorRichTextBox.Document.ContentStart,
                        EditorRichTextBox.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);


                    string text = range.Text;

                    int wordCount = text.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

                    WordCountTextBlock.Text = $"Words: {wordCount}";
                }
            }
            else
            {
                // Ako datoteka ne postoji, možeš postaviti podrazumevani tekst
                EditorRichTextBox.Document.Blocks.Clear();
                EditorRichTextBox.Document.Blocks.Add(new Paragraph(new Run("Opis nije pronađen.")));
            }
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to exit", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Close();
            }

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
