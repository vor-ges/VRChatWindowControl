Set shell = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")

scriptPath = fso.BuildPath( _
    fso.GetParentFolderName(WScript.ScriptFullName), _
    "VRChat-VR-Window.ps1")

command = "powershell.exe -NoProfile -STA -ExecutionPolicy Bypass -WindowStyle Hidden -File """ & scriptPath & """"

shell.Run command, 0, False

Set fso = Nothing
Set shell = Nothing