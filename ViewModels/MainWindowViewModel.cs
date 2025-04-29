using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Alkitab.Models;
using Alkitab.Services;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace Alkitab.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public BibleInstances BibleInstances => BibleInstancesService.Instance.BibleInstances;
    public ObservableCollection<BibleInstances> BookList { get; } = new();
    private ObservableCollection<Kitab>? _filteredBible = new();

    public ObservableCollection<Kitab>? FilteredBible
    {
        get => _filteredBible;
        set
        {
            _filteredBible = value;
            OnPropertyChanged();
        }
    }

    // DatabaseManager instance
    public readonly DatabaseManager _databaseManager;

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<NumberItemViewModel> Numbers { get; } = new();
    private AsyncRelayCommand<string?> NumberClicked { get; }
    public ICommand TogglePasalAyat { get; }

    public ToggleState ToggleState => ToggleService.Instance.ToggleState;

    private async Task NumberClickedAction(string? parameter)
    {
        if (parameter == "⌫")
        {
            ToggleState.PasalText = null;
            ToggleState.AyatText = null;
        }

        else if (parameter == "\u2936" && BibleInstances.BookName != null)
        {
            ToggleState.Pasal = ToggleState.PasalText;
            ToggleState.Ayat = ToggleState.AyatText;
            BibleInstances.SelectedBookName = BibleInstances.BookNameText;
            await FilterBible(BibleInstances.BookName, ToggleState.Pasal);
        }

        else if (ToggleState.Target == "pasal")
        {
            if (ToggleState.PasalText is { Length: 3 }) return;

            ToggleState.PasalText += parameter;
        }

        else if (ToggleState.Target == "ayat")
        {
            if (ToggleState.AyatText is { Length: 3 }) return;

            ToggleState.AyatText += parameter;
        }
    }

    // Constructor
    public MainWindowViewModel()
    {
        NumberClicked = new AsyncRelayCommand<string?>(NumberClickedAction);

        TogglePasalAyat = ReactiveCommand.Create<string?>(Toggle_PasalAyat);
        ToggleState.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ToggleState.Target))
                // This is now in instance context — should work
                OnPropertyChanged(nameof(ToggleState));
        };

        NavigateChapterCommand = ReactiveCommand.Create<object?>(NavigateChapter);
        ToggleBookmarkCommand = ReactiveCommand.Create<ObservableCollection<Kitab>>(OnToggleBookmark);
        
        Console.WriteLine("IsBookmarkEmpty : " +  IsBookmarkEmpty);

        var numberStrings = new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "⌫", "0", "\u2936" };
        foreach (var n in numberStrings) Numbers.Add(new NumberItemViewModel(n, NumberClicked));

        _databaseManager = new DatabaseManager();
        InitializeAsync();
    }

    private void Toggle_PasalAyat(string? parameter)
    {
        ToggleService.Instance.ToggleState.Target = parameter;
    }

    public event Action? BibleFiltered;

    public async Task FilterBible(string bookName, string? pasal)
    {
        Console.WriteLine("Filtering Bible...");
        await _databaseManager.FilterBible(bookName, pasal, FilteredBible);

        IsLoading = false;
        BibleInstances.BookName = "";
        ToggleState.PasalText = null;
        ToggleState.AyatText = null;
    }
    
    // ============= Fetching Database Methods =============
    private async void InitializeAsync()
    {
        await LoadBookList();
        // await GetAllBookmarks();
    }

    private async Task LoadBookList()
    {
        var bookList = await _databaseManager.GetBookList();

        if (bookList != null)
            foreach (var book in bookList)
                BookList.Add(book);
    }
    
    // ============= Navigation Methods =============
    public ICommand NavigateChapterCommand { get; }
    public Boolean NavigationMoved { get; set; }

    private async void NavigateChapter(object? parameter)
    {
        IsToolBoxVisible = false;
        OnPropertyChanged(nameof(IsToolBoxVisible));
        
        var bookName = parameter?.GetType().GetProperty("BookName")?.GetValue(parameter)?.ToString();
        var pasal = parameter?.GetType().GetProperty("Pasal")?.GetValue(parameter)?.ToString();
        var direction = parameter?.GetType().GetProperty("Direction")?.GetValue(parameter)?.ToString();

        var bibleByBookName = bookName != null ? await _databaseManager.GetBibleByBook(bookName) : null;
        var chapters = new List<string?>();
        
        foreach (var book in bibleByBookName)
        {
            if(!chapters.Contains(book.chapter))
                chapters.Add(book.chapter);
        }

        var previous = pasal != null ? (int.Parse(pasal) - 1).ToString() : "null";
        var next = pasal != null ? (int.Parse(pasal) + 1).ToString() : "null";
        
        if (direction == "left" && chapters.Contains(previous))
        {
            await _databaseManager.FilterBible(bookName, previous, FilteredBible);
            ToggleState.Pasal = previous;
        } else if (direction == "left" && !chapters.Contains(previous))
        {
            var currentIndex = BookList.Select((item, idx) => new { item, idx })
                .FirstOrDefault(x => x.item.BookName == bookName)?.idx ?? -1;
            
            if(currentIndex == 0)
            
            await _databaseManager.FilterBible(bookName, previous, FilteredBible);
            ToggleState.Pasal = previous;
        }
        else if (direction == "right" && chapters.Contains(next))
        {
            await _databaseManager.FilterBible(bookName, next, FilteredBible);
            ToggleState.Pasal = next;
        } else if (direction == "right" && !chapters.Contains(next))
        {
            Console.WriteLine("Reaching left max...");
        }
    }
    
    // ============= List Box Methods =============
    public ICommand SelectionChanged { get; }
    private ObservableCollection<Kitab> _selectedItems = new ();
    public ObservableCollection<Kitab> SelectedItems
    {
        get => _selectedItems;
        set
        {
            if (_selectedItems != value)
            {
                _selectedItems = value;
                OnPropertyChanged(nameof(SelectedItems));
            }
        }
    }
    
    // Using SelectedItems.Count to trigger visibility
    private bool _isToolBoxVisible;
    public bool IsToolBoxVisible
    {
        get => _isToolBoxVisible;
        set
        {
            if (_isToolBoxVisible != value)
            {
                _isToolBoxVisible = value;
                OnPropertyChanged();
            }
        }
    }

    public void OnSelectionChanged(ListBox listBox)
    {   
        SelectedItems.Clear();
        SelectedItems.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(SelectedItems));
        };
        
        foreach (var item in listBox.SelectedItems)
        {
            if (item is Kitab kitab)
            {
                SelectedItems.Add(kitab);
                OnPropertyChanged(nameof(SelectedItems));
                Console.WriteLine($"Verse: {kitab.verse}, Chapter: {kitab.chapter}, Book: {kitab.BookName}");
            }
        }

        IsToolBoxVisible = SelectedItems.Count > 0;
    }
    
    // ============= Tool Box Methods =============

    private ObservableCollection<Bookmark>? _bookmark = new();

    public ObservableCollection<Bookmark>? Bookmark
    {
        get => _bookmark;
        set
        {
            _bookmark = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsBookmarkEmpty));
        }
    }
    
    public bool IsBookmarkEmpty => _bookmark == null || _bookmark.Count == 0;

    public ICommand ToggleBookmarkCommand { get; }

    public async void OnToggleBookmark(ObservableCollection<Kitab> parameter)
    {
        foreach (var item in parameter)
        {
            await _databaseManager.InsertBookmark(item.BookName, item.chapter, item.verse);
        }
    }

    public async Task GetAllBookmarks()
    {
        await _databaseManager.GetAllBookmarks(Bookmark);
        OnPropertyChanged(nameof(Bookmark));
        Console.WriteLine("Bookmark => " + Bookmark);
    }
}