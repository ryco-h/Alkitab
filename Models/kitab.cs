using System.ComponentModel;

public class Kitab : INotifyPropertyChanged
{
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
                OnPropertyChanged(nameof(BookName)); // Ensure this triggers PropertyChanged
            }
        }
    }
    public string? book { get; set; }
    public string? chapter { get; set; }
    public string? verse { get; set; }
    public string? text { get; set; }

    // Event to notify when a property has changed
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
