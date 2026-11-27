namespace VRChatWindowControl;

internal sealed class AppSettings
{
    // ウィンドウの幅
    public int Width { get; set; } = 400;

    // ウィンドウの高さ
    public int Height { get; set; } = 300;

    // ツールの表示言語
    public string Language { get; set; } = AppLanguage.DefaultCode;
}
