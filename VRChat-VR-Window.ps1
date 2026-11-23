
# VRChat Window Control
# Windows PowerShell 5.1

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

if (-not ("VRChatWindowNative" -as [type])) {
    Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;

public static class VRChatWindowNative
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(
        IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags
    );

    [DllImport("user32.dll")]
    public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
}
"@
}

# Configuration
$script:ConfigDirectory = Join-Path $env:APPDATA "VRChatWindowControl"
$script:ConfigPath = Join-Path $script:ConfigDirectory "settings.json"
$script:LegacyConfigPath = Join-Path $PSScriptRoot "settings.json"
$script:WindowWidth = 400
$script:WindowHeight = 300

# Create the per-user configuration directory
try {
    if (-not (Test-Path $script:ConfigDirectory)) {
        New-Item -ItemType Directory -Path $script:ConfigDirectory -Force |
            Out-Null
    }
}
catch {
    [System.Windows.Forms.MessageBox]::Show(
        "Could not create the settings folder: $($_.Exception.Message)",
        "VRChat Window Control"
    ) | Out-Null
    throw
}

# Migrate settings from the previous application folder, if necessary
if (
    -not (Test-Path $script:ConfigPath) -and
    (Test-Path $script:LegacyConfigPath)
) {
    try {
        Copy-Item -Path $script:LegacyConfigPath -Destination $script:ConfigPath
    }
    catch {
        [System.Windows.Forms.MessageBox]::Show(
            "Could not migrate the existing settings. Default values will be used. Details: $($_.Exception.Message)",
            "VRChat Window Control"
        ) | Out-Null
    }
}

# Load saved settings
if (Test-Path $script:ConfigPath) {
    try {
        $config = Get-Content $script:ConfigPath -Raw |
            ConvertFrom-Json

        if ($config.Width -ge 100 -and $config.Width -le 7680) {
            $script:WindowWidth = [int]$config.Width
        }

        if ($config.Height -ge 100 -and $config.Height -le 4320) {
            $script:WindowHeight = [int]$config.Height
        }
    }
    catch {
        [System.Windows.Forms.MessageBox]::Show(
            "Could not load settings. Default values will be used.",
            "VRChat Window Control"
        ) | Out-Null
    }
}

# Monitoring settings
$script:DetectionTimeoutSeconds = 120
$script:PollMilliseconds = 1000
$script:Paused = $false
$script:VrServerWasRunning = $false
$script:DetectionDeadline = [DateTime]::MinValue
$script:WindowProcessed = $false

# Settings dialog
function Show-SettingsDialog {
    $form = New-Object System.Windows.Forms.Form
    $form.Text = "VRChat Window Control - Settings"
    $form.Size = New-Object System.Drawing.Size(340, 220)
    $form.StartPosition = "CenterScreen"
    $form.FormBorderStyle = "FixedDialog"
    $form.MaximizeBox = $false
    $form.MinimizeBox = $false
    $form.ShowInTaskbar = $false

    $widthLabel = New-Object System.Windows.Forms.Label
    $widthLabel.Text = "Width (px)"
    $widthLabel.Location = New-Object System.Drawing.Point(20, 25)
    $widthLabel.Size = New-Object System.Drawing.Size(110, 25)

    $widthInput = New-Object System.Windows.Forms.NumericUpDown
    $widthInput.Location = New-Object System.Drawing.Point(145, 22)
    $widthInput.Size = New-Object System.Drawing.Size(150, 25)
    $widthInput.Minimum = 100
    $widthInput.Maximum = 7680
    $widthInput.Value = $script:WindowWidth

    $heightLabel = New-Object System.Windows.Forms.Label
    $heightLabel.Text = "Height (px)"
    $heightLabel.Location = New-Object System.Drawing.Point(20, 65)
    $heightLabel.Size = New-Object System.Drawing.Size(110, 25)

    $heightInput = New-Object System.Windows.Forms.NumericUpDown
    $heightInput.Location = New-Object System.Drawing.Point(145, 62)
    $heightInput.Size = New-Object System.Drawing.Size(150, 25)
    $heightInput.Minimum = 100
    $heightInput.Maximum = 4320
    $heightInput.Value = $script:WindowHeight

    $saveButton = New-Object System.Windows.Forms.Button
    $saveButton.Text = "Save"
    $saveButton.Location = New-Object System.Drawing.Point(125, 120)
    $saveButton.Size = New-Object System.Drawing.Size(80, 30)

    $cancelButton = New-Object System.Windows.Forms.Button
    $cancelButton.Text = "Cancel"
    $cancelButton.Location = New-Object System.Drawing.Point(215, 120)
    $cancelButton.Size = New-Object System.Drawing.Size(80, 30)

    $saveButton.Add_Click({
        $newWidth = [int]$widthInput.Value
        $newHeight = [int]$heightInput.Value

        try {
            $newConfig = @{
                Width = $newWidth
                Height = $newHeight
            }

            $newConfig | ConvertTo-Json |
                Set-Content -Path $script:ConfigPath -Encoding UTF8

            $script:WindowWidth = $newWidth
            $script:WindowHeight = $newHeight

            $form.DialogResult = [System.Windows.Forms.DialogResult]::OK
            $form.Close()
        }
        catch {
            [System.Windows.Forms.MessageBox]::Show(
                "Could not save settings: $($_.Exception.Message)",
                "VRChat Window Control"
            ) | Out-Null
        }
    })

    $cancelButton.Add_Click({
        $form.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
        $form.Close()
    })

    $form.Controls.AddRange(@(
        $widthLabel,
        $widthInput,
        $heightLabel,
        $heightInput,
        $saveButton,
        $cancelButton
    ))

    $form.AcceptButton = $saveButton
    $form.CancelButton = $cancelButton

    try {
        $form.ShowDialog() | Out-Null
    }
    finally {
        $form.Dispose()
    }
}

