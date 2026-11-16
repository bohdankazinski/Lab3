using JsonManager.Models;
using JsonManager.Services;

namespace JsonManager.Forms;

public class MainForm : Form
{
    private List<Book> _books = new();
    private string? _currentPath;

    private readonly DataGridView _grid = new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AutoGenerateColumns = false
    };
    private readonly TextBox _searchText = new() { Width = 150 };
    private readonly ComboBox _searchGenre = new() { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _yearFrom = new() { Width = 65, Minimum = 0, Maximum = 2100, Value = 0 };
    private readonly NumericUpDown _yearTo = new() { Width = 65, Minimum = 0, Maximum = 2100, Value = 0 };
    private readonly StatusStrip _status = new();
    private readonly ToolStripStatusLabel _statusLabel = new();

    public MainForm()
    {
        Text = "Диспетчер JSON — Облік книг";
        Size = new Size(950, 550);
        StartPosition = FormStartPosition.CenterScreen;

        BuildMenu();
        BuildSearchPanel();
        BuildGrid();
        _status.Items.Add(_statusLabel);
        Controls.Add(_status);
        Controls.SetChildIndex(_grid, 0);
        Controls.SetChildIndex(MainMenuStrip!, Controls.Count - 1);
        _grid.RowHeadersVisible = false;
        RefreshGrid();
    }

    private void BuildMenu()
    {
        var menu = new MenuStrip();
        var file = new ToolStripMenuItem("Файл");
        file.DropDownItems.Add("Відкрити...", null, (_, _) => OpenFile());
        file.DropDownItems.Add("Зберегти", null, (_, _) => SaveFile(false));
        file.DropDownItems.Add("Зберегти як...", null, (_, _) => SaveFile(true));
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add("Вихід", null, (_, _) => Close());

        var edit = new ToolStripMenuItem("Дані");
        edit.DropDownItems.Add("Додати", null, (_, _) => AddBook());
        edit.DropDownItems.Add("Редагувати", null, (_, _) => EditBook());
        edit.DropDownItems.Add("Видалити", null, (_, _) => DeleteBook());

        var help = new ToolStripMenuItem("Довідка");
        help.DropDownItems.Add("Про програму", null, (_, _) => new AboutForm().ShowDialog(this));

        menu.Items.AddRange(new ToolStripItem[] { file, edit, help });
        MainMenuStrip = menu;
        Controls.Add(menu);
    }

    private void BuildSearchPanel()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(5) };

        var add = new Button { Text = "Додати", Width = 80 };
        var edit = new Button { Text = "Редагувати", Width = 90 };
        var del = new Button { Text = "Видалити", Width = 80 };
        add.Click += (_, _) => AddBook();
        edit.Click += (_, _) => EditBook();
        del.Click += (_, _) => DeleteBook();

        _searchGenre.Items.Add("(усі жанри)");
        _searchGenre.Items.AddRange(new object[] { "Роман", "Фантастика", "Детектив", "Наукова", "Поезія", "Інше" });
        _searchGenre.SelectedIndex = 0;

        var find = new Button { Text = "Пошук", Width = 70 };
        var reset = new Button { Text = "Скинути", Width = 70 };
        find.Click += (_, _) => RefreshGrid();
        reset.Click += (_, _) =>
        {
            _searchText.Clear(); _searchGenre.SelectedIndex = 0;
            _yearFrom.Value = 0; _yearTo.Value = 0;
            RefreshGrid();
        };

        Label L(string t) => new() { Text = t, AutoSize = true, Margin = new Padding(8, 7, 0, 0) };
        panel.Controls.AddRange(new Control[]
        {
            add, edit, del,
            L("Назва/автор:"), _searchText,
            L("Жанр:"), _searchGenre,
            L("Рік від:"), _yearFrom, L("до:"), _yearTo,
            find, reset
        });
        Controls.Add(panel);
    }

    private void BuildGrid()
    {
        void Col(string prop, string header) =>
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = prop, HeaderText = header });
        Col(nameof(Book.Title), "Назва");
        Col(nameof(Book.Author), "Автор");
        Col(nameof(Book.Genre), "Жанр");
        Col(nameof(Book.Year), "Рік");
        Col(nameof(Book.Pages), "Сторінок");
        _grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(Book.Available), HeaderText = "В наявності" });
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) EditBook(); };
        Controls.Add(_grid);
    }

    private List<Book> ApplyFilter()
    {
        IEnumerable<Book> q = _books;

        string text = _searchText.Text.Trim();
        if (text.Length > 0)
            q = q.Where(b => b.Title.Contains(text, StringComparison.CurrentCultureIgnoreCase)
                          || b.Author.Contains(text, StringComparison.CurrentCultureIgnoreCase));

        if (_searchGenre.SelectedIndex > 0)
        {
            string genre = _searchGenre.Text;
            q = q.Where(b => b.Genre.Equals(genre, StringComparison.CurrentCultureIgnoreCase));
        }

        int from = (int)_yearFrom.Value, to = (int)_yearTo.Value;
        if (from > 0) q = q.Where(b => b.Year >= from);
        if (to > 0) q = q.Where(b => b.Year <= to);

        return q.OrderBy(b => b.Title).ToList();
    }

    private void RefreshGrid()
    {
        var shown = ApplyFilter();
        _grid.DataSource = null;
        _grid.DataSource = shown;
        _statusLabel.Text = $"Файл: {_currentPath ?? "(не відкрито)"}   |   Показано: {shown.Count} з {_books.Count}";
    }

    private Book? Selected => _grid.CurrentRow?.DataBoundItem as Book;

    private void AddBook()
    {
        using var f = new EditForm();
        if (f.ShowDialog(this) == DialogResult.OK)
        {
            _books.Add(f.Result);
            RefreshGrid();
        }
    }

    private void EditBook()
    {
        var book = Selected;
        if (book == null) { MessageBox.Show("Оберіть рядок."); return; }
        using var f = new EditForm(book);
        if (f.ShowDialog(this) == DialogResult.OK)
        {
            int i = _books.IndexOf(book);
            _books[i] = f.Result;
            RefreshGrid();
        }
    }

    private void DeleteBook()
    {
        var book = Selected;
        if (book == null) { MessageBox.Show("Оберіть рядок."); return; }
        if (MessageBox.Show($"Видалити «{book.Title}»?", "Підтвердження",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _books.Remove(book);
            RefreshGrid();
        }
    }

    private const string Filter = "JSON файли (*.json)|*.json|Усі файли (*.*)|*.*";

    private void OpenFile()
    {
        using var dlg = new OpenFileDialog { Filter = Filter };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _books = JsonStorage.Load(dlg.FileName);
            _currentPath = dlg.FileName;
            RefreshGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не вдалося прочитати файл:\n" + ex.Message, "Помилка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveFile(bool saveAs)
    {
        string? path = _currentPath;
        if (saveAs || path == null)
        {
            using var dlg = new SaveFileDialog { Filter = Filter, DefaultExt = "json" };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;
            path = dlg.FileName;
        }
        try
        {
            JsonStorage.Save(path, _books);
            _currentPath = path;
            RefreshGrid();
            MessageBox.Show("Збережено.", "Готово");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не вдалося зберегти файл:\n" + ex.Message, "Помилка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
