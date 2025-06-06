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
    x11-apps && \
    rm -rf /var/lib/apt/lists/*

# Node.js 18 のインストール（Cursor IDE用）
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

# .NET 8 SDK のインストール
RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-8.0 && \
    rm -rf /var/lib/apt/lists/*

# Cursor IDE のインストール
RUN wget https://download.cursor.sh/linux/appImage/x64/cursor-0.1.0.AppImage && \
    chmod +x cursor-0.1.0.AppImage && \
    mv cursor-0.1.0.AppImage /usr/local/bin/cursor

# Godot Engine 4.4.1 のインストール
RUN wget https://github.com/godotengine/godot/releases/download/4.4.1-stable/Godot_v4.4.1-stable_linux.x86_64.zip && \
    unzip Godot_v4.4.1-stable_linux.x86_64.zip && \
    mv Godot_v4.4.1-stable_linux.x86_64 /usr/local/bin/godot && \
    rm Godot_v4.4.1-stable_linux.x86_64.zip

# Obsidian のインストール
RUN wget https://github.com/obsidianmd/obsidian-releases/releases/download/v1.5.8/obsidian_1.5.8_amd64.deb && \
    apt-get install -y ./obsidian_1.5.8_amd64.deb && \
    rm obsidian_1.5.8_amd64.deb

WORKDIR /workspace

COPY . .

ENV GODOT_EDITOR_PATH=/usr/local/bin/godot
ENV CURSOR_PATH=/usr/local/bin/cursor
ENV OBSIDIAN_PATH=/usr/bin/obsidian

RUN mkdir -p /root/.config/godot && \
    mkdir -p /root/.config/Cursor && \
    mkdir -p /root/.config/obsidian

CMD ["godot", "--editor"]
