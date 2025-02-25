using System.ComponentModel;

public class DaftarKitab : INotifyPropertyChanged
{
    public int book_id { get; set; }
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

    // Override ToString() to return BookName
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
