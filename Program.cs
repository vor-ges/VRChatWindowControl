namespace VRChatWindowControl;

static void Main(string[] args)
{
    ApplicationConfiguration.Initialize();

    bool startInTray = Array.Exists(
        args,
        arg => string.Equals(
            arg,
            "--tray",
            StringComparison.OrdinalIgnoreCase
        )
    );

    Application.Run(new Form1(startInTray));
}
