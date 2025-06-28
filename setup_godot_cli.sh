#!/bin/bash

# Godot CLI セットアップスクリプト
# Docker環境でのGodot開発環境を構築します

echo "🚀 Godot Game Development Environment Setup"
echo "=========================================="

# Dockerがインストールされているかチェック
if ! command -v docker &> /dev/null; then
    echo "❌ Dockerがインストールされていません。"
    echo "   https://docs.docker.com/get-docker/ からDockerをインストールしてください。"
    exit 1
fi

# Docker Composeがインストールされているかチェック
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Composeがインストールされていません。"
    echo "   https://docs.docker.com/compose/install/ からDocker Composeをインストールしてください。"
    exit 1
fi

echo "✅ Docker環境の確認完了"

# 既存のコンテナを停止・削除
echo "🧹 既存のコンテナをクリーンアップ..."
docker-compose down -v 2>/dev/null || true

# イメージをビルド
echo "🔨 Dockerイメージをビルド中..."
docker-compose build --no-cache

if [ $? -eq 0 ]; then
    echo "✅ ビルド完了"
else
    echo "❌ ビルドに失敗しました"
    exit 1
fi

# コンテナを起動
echo "🚀 開発環境を起動中..."
docker-compose up -d

if [ $? -eq 0 ]; then
    echo "✅ 開発環境の起動完了"
    echo ""
    echo "📋 利用可能なコマンド:"
    echo "  docker-compose up -d          # バックグラウンドで起動"
    echo "  docker-compose down           # 停止"
    echo "  docker-compose logs -f        # ログを表示"
    echo "  docker exec -it godot-game-dev bash  # コンテナ内でシェル実行"
    echo ""
    echo "🌐 アクセス先:"
    echo "  Godot Editor: http://localhost:6006"
    echo "  Cursor IDE: http://localhost:3000"
    echo ""
    echo "🎮 開発を開始するには:"
    echo "  docker exec -it godot-game-dev godot --editor"
else
    echo "❌ 起動に失敗しました"
    exit 1
fi
