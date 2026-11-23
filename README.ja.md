# VRChat Window Control



SteamVRの起動を検知し、VRChatのPCウィンドウを自動的にリサイズ・最小化するWindows向けユーティリティです。



VRChatをVRモードで利用するとき、PC側のウィンドウを小さくして最小化することで、VR内での動作を軽量化します。



## Features



- SteamVRの起動から120秒間以内に起動されたVRChatのウィンドウを指定サイズに変更して最小化します。

- システムトレイから設定や監視状態を操作できます。

- ウィンドウの幅・高さの変更や、自動監視の一時停止・再開ができます。



## Requirements



- Windows

- Windows PowerShell 5.1

- SteamVR

- VRChat



## Installation



### Current Version



1. このリポジトリをダウンロードまたはクローンします。

2. `Start-VRChatWindowHelper.vbs` を実行します。

3. システムトレイにアイコンが表示されれば起動完了です。



### Configuration



システムトレイのアイコンを右クリックし、`Settings...` を選択します。



| Setting | Description  |

| ------- | ------------ |

| Width   | ウィンドウの幅（px）  |

| Height  | ウィンドウの高さ（px） |



設定は `settings.json` に保存されます。



## Limitations



- SteamVRが起動している状態でVRChatを起動した場合も、処理対象になることがあります。

- SteamVR起動後120秒を過ぎてからVRChatを起動した場合、自動処理されません。

- 現在はWindows専用です。



## Contributing



Issues and pull requests are welcome.



Bug reports, feature requests, and improvements are appreciated.



## Roadmap



- [ ] Windows installer

- [ ] Start Menu shortcut and startup registration

- [ ] Uninstaller

- [ ] GitHub Releases

- [ ] Automated builds with GitHub Actions

- [ ] Migration to C# / .NET

- [ ] Windows ARM64 support

- [ ] Cross-platform support investigation



## License



This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.



