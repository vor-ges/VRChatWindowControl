using System.IO;
namespace VRChatWindowControl;

public partial class Form1 : Form
{
    private readonly NotifyIcon trayIcon;
    private readonly ContextMenuStrip trayMenu;

    private readonly ToolStripMenuItem pauseItem;
    private readonly ToolStripMenuItem resumeItem;
    private readonly ToolStripMenuItem settingsItem;
    private readonly ToolStripMenuItem exitItem;

    private readonly VRChatMonitor monitor;

    private AppSettings settings;

    public Form1()
	{

        InitializeComponent();

        // 設定を読み込む
        settings = SettingsManager.Load();

        // 監視処理を初期化
        monitor = new VRChatMonitor
        {
            WindowWidth = settings.Width,
            WindowHeight = settings.Height
        };

        // タスクトレイアイコン
        trayIcon = new NotifyIcon
        {
            Text = "VRChat Window Control",
            Icon = new Icon(
                Path.Combine(AppContext.BaseDirectory, "app.ico")
            ),
            Visible = true
        };

        // タスクトレイメニュー
        trayMenu = new ContextMenuStrip();

        settingsItem = new ToolStripMenuItem();
        settingsItem.Click += (_, _) =>
        {
            ShowSettings();
        };
        trayMenu.Items.Add(settingsItem);

        pauseItem = new ToolStripMenuItem("一時停止");
        resumeItem = new ToolStripMenuItem("再開");

        pauseItem.Click += (_, _) =>
        {
            monitor.Paused = true;
            pauseItem.Enabled = false;
            resumeItem.Enabled = true;
        };

        resumeItem.Click += (_, _) =>
        {
            monitor.Paused = false;
            pauseItem.Enabled = true;
            resumeItem.Enabled = false;
        };

        trayMenu.Items.Add(pauseItem);
        trayMenu.Items.Add(resumeItem);

        trayMenu.Items.Add(new ToolStripSeparator());

        exitItem = new ToolStripMenuItem();
        exitItem.Click += (_, _) =>
        {
            Application.Exit();
        };
        trayMenu.Items.Add(exitItem);

        resumeItem.Enabled = false;

        trayIcon.ContextMenuStrip = trayMenu;
        trayIcon.DoubleClick += (_, _) => ShowSettings();
        ApplyLanguage();

        // ×ボタンで終了せず、非表示にする
        FormClosing += (_, e) =>
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        };

        // アプリ終了時の後処理
        FormClosed += (_, _) =>
        {
            monitor.Dispose();

            trayIcon.Visible = false;
            trayIcon.Dispose();
            trayMenu.Dispose();
        };

        // 監視開始
        monitor.Start();
    }

    // 起動時はメインウィンドウを表示せず、タスクトレイに常駐する
	protected override void SetVisibleCore(bool value)
	{
    	if (!IsHandleCreated && value)
    	{
        	value = false;
        	CreateHandle();
    	}

    	base.SetVisibleCore(value);
	}

    // 設定画面を開く
    private void ShowSettings()
    {
        using var dialog = new SettingsForm(settings);

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                SettingsManager.Save(dialog.Settings);

                settings = dialog.Settings;

                ApplyLanguage();

                // 監視処理にも新しいサイズを反映
                monitor.WindowWidth = settings.Width;
                monitor.WindowHeight = settings.Height;

                AppLanguageText text = AppLanguage.GetText(settings.Language);
                MessageBox.Show(
                    text.SaveSuccess,
                    "VRChat Window Control",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                AppLanguageText text = AppLanguage.GetText(dialog.Settings.Language);
                MessageBox.Show(
                    text.SaveErrorPrefix + ex.Message,
                    "VRChat Window Control",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }

    private void ApplyLanguage()
    {
        AppLanguageText text = AppLanguage.GetText(settings.Language);
        settingsItem.Text = text.TraySettings;
        pauseItem.Text = text.TrayPause;
        resumeItem.Text = text.TrayResume;
        exitItem.Text = text.TrayExit;
    }
}
