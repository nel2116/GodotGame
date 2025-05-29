# 開発環境のセットアップ

## 必要条件

-   Docker
-   Docker Compose
-   X11 forwarding 対応のディスプレイサーバー

## セットアップ手順

1. リポジトリをクローン

```bash
git clone [リポジトリのURL]
cd newgameproject
```

2. 開発環境の起動

```bash
docker-compose up
```

## 利用可能なツール

### Godot Editor

-   ポート: 6006
-   設定ファイル: ~/.config/godot
-   起動方法: コンテナ内で `godot --editor` を実行

### Cursor IDE

-   ポート: 3000
-   設定ファイル: ~/.config/Cursor
-   起動方法: コンテナ内で `cursor` を実行

### Obsidian

-   設定ファイル: ~/.config/obsidian
-   起動方法: コンテナ内で `obsidian` を実行

## 注意事項

-   初回起動時はイメージのビルドに時間がかかります
-   プロジェクトファイルは自動的にコンテナ内にマウントされます
-   設定ファイルはホストマシンと同期されます
-   X11 forwarding が必要なため、Linux 環境での実行を推奨します
-   Windows 環境で実行する場合は、WSL2 の設定が必要です

## トラブルシューティング

### X11 forwarding の問題

```bash
xhost +local:docker
```

### 権限の問題

```bash
sudo chown -R $USER:$USER ~/.config/godot ~/.config/Cursor ~/.config/obsidian
```
