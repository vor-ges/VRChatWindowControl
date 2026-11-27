using System;
using System.Drawing;
using System.Windows.Forms;

namespace VRChatWindowControl;

internal sealed class SettingsForm : Form
{
    private readonly NumericUpDown widthInput;
    private readonly NumericUpDown heightInput;

    public AppSettings Settings { get; private set; }

    public SettingsForm(AppSettings currentSettings)
    {
        Settings = new AppSettings
        {
            Width = currentSettings.Width,
            Height = currentSettings.Height
        };

        Text = "VRChat Window Control - Settings";
        Size = new Size(340, 220);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        var widthLabel = new Label
        {
            Text = "Width (px)",
            Location = new Point(20, 25),
            Size = new Size(110, 25)
        };

        widthInput = new NumericUpDown
        {
            Location = new Point(145, 22),
            Size = new Size(150, 25),
            Minimum = 100,
            Maximum = 7680,
            Value = currentSettings.Width
        };

        var heightLabel = new Label
        {
            Text = "Height (px)",
            Location = new Point(20, 65),
            Size = new Size(110, 25)
        };

        heightInput = new NumericUpDown
        {
            Location = new Point(145, 62),
            Size = new Size(150, 25),
            Minimum = 100,
            Maximum = 4320,
            Value = currentSettings.Height
        };

        var saveButton = new Button
        {
            Text = "Save",
            Location = new Point(125, 120),
            Size = new Size(80, 30),
            DialogResult = DialogResult.OK
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(215, 120),
            Size = new Size(80, 30),
            DialogResult = DialogResult.Cancel
        };

        saveButton.Click += (_, _) =>
        {
            Settings.Width = (int)widthInput.Value;
            Settings.Height = (int)heightInput.Value;
        };

        Controls.AddRange(new Control[]
        {
            widthLabel,
            widthInput,
            heightLabel,
            heightInput,
            saveButton,
            cancelButton
        });

        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }
}
