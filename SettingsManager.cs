using System;
using System.IO;
using System.Text.Json;

namespace VRChatWindowControl;

internal static class SettingsManager
{
    private static readonly string ConfigDirectory =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData
            ),
            "VRChatWindowControl"
        );

    private static readonly string ConfigPath =
        Path.Combine(ConfigDirectory, "settings.json");

    // 旧バージョンの設定ファイル
    private static readonly string LegacyConfigPath =
        Path.Combine(
            AppContext.BaseDirectory,
            "settings.json"
        );

    // 設定を読み込む
    public static AppSettings Load()
    {
        try
        {
            Directory.CreateDirectory(ConfigDirectory);

            // 新しい保存先に設定がなければ旧設定を移行
            if (!File.Exists(ConfigPath) &&
                File.Exists(LegacyConfigPath))
            {
                File.Copy(
                    LegacyConfigPath,
                    ConfigPath
                );
            }

            if (!File.Exists(ConfigPath))
            {
                return new AppSettings();
            }

            string json = File.ReadAllText(ConfigPath);

            AppSettings? settings =
                JsonSerializer.Deserialize<AppSettings>(json);

            if (settings == null)
            {
                return new AppSettings();
            }

            // 設定値の範囲を検証
            if (settings.Width < 100 || settings.Width > 7680)
            {
                settings.Width = 400;
            }

            if (settings.Height < 100 || settings.Height > 4320)
            {
                settings.Height = 300;
            }

            return settings;
        }
        catch
        {
            return new AppSettings();
        }
    }

    // 設定を保存する
    public static void Save(AppSettings settings)
    {
        Directory.CreateDirectory(ConfigDirectory);

        string json = JsonSerializer.Serialize(
            settings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            }
        );

        File.WriteAllText(ConfigPath, json);
    }
}
