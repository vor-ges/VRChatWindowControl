# VRChat Window Control

[English](README.md) | [日本語](README.ja.md)

A Windows utility that automatically resizes and minimizes the VRChat window when SteamVR is detected.

It helps improve performance while using VR mode by making VRChat window smaller and minimizing it.

## Features

- **Automatic Window Management** — Detects SteamVR and resizes and minimizes the VRChat window when it is found.
- **System Tray Operation** — Manage settings and monitoring from the system tray.
- **Customizable Settings** — Adjust the window width and height, and pause or resume automatic monitoring.

## Requirements

- Windows
- Windows PowerShell 5.1
- SteamVR
- VRChat

## Installation

### Current Version

1. Download or clone this repository.
2. Extract the files if you downloaded the ZIP archive.
3. Run `Start-VRChatWindowHelper.vbs`.
4. The application is running when its icon appears in the system tray.

No installer is currently provided.

### Configuration

Right-click the system tray icon and select `Settings...`.

| Setting | Description |
| ------- | ----------- |
| Width | Window width in pixels |
| Height | Window height in pixels |

Settings are saved to `settings.json`.

## Limitations

- The application checks for the VRChat window for up to 120 seconds after detecting SteamVR as running.
- VRChat may also be processed when launched while SteamVR is already running.
- VRChat will not be processed if its window is not detected within the detection period.
- Windows only.

## Contributing

Issues and pull requests are welcome.

Bug reports, feature requests, and improvements are appreciated.

## Planned features

- [ ] Automated builds with GitHub Actions
- [ ] Migration to C# / .NET
- [ ] Windows ARM64 support
- [ ] Cross-platform support investigation

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
