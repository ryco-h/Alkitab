namespace Alkitab.Models
{
    using System.ComponentModel;

    public class Kitab : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public int verse_id { get; set; }
        private string? book_name;

        public string? BookName
        {
            get => book_name;
            set
            {
                if (book_name != value)
                {
                    book_name = value;
                    OnPropertyChanged(nameof(BookName));
                }
            }
        }

        public string? book { get; set; }
        public string? chapter { get; set; }
        public string? verse { get; set; }
        public string? text { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}