using System;
using System.Linq;
using System.Threading.Tasks;
using Alkitab.Models;
using Alkitab.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace Alkitab.Views;

public partial class BookmarkPanel : UserControl
{
    public BookmarkPanel()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void DotsButton_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.ContextFlyout is MenuFlyout flyout)
        {
            flyout.ShowAt(control);
        }
    }

    private async void FlyoutMenuDelete_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && sender is MenuItem menuItem &&
            menuItem.GetVisualAncestors().OfType<Control>().FirstOrDefault() is Control parentControl &&
            parentControl.DataContext is Bookmark bookmark)
        {
            await vm._databaseManager.RemoveBookmark(bookmark.id, vm.Bookmark);
        }
        // await DialogHost.Show(Resources["BookmarkPanel"]!, "MainDialogHost");
    }
}