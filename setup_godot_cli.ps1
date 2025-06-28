# Godot CLI セットアップスクリプト (PowerShell版)
# Docker環境でのGodot開発環境を構築します

Write-Host "🚀 Godot Game Development Environment Setup" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green

# Dockerがインストールされているかチェック
try {
    $dockerVersion = docker --version
    Write-Host "✅ Docker: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Dockerがインストールされていません。" -ForegroundColor Red
    Write-Host "   https://docs.docker.com/get-docker/ からDockerをインストールしてください。" -ForegroundColor Yellow
    exit 1
}

# Docker Composeがインストールされているかチェック
try {
    $composeVersion = docker-compose --version
    Write-Host "✅ Docker Compose: $composeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker Composeがインストールされていません。" -ForegroundColor Red
    Write-Host "   https://docs.docker.com/compose/install/ からDocker Composeをインストールしてください。" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Docker環境の確認完了" -ForegroundColor Green

# 既存のコンテナを停止・削除
Write-Host "🧹 既存のコンテナをクリーンアップ..." -ForegroundColor Yellow
docker-compose down -v 2>$null

# イメージをビルド
Write-Host "🔨 Dockerイメージをビルド中..." -ForegroundColor Yellow
docker-compose build --no-cache

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ ビルド完了" -ForegroundColor Green
} else {
    Write-Host "❌ ビルドに失敗しました" -ForegroundColor Red
    exit 1
}

# コンテナを起動
Write-Host "🚀 開発環境を起動中..." -ForegroundColor Yellow
docker-compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ 開発環境の起動完了" -ForegroundColor Green
    Write-Host ""
    Write-Host "📋 利用可能なコマンド:" -ForegroundColor Cyan
    Write-Host "  docker-compose up -d          # バックグラウンドで起動" -ForegroundColor White
    Write-Host "  docker-compose down           # 停止" -ForegroundColor White
    Write-Host "  docker-compose logs -f        # ログを表示" -ForegroundColor White
    Write-Host "  docker exec -it godot-game-dev bash  # コンテナ内でシェル実行" -ForegroundColor White
    Write-Host ""
    Write-Host "🌐 アクセス先:" -ForegroundColor Cyan
    Write-Host "  Godot Editor: http://localhost:6006" -ForegroundColor White
    Write-Host "  Cursor IDE: http://localhost:3000" -ForegroundColor White
    Write-Host ""
    Write-Host "🎮 開発を開始するには:" -ForegroundColor Cyan
    Write-Host "  docker exec -it godot-game-dev godot --editor" -ForegroundColor White
} else {
    Write-Host "❌ 起動に失敗しました" -ForegroundColor Red
    exit 1
}
