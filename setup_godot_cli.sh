#!/bin/bash
set -e
VERSION="4.4.1"
URL="https://github.com/godotengine/godot/releases/download/${VERSION}-stable/Godot_v${VERSION}-stable_mono_linux_x86_64.zip"
if ! command -v dotnet >/dev/null 2>&1; then
    sudo apt-get update
    sudo apt-get install -y dotnet-sdk-8.0
fi
if command -v godot >/dev/null 2>&1; then
    echo "godot is already installed: $(godot --version)"
else
    TMP_DIR=$(mktemp -d)
    cd "$TMP_DIR"
    curl -L -o godot.zip "$URL"
    unzip -q godot.zip
    sudo mv Godot_v${VERSION}-stable_mono_linux_x86_64/Godot_v${VERSION}-stable_mono_linux.x86_64 /usr/local/bin/godot
    sudo mv Godot_v${VERSION}-stable_mono_linux_x86_64/GodotSharp /usr/local/bin/
    sudo chmod +x /usr/local/bin/godot
    cd -
    rm -rf "$TMP_DIR"
    echo "Godot CLI installed to /usr/local/bin/godot"
fi
PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"
godot --headless --path "$PROJECT_DIR" --import
godot --headless --path "$PROJECT_DIR" --build-solutions --quit
echo "Assets imported for project at $PROJECT_DIR"
