Set shell = CreateObject("WScript.Shell")

scriptPath = shell.ExpandEnvironmentStrings( _
    "%USERPROFILE%\Documents\GitHub\VRChatWindowControl\VRChat-VR-Window.ps1")

command = "powershell.exe -NoProfile -STA -ExecutionPolicy Bypass -WindowStyle Hidden -File """ & scriptPath & """"

shell.Run command, 0, False

Set shell = Nothing