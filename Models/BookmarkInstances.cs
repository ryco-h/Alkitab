using System.Collections.ObjectModel;

namespace Alkitab.Models;

public class BookmarkInstances : ObservableCollection<Bookmark>
{
    public ObservableCollection<Bookmark> Bookmark { get; set; } = new ();
}