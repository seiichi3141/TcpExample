# リポジトリガイドライン

## プロジェクト構成とモジュール整理
- ソリューションのエントリは `TcpExample.sln`。WPF のソースは `TcpExample/` に配置。
- アプリの起動とリソース: `TcpExample/App.xaml` および `App.xaml.cs`。
- メインウィンドウの UI とコードビハインド: `TcpExample/MainWindow.xaml` および `MainWindow.xaml.cs`。UI の配線はここに留め、可能な箇所はヘルパークラスへロジックを移す。
- 生成されるリソース/設定: `TcpExample/Properties/` (`Resources.resx`, `Settings.settings`, `AssemblyInfo.cs`)。
- ビルド成果物は `TcpExample/bin/Debug` または `TcpExample/bin/Release`。一時生成物は `TcpExample/obj/` に配置。

## ビルド・テスト・開発コマンド
- Debug ビルド: `msbuild TcpExample/TcpExample.csproj /p:Configuration=Debug`
- Release ビルド: `msbuild TcpExample/TcpExample.csproj /p:Configuration=Release`
- クリーン: `msbuild TcpExample/TcpExample.csproj /t:Clean`
- ローカル実行: Debug ビルド後に `TcpExample/bin/Debug/TcpExample.exe` を起動、または Visual Studio でソリューションを開いてデバッグ開始。

## コーディングスタイルと命名規約
- ターゲット フレームワークは .NET Framework 4.8.1。コードは Windows 前提で WPF に配慮。
- インデントは 4 スペース。型/メソッドの波括弧は改行後に配置（Visual Studio 既定）。
- 命名: クラス・メソッド・プロパティ・イベント・XAML コントロールの `x:Name` は PascalCase（例: `ConnectButton`）。ローカル変数/引数は camelCase。プライベートフィールドは `_camelCase`。
- コードビハインドは最小限にし、UI とネットワーク/データロジックを分離してテストしやすく保つ。
- 設定を追加する場合はハードコーディングせず `App.config` を更新（接続情報、ポート、タイムアウト等）。

## テストガイドライン
- 自動テストは未整備。追加する場合は MSTest または NUnit を使った兄弟プロジェクト `TcpExample.Tests` を推奨。ファイル名は `MainWindowTests.cs` とし、機能ごとにフィクスチャを分ける。
- 手動確認: ビルドしてアプリを実行し、メインウィンドウがバインドや起動例外なく開くことを確認。追加した TCP のやり取りは想定エンドポイントとエラー処理に沿うか検証。

## コミットとプルリクエストの指針
- コミットは挙動変更を示す簡潔な命令形サブジェクト（例: `Add basic TCP connect UI`）。関連変更をまとめる。
- プルリクエストには概要、テストノートや再現手順、UI 更新時のスクリーンショット/GIF を添付。関連作業項目/Issue をリンクし、リスク領域（ネットワーク変更、スレッド、UI レスポンス）を明記。

## セキュリティと設定のヒント
- シークレット、エンドポイント、証明書はコミットしない。`App.config` のアプリ設定を利用し、デフォルトを記録する。
- ネットワーク変更時は入力検証を行い、ソケットエラーを丁寧に処理して UI のハングを防ぐ。長時間処理は UI スレッドから外す。
