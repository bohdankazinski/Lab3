namespace JsonManager.Forms;

public class AboutForm : Form
{
    public AboutForm()
    {
        Text = "Про програму";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = MinimizeBox = false;
        ClientSize = new Size(420, 220);

        var lbl = new Label
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            Font = new Font("Segoe UI", 10),
            Text = "Лабораторна робота № 3\n" +
                   "Диспетчер для JSON (облік книг)\n\n" +
                   "ПІБ: Казінський Богдан В'ячеславович\n" +
                   "Курс: 2\n" +
                   "Група: К-27\n" +
                   "Рік: 2026\n\n" +
                   "Програма читає, редагує та зберігає список книг у форматі JSON."
        };
        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom, Height = 32 };
        Controls.Add(lbl);
        Controls.Add(ok);
        AcceptButton = ok;
    }
}
