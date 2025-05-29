FROM ubuntu:22.04

# 必要なパッケージのインストール
RUN apt-get update && apt-get install -y \
    wget \
    unzip \
    git \
    curl \
    gnupg \
    software-properties-common \
    apt-transport-https \
    ca-certificates \
    x11-apps \
    && rm -rf /var/lib/apt/lists/*

# Node.jsのインストール（Cursor IDE用）
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs

# Cursor IDEのインストール
RUN wget https://download.cursor.sh/linux/appImage/x64/cursor-0.1.0.AppImage \
    && chmod +x cursor-0.1.0.AppImage \
    && mv cursor-0.1.0.AppImage /usr/local/bin/cursor

# Godot Engine 4.4のインストール
RUN wget https://github.com/godotengine/godot/releases/download/4.4-stable/Godot_v4.4-stable_linux.x86_64.zip \
    && unzip Godot_v4.4-stable_linux.x86_64.zip \
    && mv Godot_v4.4-stable_linux.x86_64 /usr/local/bin/godot \
    && rm Godot_v4.4-stable_linux.x86_64.zip

# Obsidianのインストール
RUN wget https://github.com/obsidianmd/obsidian-releases/releases/download/v1.5.8/obsidian_1.5.8_amd64.deb \
    && apt-get install -y ./obsidian_1.5.8_amd64.deb \
    && rm obsidian_1.5.8_amd64.deb

# 作業ディレクトリの設定
WORKDIR /workspace

# プロジェクトファイルのコピー
COPY . .

# 環境変数の設定
ENV GODOT_EDITOR_PATH=/usr/local/bin/godot
ENV CURSOR_PATH=/usr/local/bin/cursor
ENV OBSIDIAN_PATH=/usr/bin/obsidian

# 設定ファイルの配置
RUN mkdir -p /root/.config/godot \
    && mkdir -p /root/.config/Cursor \
    && mkdir -p /root/.config/obsidian

# デフォルトコマンド
CMD ["godot", "--editor"] 