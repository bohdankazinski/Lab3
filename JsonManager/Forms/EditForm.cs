using JsonManager.Models;

namespace JsonManager.Forms;

public class EditForm : Form
{
    private readonly TextBox _title = new(), _author = new();
    private readonly ComboBox _genre = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private readonly NumericUpDown _year = new() { Minimum = 1000, Maximum = 2100 };
    private readonly NumericUpDown _pages = new() { Minimum = 1, Maximum = 10000 };
    private readonly CheckBox _available = new() { Text = "Є в наявності" };

    public Book Result { get; private set; }

    public EditForm(Book? book = null)
    {
        Result = book != null
            ? new Book { Title = book.Title, Author = book.Author, Genre = book.Genre, Year = book.Year, Pages = book.Pages, Available = book.Available }
            : new Book();

        Text = book == null ? "Додати книгу" : "Редагувати книгу";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = MinimizeBox = false;
        ClientSize = new Size(380, 280);

        _genre.Items.AddRange(new object[] { "Роман", "Фантастика", "Детектив", "Наукова", "Поезія", "Інше" });

        var table = new TableLayoutPanel { Dock = DockStyle.Top, Height = 220, ColumnCount = 2, Padding = new Padding(10) };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        void Row(string label, Control c)
        {
            table.Controls.Add(new Label { Text = label, Anchor = AnchorStyles.Left, AutoSize = true });
            c.Dock = DockStyle.Fill;
            table.Controls.Add(c);
        }
        Row("Назва:", _title);
        Row("Автор:", _author);
        Row("Жанр:", _genre);
        Row("Рік видання:", _year);
        Row("Сторінок:", _pages);
        Row("", _available);

        var ok = new Button { Text = "Зберегти", Left = 190, Top = 235, Width = 85 };
        var cancel = new Button { Text = "Скасувати", Left = 285, Top = 235, Width = 85, DialogResult = DialogResult.Cancel };
        ok.Click += OnOk;
        Controls.AddRange(new Control[] { table, ok, cancel });
        AcceptButton = ok;
        CancelButton = cancel;

        _title.Text = Result.Title;
        _author.Text = Result.Author;
        _genre.Text = Result.Genre;
        _year.Value = Math.Clamp(Result.Year, 1000, 2100);
        _pages.Value = Math.Clamp(Result.Pages, 1, 10000);
        _available.Checked = Result.Available;
    }

    private void OnOk(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_title.Text) || string.IsNullOrWhiteSpace(_author.Text))
        {
            MessageBox.Show("Заповніть назву та автора.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        Result.Title = _title.Text.Trim();
        Result.Author = _author.Text.Trim();
        Result.Genre = _genre.Text.Trim();
        Result.Year = (int)_year.Value;
        Result.Pages = (int)_pages.Value;
        Result.Available = _available.Checked;
        DialogResult = DialogResult.OK;
    }
}
