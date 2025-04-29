namespace Alkitab.Models;

public class Bookmark
{
    public required string id { get; set; }
    public required string bookname { get; set; }
    public required string chapter { get; set; }
    public required string verse { get; set; }
    public required string created_at { get; set; }
    
    public override string ToString()
    {
        return $"{bookname} {chapter}:{verse}";
    }
}