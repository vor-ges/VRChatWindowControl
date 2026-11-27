using System.IO;
namespace VRChatWindowControl;

public partial class Form1 : Form
{
    private readonly NotifyIcon trayIcon;
    private readonly ContextMenuStrip trayMenu;

    private readonly ToolStripMenuItem pauseItem;
    private readonly ToolStripMenuItem resumeItem;

    private readonly VRChatMonitor monitor;

    private AppSettings settings;

    // --tray で起動された場合は、最初のウィンドウ表示を抑止する
    private readonly bool startInTray;
    private bool initialVisibilityHandled;

    public Form1(bool startInTray = false)
    {
        this.startInTray = startInTray;

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

        trayMenu.Items.Add("設定", null, (_, _) =>
        {
            ShowSettings();
        });

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

        trayMenu.Items.Add("終了", null, (_, _) =>
        {
            Application.Exit();
        });

        resumeItem.Enabled = false;

        trayIcon.ContextMenuStrip = trayMenu;
        trayIcon.DoubleClick += (_, _) => ShowWindow();

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

    // スタートアップ起動時は、最初のウィンドウ表示を抑止する
    protected override void SetVisibleCore(bool value)
    {
        if (value && startInTray && !initialVisibilityHandled)
        {
            initialVisibilityHandled = true;
            value = false;
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

                // 監視処理にも新しいサイズを反映
                monitor.WindowWidth = settings.Width;
                monitor.WindowHeight = settings.Height;

                MessageBox.Show(
                    "Settings saved successfully.",
                    "VRChat Window Control",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not save settings:\n{ex.Message}",
                    "VRChat Window Control",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }

    // メインウィンドウを表示
    private void ShowWindow()
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
    }
}
