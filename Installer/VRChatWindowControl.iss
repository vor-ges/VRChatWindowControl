#define MyAppName "VRChat Window Control"
#define MyAppVersion "1.1.0"
#define MyAppPublisher "vor-ges"

[Setup]
AppId={{B6D5C9E1-7A43-4F2D-9E86-2D2D2D4F1A71}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
UsePreviousPrivileges=yes
OutputDir=output
OutputBaseFilename=VRChatWindowControl-Setup-{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\VRChatWindowControl.exe
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[CustomMessages]
english.AdditionalTasks=Additional tasks:
english.StartupTask=Start VRChat Window Control when Windows starts (recommended)
english.DesktopTask=Create a desktop shortcut
english.StartMenuTask=Create a Start Menu shortcut
japanese.AdditionalTasks=追加タスク:
japanese.StartupTask=Windowsの起動時にVRChat Window Controlを起動する（推奨）
japanese.DesktopTask=デスクトップにショートカットを作成する
japanese.StartMenuTask=スタートメニューにショートカットを作成する

[Tasks]
Name: "startup"; Description: "{cm:StartupTask}"; GroupDescription: "{cm:AdditionalTasks}"
Name: "desktopicon"; Description: "{cm:DesktopTask}"; GroupDescription: "{cm:AdditionalTasks}"; Flags: unchecked
Name: "startmenuicon"; Description: "{cm:StartMenuTask}"; GroupDescription: "{cm:AdditionalTasks}"; Flags: unchecked

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\VRChatWindowControl.exe"; Tasks: startmenuicon
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\VRChatWindowControl.exe"; Tasks: desktopicon
Name: "{autostartup}\{#MyAppName}"; Filename: "{app}\VRChatWindowControl.exe"; Tasks: startup

[UninstallDelete]
; Keep per-user settings in %APPDATA% so uninstalling does not erase preferences.
