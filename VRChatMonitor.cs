
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace VRChatWindowControl;

internal sealed class VRChatMonitor : IDisposable
{
    // 監視間隔
    private const int PollMilliseconds = 1000;

    // SteamVR起動後の検出期限
    private const int DetectionTimeoutSeconds = 120;

    // ウィンドウ操作用のフラグ
    private const uint WindowPositionFlags = 0x0016;

    // 最小化
    private const int MinimizeWindow = 6;

    private readonly System.Windows.Forms.Timer timer;

    private bool vrServerWasRunning;
    private bool windowProcessed;
    private DateTime detectionDeadline;

    // 一時停止状態
    public bool Paused { get; set; }

    // 変更するウィンドウサイズ
    public int WindowWidth { get; set; } = 400;
    public int WindowHeight { get; set; } = 300;

    public VRChatMonitor()
    {
        timer = new System.Windows.Forms.Timer
        {
            Interval = PollMilliseconds
        };

        timer.Tick += OnTimerTick;
    }

    // 監視開始
    public void Start()
    {
        timer.Start();
    }

    // 監視終了
    public void Stop()
    {
        timer.Stop();
    }

    // 1秒ごとの監視処理
    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (Paused)
        {
            return;
        }

        // SteamVRの起動状態を確認
        bool vrServerRunning = IsProcessRunning("vrserver");

        // SteamVRが新しく起動した場合
        if (vrServerRunning && !vrServerWasRunning)
        {
            detectionDeadline = DateTime.Now.AddSeconds(
                DetectionTimeoutSeconds
            );

            windowProcessed = false;
        }

        // SteamVRが終了した場合
        if (!vrServerRunning)
        {
            windowProcessed = false;
        }

        vrServerWasRunning = vrServerRunning;

        // SteamVRが起動していない、または処理済みなら終了
        if (!vrServerRunning || windowProcessed)
        {
            return;
        }

        // 検出期限を過ぎていたら終了
        if (DateTime.Now > detectionDeadline)
        {
            return;
        }

        // VRChatのウィンドウを探す
        Process[] processes = Process.GetProcessesByName("VRChat");

        foreach (Process process in processes)
        {
            using (process)
            {
                IntPtr hWnd = process.MainWindowHandle;

                // メインウィンドウが存在しない場合は次へ
                if (hWnd == IntPtr.Zero)
                {
                    continue;
                }

                // ウィンドウサイズを変更
                NativeMethods.SetWindowPos(
                    hWnd,
                    IntPtr.Zero,
                    0,
                    0,
                    WindowWidth,
                    WindowHeight,
                    WindowPositionFlags
                );

                // ウィンドウを最小化
                NativeMethods.ShowWindowAsync(
                    hWnd,
                    MinimizeWindow
                );

                // 処理済みにする
                windowProcessed = true;

                break;
            }
        }
    }

    // 指定した名前のプロセスが存在するか確認
    private static bool IsProcessRunning(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);

        try
        {
            return processes.Length > 0;
        }
        finally
        {
            foreach (Process process in processes)
            {
                process.Dispose();
            }
        }
    }

    // リソース解放
    public void Dispose()
    {
        timer.Stop();
        timer.Dispose();
    }
}
