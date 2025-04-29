using System.Collections.ObjectModel;
using Alkitab.Models;

namespace Alkitab.Services;

public class BookmarkService
{
    public ObservableCollection<Bookmark> Bookmark { get; } = new BookmarkInstances();

    private static BookmarkService? _instance;
    public static BookmarkService Instance => _instance ??= new BookmarkService();

    private BookmarkService() {}
}