# Tray icon
$notifyIcon = New-Object System.Windows.Forms.NotifyIcon
$notifyIcon.Icon = [System.Drawing.SystemIcons]::Application
$notifyIcon.Text = "VRChat Window Control"
$notifyIcon.Visible = $true

$menu = New-Object System.Windows.Forms.ContextMenuStrip

$settingsItem = $menu.Items.Add("Settings...")
$menu.Items.Add("-") | Out-Null

$pauseItem = $menu.Items.Add("Pause monitoring")
$resumeItem = $menu.Items.Add("Resume monitoring")
$menu.Items.Add("-") | Out-Null

$exitItem = $menu.Items.Add("Exit")

$settingsItem.Add_Click({
    Show-SettingsDialog
})

$pauseItem.Add_Click({
    $script:Paused = $true
    $pauseItem.Enabled = $false
    $resumeItem.Enabled = $true
})

$resumeItem.Add_Click({
    $script:Paused = $false
    $pauseItem.Enabled = $true
    $resumeItem.Enabled = $false
})

$exitItem.Add_Click({
    $notifyIcon.Visible = $false
    $notifyIcon.Dispose()
    $timer.Stop()
    $timer.Dispose()
    [System.Windows.Forms.Application]::Exit()
})

$resumeItem.Enabled = $false
$notifyIcon.ContextMenuStrip = $menu

# Timer
$timer = New-Object System.Windows.Forms.Timer
$timer.Interval = $script:PollMilliseconds

$timer.Add_Tick({

    if ($script:Paused) {
        return
    }

    $vrServer = Get-Process -Name "vrserver" -ErrorAction SilentlyContinue
    $vrServerRunning = ($null -ne $vrServer)

    if ($vrServerRunning -and -not $script:VrServerWasRunning) {
        $script:DetectionDeadline = [DateTime]::Now.AddSeconds(
            $script:DetectionTimeoutSeconds
        )
        $script:WindowProcessed = $false
    }

    if (-not $vrServerRunning) {
        $script:WindowProcessed = $false
    }

    $script:VrServerWasRunning = $vrServerRunning

    if (-not $vrServerRunning -or $script:WindowProcessed) {
        return
    }

    if ([DateTime]::Now -gt $script:DetectionDeadline) {
        return
    }

    $vrChat = Get-Process -Name "VRChat" -ErrorAction SilentlyContinue |
        Where-Object { $_.MainWindowHandle -ne 0 } |
        Select-Object -First 1

    if ($null -eq $vrChat) {
        return
    }

    $hwnd = $vrChat.MainWindowHandle

    [VRChatWindowNative]::SetWindowPos(
        $hwnd,
        [IntPtr]::Zero,
        0,
        0,
        $script:WindowWidth,
        $script:WindowHeight,
        0x0016
    ) | Out-Null

    [VRChatWindowNative]::ShowWindowAsync($hwnd, 6) | Out-Null

    $script:WindowProcessed = $true
})

# Start application
$timer.Start()

try {
    [System.Windows.Forms.Application]::Run()
}
finally {
    $timer.Stop()
    $timer.Dispose()
    $notifyIcon.Visible = $false
    $notifyIcon.Dispose()
}
