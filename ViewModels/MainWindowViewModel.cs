using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Alkitab.Models;
using Alkitab.Services;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace Alkitab.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public BibleInstances BibleInstances => BibleInstancesService.Instance.BibleInstances;
    public ObservableCollection<BibleInstances> BookList { get; } = new();
    private IEnumerable<Kitab>? _filteredBible;

    public IEnumerable<Kitab> FilteredBible
    {
        get => _filteredBible;
        set
        {
            _filteredBible = value;
            OnPropertyChanged();
        }
    }

    // DatabaseManager instance
    private readonly DatabaseManager _databaseManager;

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
            await FilterBible(BibleInstances.BookName, ToggleState.Pasal, ToggleState.Ayat);
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

    private async Task FilterBible(string bookName, string? pasal, string? ayat)
    {
        FilteredBible = await _databaseManager.FilterBible(bookName, pasal);

        OnPropertyChanged(nameof(FilteredBible));
        BibleFiltered?.Invoke();

        IsLoading = false;
        BibleInstances.BookName = "";
        ToggleState.PasalText = null;
        ToggleState.AyatText = null;
    }
    
    // ============= Fetching Database Methods =============
    private async void InitializeAsync()
    {
        await LoadBookList();
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

    private async void NavigateChapter(object? parameter)
    {
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
            FilteredBible = await _databaseManager.FilterBible(bookName, previous);
            ToggleState.Pasal = previous;

            OnPropertyChanged(nameof(FilteredBible));
            BibleFiltered?.Invoke();
        }
        else if (direction == "right" && chapters.Contains(next))
        {
            FilteredBible = await _databaseManager.FilterBible(bookName, next);
            ToggleState.Pasal = next;

            OnPropertyChanged(nameof(FilteredBible));
            BibleFiltered?.Invoke();
        } else
        {
            return;
        }
        
        
        Console.WriteLine(string.Join(", ", chapters));
        Console.WriteLine("chapters => " + chapters);
    }
}