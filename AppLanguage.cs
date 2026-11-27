namespace VRChatWindowControl;

internal sealed record LanguageOption(string Code, string DisplayName);

internal sealed record AppLanguageText(
    string SettingsTitle,
    string WidthLabel,
    string HeightLabel,
    string LanguageLabel,
    string SaveButton,
    string CancelButton,
    string TraySettings,
    string TrayPause,
    string TrayResume,
    string TrayExit,
    string SaveSuccess,
    string SaveErrorPrefix
);

internal static class AppLanguage
{
    public const string DefaultCode = "en";

    public static IReadOnlyList<LanguageOption> Options { get; } =
    [
        new("en", "English"),
        new("ja", "日本語"),
        new("de", "Deutsch"),
        new("ko", "한국어"),
        new("zh-Hans", "简体中文"),
        new("zh-Hant", "繁體中文"),
        new("tr", "Türkçe")
    ];

    public static bool IsSupported(string? code) =>
        Options.Any(option => option.Code == code);

    public static string NormalizeCode(string? code) =>
        IsSupported(code) ? code! : DefaultCode;

    public static AppLanguageText GetText(string? code) =>
        NormalizeCode(code) switch
        {
            "ja" => new(
                "VRChat Window Control - 設定",
                "幅 (px)",
                "高さ (px)",
                "言語",
                "保存",
                "キャンセル",
                "設定",
                "一時停止",
                "再開",
                "終了",
                "設定を保存しました。",
                "設定を保存できませんでした:\n"
            ),
            "de" => new(
                "VRChat Window Control - Einstellungen",
                "Breite (px)",
                "Höhe (px)",
                "Sprache",
                "Speichern",
                "Abbrechen",
                "Einstellungen",
                "Pausieren",
                "Fortsetzen",
                "Beenden",
                "Einstellungen gespeichert.",
                "Einstellungen konnten nicht gespeichert werden:\n"
            ),
            "ko" => new(
                "VRChat Window Control - 설정",
                "너비 (px)",
                "높이 (px)",
                "언어",
                "저장",
                "취소",
                "설정",
                "일시 정지",
                "다시 시작",
                "종료",
                "설정을 저장했습니다.",
                "설정을 저장할 수 없습니다:\n"
            ),
            "zh-Hans" => new(
                "VRChat Window Control - 设置",
                "宽度（像素）",
                "高度（像素）",
                "语言",
                "保存",
                "取消",
                "设置",
                "暂停",
                "继续",
                "退出",
                "设置已保存。",
                "无法保存设置：\n"
            ),
            "zh-Hant" => new(
                "VRChat Window Control - 設定",
                "寬度（像素）",
                "高度（像素）",
                "語言",
                "儲存",
                "取消",
                "設定",
                "暫停",
                "繼續",
                "結束",
                "設定已儲存。",
                "無法儲存設定：\n"
            ),
            "tr" => new(
                "VRChat Window Control - Ayarlar",
                "Genişlik (px)",
                "Yükseklik (px)",
                "Dil",
                "Kaydet",
                "İptal",
                "Ayarlar",
                "Duraklat",
                "Sürdür",
                "Çıkış",
                "Ayarlar kaydedildi.",
                "Ayarlar kaydedilemedi:\n"
            ),
            _ => new(
                "VRChat Window Control - Settings",
                "Width (px)",
                "Height (px)",
                "Language",
                "Save",
                "Cancel",
                "Settings",
                "Pause",
                "Resume",
                "Exit",
                "Settings saved successfully.",
                "Could not save settings:\n"
            )
        };
}
