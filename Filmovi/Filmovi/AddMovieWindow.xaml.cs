using Filmovi.Helpers;
using Filmovi.Models;
using Microsoft.Win32;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Filmovi
{
    /// <summary>
    /// Interaction logic for AddMovieWindow.xaml
    /// </summary>
    public partial class AddMovieWindow : Window
    {
        private string pathPicture = "";
        private NotificationManager notificationManager;
        public ObservableCollection<Movie> Movies { get; set; }

        public AddMovieWindow(ObservableCollection<Movie> movies)
        {
            InitializeComponent();
            Movies = movies;
            notificationManager = new NotificationManager();
            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            List<int> fontSizes = Enumerable.Range(8, 22 - 8 + 1).ToList();
            FontSizeComboBox.ItemsSource = fontSizes;
            FontSizeComboBox.SelectedItem = 12;
            FontColorComboBox.ItemsSource = typeof(Colors).GetProperties().Select(p => p.Name).ToList(); ;

        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
        public void ShowToastNotification(ToastNotification toastNotification)
        {
            notificationManager.Show(toastNotification.Title, toastNotification.Message, toastNotification.Type, "WindowNotificationArea");
        }
        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                EditorRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        private void EditorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object fontWeight = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldToggleButton.IsChecked = fontWeight.Equals(FontWeights.Bold) && fontWeight != DependencyProperty.UnsetValue;

            object fontItalic = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicToggleButton.IsChecked = fontItalic.Equals(FontStyles.Italic) && fontItalic != DependencyProperty.UnsetValue;

            object fontUnderline = EditorRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineToggleButton.IsChecked = fontUnderline.Equals(TextDecorations.Underline) && fontUnderline != DependencyProperty.UnsetValue;

            object fontFamily = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamilyComboBox.SelectedItem = fontFamily;

        }
        private bool Validation()
        {
            bool isValid = true;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                ErrortTitleLabel.Content = "*You must enter a movie title.";
                isValid = false;
                TitleTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                ErrortTitleLabel.Content = string.Empty;
                TitleTextBox.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FFABADB3")!);
            }

            if (string.IsNullOrWhiteSpace(DurationTextBox.Text))
            {
                ErrorDurationLabel.Content = "*You must enter duration in minutes.";
                isValid = false;
                DurationTextBox.BorderBrush = Brushes.Red;
            }
            else if (!int.TryParse(DurationTextBox.Text, out _))
            {
                ErrorDurationLabel.Content = "*Duration must be a numeric value.";
                isValid = false;
                DurationTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                ErrorDurationLabel.Content = string.Empty;
                DurationTextBox.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FFABADB3")!);
            }


            if (string.IsNullOrWhiteSpace(RateTextBox.Text))
            {
                ErrorRateLabel.Content = "*You must enter rate value.";
                isValid = false;
                RateTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                if (int.TryParse(RateTextBox.Text, out int broj))
                {
                    if (broj > 0 && broj <= 10)
                    {
                        ErrorRateLabel.Content = string.Empty;
                        RateTextBox.BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FFABADB3")!);
                    }
                    else
                    {
                        ErrorImageLabel.Content = "*Rate must be number 1-10.";
                        isValid = false;
                        RateTextBox.BorderBrush = Brushes.Red;
                    }
                }
                else
                {
                    ErrorRateLabel.Content = "*Rate must be a numeric value.";
                    isValid = false;
                    RateTextBox.BorderBrush = Brushes.Red;
                }
            }
           

            if (string.IsNullOrWhiteSpace(pathPicture))
            {
                ErrorImageLabel.Content = "*You must select an image.";
                isValid = false;
            }
            else
            {
                ErrorImageLabel.Content = string.Empty;
            }

            TextRange textRange = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
            if (string.IsNullOrWhiteSpace(textRange.Text.Trim()))
            {
                ErrorDescriptionLabel.Content = "*You must enter a description.";
                isValid = false;
                EditorRichTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                ErrorDescriptionLabel.Content = string.Empty;
                EditorRichTextBox.BorderBrush = Brushes.Black;
            }

            return isValid;
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (Validation())
            {
                //automatski se dodaje date and time u polje klase movie
                string rtfPutanja = DodajTekstURtfDatoteku();
                Movies.Add(new Movie(false, Int32.Parse(DurationTextBox.Text), TitleTextBox.Text, pathPicture, rtfPutanja, DateTime.Now, Int32.Parse(RateTextBox.Text)));
                mainWindow.ShowToastNotification(new ToastNotification("Successful", "You are successfully add movie", NotificationType.Success));
                ResetFields();
            }
            else
            {
                mainWindow.ShowToastNotification(new ToastNotification("Error", "Data is not valid.", NotificationType.Error));
            }
        }

        private string DodajTekstURtfDatoteku()
        {
            string folderPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Movies");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Kreiraj jedinstveno ime za .rtf fajl na osnovu naslova
            string fileName = $"{TitleTextBox.Text}.rtf";
            string fullPath = System.IO.Path.Combine(folderPath, fileName);

            // Snimi sadržaj RichTextBox-a u .rtf fajl
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                TextRange range = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
                range.Save(fs, DataFormats.Rtf);
            }
            return fullPath;
        }
        private void ResetFields()
        {
            TitleTextBox.Text = string.Empty;
            DurationTextBox.Text = string.Empty;
            pathPicture = string.Empty;
            EditorRichTextBox.Document.Blocks.Clear();
            SelectImage.Source = new BitmapImage();
            RateTextBox.Text = string.Empty;
        }
        private void EditorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange textRange = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
            string text = textRange.Text;

            int wordCount = text.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

            WordCountTextBlock.Text = $"Words: {wordCount}";
        }

        private void SearchImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();

                pathPicture = openFileDialog.FileName;
                SelectImage.Source = bitmap;

            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem != null)
            {
                int selectedSize = (int)FontSizeComboBox.SelectedItem;
                EditorRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)selectedSize);
            }
        }

        private void FontColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontColorComboBox.SelectedItem != null)
            {
                string? selectedColorName = FontColorComboBox.SelectedItem.ToString();
                Color selectedColor = (Color)ColorConverter.ConvertFromString(selectedColorName);
                EditorRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(selectedColor));

            }
        }
    }
}
