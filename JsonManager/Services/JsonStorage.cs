using System.Text.Json;
using JsonManager.Models;

namespace JsonManager.Services;

public static class JsonStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static List<Book> Load(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<Book>>(json, Options) ?? new List<Book>();
    }

    public static void Save(string path, IEnumerable<Book> books)
    {
        string json = JsonSerializer.Serialize(books, Options);
        File.WriteAllText(path, json);
    }
}
