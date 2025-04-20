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
    private IEnumerable<Kitab> _filteredBible;

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

    private async void InitializeAsync()
    {
        await LoadBookList();
        GetProperties();
    }

    private void GetProperties()
    {
        // Get the Type object for Kitab
        var kitabType = typeof(Kitab);

        // Get the properties of the Kitab class
        var properties = kitabType.GetProperties();
    }

    private async Task LoadBookList()
    {
        var bookList = await _databaseManager.GetBookList(); // Make sure this returns IEnumerable<Kitab>

        if (bookList != null)
            foreach (var book in bookList)
                BookList.Add(book);
    }
}