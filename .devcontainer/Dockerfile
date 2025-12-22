# .devcontainer/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0

# 必要であれば追加ツール
RUN apt-get update && apt-get install -y \
    bash \
    unzip \
    curl \
    postgresql-client \
    && rm -rf /var/lib/apt/lists/*

# 作業ディレクトリ
WORKDIR /workspace

# （任意）タイムゾーン
ENV TZ=Asia/Tokyo