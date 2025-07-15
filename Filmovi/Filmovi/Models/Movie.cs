using System.ComponentModel;

namespace Filmovi.Models
{
    [Serializable]
    public class Movie : INotifyPropertyChanged
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        private string imagePath;
        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                if (imagePath != value)
                {
                    imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public int Duration { get; set; }
        public string RtfFilePath { get; set; }
        public DateTime DateAdded { get; set; }

        public int Rate { get; set; }

        public Movie() { }

        public Movie(bool isSelected, int number, string title, string imagePath, string rtfFilePath, DateTime dateAdded, int rate)
        {
            IsSelected = isSelected;
            Duration = number;
            Title = title;
            ImagePath = imagePath;
            RtfFilePath = rtfFilePath;
            DateAdded = dateAdded;
            Rate = rate;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
