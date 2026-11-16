namespace JsonManager.Models;

public class Book
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Genre { get; set; } = "";
    public int Year { get; set; } = DateTime.Now.Year;
    public int Pages { get; set; } = 1;
    public bool Available { get; set; } = true;
}
