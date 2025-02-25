namespace Alkitab.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using DialogHostAvalonia;

public partial class MainWindowViewModel : ViewModelBase
{
    // public string Greeting { get; } = "Welcome to Avalonia!";

    // Property to hold the list of Kitab entities
    public ObservableCollection<Kitab> KitabList { get; set; } = new ObservableCollection<Kitab>();
    public ObservableCollection<DaftarKitab> DaftarKitab { get; set; } =
        new ObservableCollection<DaftarKitab>();

    // DatabaseManager instance
    private readonly DatabaseManager _databaseManager;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    // Constructor
    public MainWindowViewModel()
    {
        _databaseManager = new DatabaseManager();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await LoadBible(); // Ensures this completes first
        await LoadBookList();
        GetProperties(); // Runs after all async tasks
        Console.WriteLine(IsLoading ? "Loading..." : "Done...");
    }

    public void GetProperties()
    {
        // Get the Type object for Kitab
        Type kitabType = typeof(Kitab);

        // Get the properties of the Kitab class
        PropertyInfo[] properties = kitabType.GetProperties();
    }

    // Method to load entities asynchronously
    private async Task LoadBible()
    {
        if (IsLoading)
            return; // Prevent duplicate calls
        IsLoading = true;

        Console.WriteLine(IsLoading ? "Loading..." : "Done...");

        var bible = await _databaseManager.GetBible(); // Make sure this returns IEnumerable<Kitab>
        if (bible != null)
        {
            foreach (var book in bible)
            {
                KitabList.Add(book); // Add each individual Kitab item to the ObservableCollection
                Console.WriteLine(book);
            }
        }

        IsLoading = false;
    }

    private async Task LoadBookList()
    {
        var book_list = await _databaseManager.GetBookList(); // Make sure this returns IEnumerable<Kitab>

        if (book_list != null)
        {
            foreach (var book in book_list)
            {
                DaftarKitab.Add(book);
            }
        }

        Console.WriteLine($"Final DaftarKitab count: {DaftarKitab.Count}");
    }
}
