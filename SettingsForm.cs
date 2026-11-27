using System;
using System.Drawing;
using System.Windows.Forms;

namespace VRChatWindowControl;

internal sealed class SettingsForm : Form
{
    private readonly NumericUpDown widthInput;
    private readonly NumericUpDown heightInput;
    private readonly ComboBox languageInput;
    private readonly Label widthLabel;
    private readonly Label heightLabel;
    private readonly Label languageLabel;
    private readonly Button saveButton;
    private readonly Button cancelButton;

    public AppSettings Settings { get; private set; }

    public SettingsForm(AppSettings currentSettings)
    {
        Settings = new AppSettings
        {
            Width = currentSettings.Width,
            Height = currentSettings.Height,
            Language = AppLanguage.NormalizeCode(currentSettings.Language)
        };

        Size = new Size(390, 285);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;

        widthLabel = new Label
        {
            Location = new Point(20, 25),
            Size = new Size(110, 25)
        };

        widthInput = new NumericUpDown
        {
            Location = new Point(145, 22),
            Size = new Size(200, 25),
            Minimum = 100,
            Maximum = 7680,
            Value = currentSettings.Width
        };

        heightLabel = new Label
        {
            Location = new Point(20, 65),
            Size = new Size(110, 25)
        };

        heightInput = new NumericUpDown
        {
            Location = new Point(145, 62),
            Size = new Size(200, 25),
            Minimum = 100,
            Maximum = 4320,
            Value = currentSettings.Height
        };

        languageLabel = new Label
        {
            Location = new Point(20, 105),
            Size = new Size(110, 25)
        };

        languageInput = new ComboBox
        {
            Location = new Point(145, 102),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            DataSource = AppLanguage.Options.ToArray(),
            DisplayMember = nameof(LanguageOption.DisplayName),
            ValueMember = nameof(LanguageOption.Code),
            SelectedValue = Settings.Language
        };

        saveButton = new Button
        {
            Location = new Point(155, 160),
            Size = new Size(90, 30),
            DialogResult = DialogResult.OK
        };

        cancelButton = new Button
        {
            Location = new Point(255, 160),
            Size = new Size(90, 30),
            DialogResult = DialogResult.Cancel
        };

        saveButton.Click += (_, _) =>
        {
            Settings.Width = (int)widthInput.Value;
            Settings.Height = (int)heightInput.Value;
            Settings.Language = SelectedLanguage;
        };

        languageInput.SelectedValueChanged += (_, _) =>
        {
            ApplyLanguage(AppLanguage.GetText(SelectedLanguage));
        };

        Controls.AddRange(new Control[]
        {
            widthLabel,
            widthInput,
            heightLabel,
            heightInput,
            languageLabel,
            languageInput,
            saveButton,
            cancelButton
        });

        ApplyLanguage(AppLanguage.GetText(Settings.Language));

        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private string SelectedLanguage =>
        AppLanguage.NormalizeCode(languageInput.SelectedValue as string);

    private void ApplyLanguage(AppLanguageText text)
    {
        Text = text.SettingsTitle;
        widthLabel.Text = text.WidthLabel;
        heightLabel.Text = text.HeightLabel;
        languageLabel.Text = text.LanguageLabel;
        saveButton.Text = text.SaveButton;
        cancelButton.Text = text.CancelButton;
    }
}
