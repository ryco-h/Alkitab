namespace Alkitab.Models
{
    using System;
    using System.ComponentModel;

    public class BibleInstances : INotifyPropertyChanged
    {
        public int BookId { get; set; }
        private string? _bookName;

        public string? BookName
        {
            get => _bookName;
            set
            {
                if (_bookName != value)
                {
                    _bookName = value;
                    OnPropertyChanged(nameof(BookName));
                }
            }
        }

        private string? _bookNameText;

        public string? BookNameText
        {
            get => _bookNameText;
            set
            {
                if (_bookNameText != value)
                {
                    _bookNameText = value;
                    OnPropertyChanged(nameof(BookNameText));
                }
            }
        }

        private string? _selectedBookName;

        public string? SelectedBookName
        {
            get => _selectedBookName;
            set
            {
                if (_selectedBookName != value)
                {
                    _selectedBookName = value;
                    Console.WriteLine("SelectedBookName => " + SelectedBookName);
                    OnPropertyChanged(nameof(SelectedBookName));
                }
            }
        }

        public override string ToString()
        {
            return BookName ?? "Unknown Book";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}