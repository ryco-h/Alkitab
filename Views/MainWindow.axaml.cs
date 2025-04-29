using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Alkitab.Models;
using Alkitab.Objects;
using Alkitab.Services;
using Alkitab.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using DialogHostAvalonia;

namespace Alkitab.Views;

public partial class MainWindow : Window
{
    private readonly Image? _bibleImage;
    private readonly Image? _infoIcon;
    private DialogHost? _dialog;
    private readonly AutoCompleteBox? _autoCompleteBox;
    private readonly ListBox? _listBox;
    private readonly ScrollViewer? _scrollViewer;

    public MainWindow()
    {
        InitializeComponent();
        ToggleService.Instance.ToggleState.PropertyChanged += ToggleState_PropertyChanged;

        DataContext = new MainWindowViewModel();
        Opened += (_, _) =>
        {
            RestoreWindowBounds();
            LoadSavedBook();
        };
        Closing += (_, _) =>
        {
            SaveWindowBounds();
            SaveLoadedBook();
        };

#if DEBUG
        this.AttachDevTools();
#endif

        _bibleImage = this.FindControl<Image>("BibleImage");
        _infoIcon = this.FindControl<Image>("InfoIcon");
        _dialog = this.FindControl<DialogHost>("dialogHost");
        _autoCompleteBox = this.FindControl<AutoCompleteBox>("autoCompleteBox");
        _scrollViewer = this.FindControl<ScrollViewer>("BibleScrollViewer");
        _listBox = this.FindControl<ListBox>("ScrollBible");

        if (_bibleImage != null)
        {
            _bibleImage.PointerEntered += BibleImage_PointerEnter;
            _bibleImage.PointerExited += BibleImage_PointerLeave;
            _bibleImage.PointerPressed += BibleImage_PointerPressed;
        }

        if (_infoIcon != null)
            _infoIcon.PointerPressed += InfoIcon_PointerPressed;

        if (_autoCompleteBox != null) _autoCompleteBox.GotFocus += ShowDropDown;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void BibleImage_PointerEnter(object? sender, PointerEventArgs e)
    {
        if (_bibleImage != null)
            _bibleImage.Source = new Bitmap(
                AssetLoader.Open(new Uri("avares://Alkitab/Assets/bookOW.png"))
            );
    }

    private void BibleImage_PointerLeave(object? sender, PointerEventArgs e)
    {
        if (_bibleImage != null)
            _bibleImage.Source = new Bitmap(
                AssetLoader.Open(new Uri("avares://Alkitab/Assets/bookCW.png"))
            );
    }

    private async void BibleImage_PointerPressed(object? sender, PointerEventArgs e)
    {
        await DialogHost.Show(Resources["SearchBible"]!, "MainDialogHost");
    }
    
    private async void InfoIcon_PointerPressed(object? sender, PointerEventArgs e)
    {
        await DialogHost.Show(Resources["ChangeLogView"]!, "MainDialogHost");
    }

    private void ShowDropDown(object? sender, GotFocusEventArgs e)
    {
        if (_autoCompleteBox != null) _autoCompleteBox.IsDropDownOpen = true;
    }

    // ========== Save & Load Configurations ==========
    private const string SettingsFile = "window-settings.json";

    private void SaveWindowBounds()
    {
        var settings = new WindowSettings
        {
            X = Position.X,
            Y = Position.Y,
            Width = Width,
            Height = Height
        };

        File.WriteAllText(SettingsFile, JsonSerializer.Serialize(settings));
    }

    private void RestoreWindowBounds()
    {
        if (File.Exists(SettingsFile))
            try
            {
                var json = File.ReadAllText(SettingsFile);
                var settings = JsonSerializer.Deserialize<WindowSettings>(json);

                if (settings != null)
                {
                    Position = new PixelPoint((int)settings.X, (int)settings.Y);
                    Width = settings.Width;
                    Height = settings.Height;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to restore window bounds: {ex.Message}");
            }
    }

    private const string BibleFile = "bible-settings.json";

    private void SaveLoadedBook()
    {
        if (BibleInstancesService.Instance.BibleInstances.SelectedBookName != null &&
            ToggleService.Instance.ToggleState.Pasal != null
            )
        {
            var bibleData = new BibleSettings()
            {
                SelectedBook = BibleInstancesService.Instance.BibleInstances.SelectedBookName,
                Pasal = ToggleService.Instance.ToggleState.Pasal,
            };
            
            File.WriteAllText(BibleFile, JsonSerializer.Serialize(bibleData));
        }
    }

    private async void LoadSavedBook()
    {
        if (File.Exists(BibleFile))
            try
            {
                var json = File.ReadAllText(BibleFile);
                var bibleData = JsonSerializer.Deserialize<BibleSettings>(json);

                if (bibleData != null)
                {
                    BibleInstancesService.Instance.BibleInstances.SelectedBookName = bibleData.SelectedBook;
                    ToggleService.Instance.ToggleState.Pasal = bibleData.Pasal;
                }

                if (DataContext is MainWindowViewModel vm)
                {
                    await vm.FilterBible(bibleData.SelectedBook, bibleData.Pasal);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to restore saved book: {ex.Message}");
            }
    }
    
    private void ToggleState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToggleState.Ayat))
        {
            Dispatcher.UIThread.Post(async () =>
            {
                await Task.Delay(100);
                ScrollToAyat();
            });
        }
    }

    private async void ScrollToAyat()
    {
        if (_listBox != null
            && !string.IsNullOrEmpty(ToggleService.Instance.ToggleState.Ayat)
            && int.TryParse(ToggleService.Instance.ToggleState.Ayat, out var ayatNumber)
            && ayatNumber != 0)
        {
            var vm = DataContext as MainWindowViewModel;
            if (vm?.FilteredBible == null) return;

            var item = vm.FilteredBible.FirstOrDefault(x => int.Parse(x.verse) == ayatNumber);

            if (item != null)
            {
                ScrollItemToTop(item);
            }
        }
    }

    private void ScrollItemToTop(object? item)
    {

        if (item == null)
        {
            Console.WriteLine("[Scroll] Item is null");
            return;
        }

        if (_listBox == null)
        {
            Console.WriteLine("[Scroll] _listBox is null");
            return;
        }

        if (_scrollViewer == null)
        {
            Console.WriteLine("[Scroll] _scrollViewer is null");
            return;
        }

        var index = _listBox.Items.IndexOf(item);
        
        if (index < 0)
        {
            Console.WriteLine("[Scroll] Item not found in ListBox items");
            return;
        }

        _listBox.ScrollIntoView(item);

        Dispatcher.UIThread.Post(() =>
        {
            var container = _listBox.ItemContainerGenerator.ContainerFromIndex(index);

            if (container == null)
            {
                Console.WriteLine("[Scroll] Container is null");
                return;
            }

            var itemBounds = container.Bounds;
            Console.WriteLine($"[Scroll] Scrolling to Y={itemBounds.Top}");

            _scrollViewer.Offset = new Vector(_scrollViewer.Offset.X, itemBounds.Top);
        }, DispatcherPriority.Background);
    }

    private void ScrollBible_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {   
        var listBox = sender as ListBox;
        
        if (DataContext is MainWindowViewModel vm && listBox != null)
        {
            vm.OnSelectionChanged(listBox); // This method is in your ViewModel
        }
    }

    private async void BookmarkPanel_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.Bookmark != null)
        {
            await vm._databaseManager.GetAllBookmarks(vm.Bookmark);
            
            var bookmarkPanel = new BookmarkPanel
            {
                DataContext = vm
            };

            await DialogHost.Show(bookmarkPanel, "MainDialogHost");
        }
    }
}