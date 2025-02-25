using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Alkitab.Views;

using Avalonia;
using Avalonia.Markup.Xaml;
using DialogHostAvalonia;

public partial class MainWindow : Window
{
    private Image? _bibleImage;
    private DialogHost? _dialog;
    private AutoCompleteBox? _autoCompleteBox;

    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif

        // Find the Image control AFTER the UI has loaded
        _bibleImage = this.FindControl<Image>("BibleImage");
        _dialog = this.FindControl<DialogHost>("dialogHost");
        _autoCompleteBox = this.FindControl<AutoCompleteBox>("autoCompleteBox");

        if (_bibleImage != null)
        {
            _bibleImage.PointerEntered += BibleImage_PointerEnter;
            _bibleImage.PointerExited += BibleImage_PointerLeave;
            _bibleImage.PointerPressed += BibleImage_PointerPressed;
        }

        if (_autoCompleteBox != null)
        {
            _autoCompleteBox.GotFocus += ShowDropDown;
            _autoCompleteBox.SelectionChanged += SelectedBook;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void BibleImage_PointerEnter(object? sender, PointerEventArgs e)
    {
        if (_bibleImage != null)
        {
            _bibleImage.Source = new Bitmap(
                AssetLoader.Open(new Uri("avares://Alkitab/Assets/bookOW.png"))
            );
        }
    }

    private void BibleImage_PointerLeave(object? sender, PointerEventArgs e)
    {
        if (_bibleImage != null)
        {
            _bibleImage.Source = new Bitmap(
                AssetLoader.Open(new Uri("avares://Alkitab/Assets/bookCW.png"))
            );
        }
    }

    private async void BibleImage_PointerPressed(object? sender, PointerEventArgs e)
    {
        await DialogHost.Show(Resources["SearchBible"]!, "MainDialogHost");
    }

    private void ShowDropDown(object? sender, GotFocusEventArgs e)
    {
        if (_autoCompleteBox != null)
        {
            _autoCompleteBox.IsDropDownOpen = true;
        }
    }

    private void SelectedBook(object? sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine("Selected Book: " + _autoCompleteBox?.SelectedItem?.ToString());
    }
}
