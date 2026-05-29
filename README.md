# CleArchKit

シンプルな .NET 9 の Clean Architecture 開発テンプレートです。Blazor Server と ASP.NET Core API を同じユースケース層で共有し、EF Core + PostgreSQL によるマイグレーション運用を想定したつくりになっていますが、フロントをReactに変えたりDBをMySQLに変えたりすることも自由です。

## 目的
- このリポジトリはクリーンアーキテクチャの学習・プロトタイピングの補助として使えることを目的としています。
レイヤードな設計（Domain / Application / Infrastructure）と運用フロー（マイグレーション管理、devcontainer）を実例付きで提供します。


## 技術スタック
- 言語: C# (.NET 9)
- Web: ASP.NET Core Web API, Blazor Server
- ORM/DB: EF Core 9 + Npgsql (PostgreSQL 16)
- テスト: xUnit, Moq, EF InMemory


## Dockerを使う場合
- 開発用の Compose 定義は `compose.yml`。補助サービスは `compose.override.yml` で追加します。
ここでは参考としてAIコードジェネレーターを補助サービスとしてコンテナに定義しています。

- 起動（通常）
```bash
docker compose -f compose.yml up -d
```

- 起動（override 併用）
```bash
docker compose -f compose.yml -f compose.override.yml up -d
```

- コンテナ内で操作する例
```bash
docker compose -f compose.yml exec app bash
dotnet restore && dotnet build CleArchKit.sln
dotnet run --project CleArchKit.Api
```

- VS Code Devcontainer: `.devcontainer/devcontainer.json` を使い、VS Code で「Reopen in Container」を実行します。
拡張機能は自身の開発環境に合わせて調整してください。

## プロジェクトの起動方法
- API 起動
```bash
dotnet run --project CleArchKit.Api
```

- Blazor 起動
```bash
dotnet run --project CleArchKit.Presentation.Web
```

テスト
```bash
dotnet test CleArchKit.Tests/CleArchKit.Tests.csproj
```

## プロジェクトについての説明
- レイヤー:
  - `CleArchKit.Domain` — エンティティ、値オブジェクト、リポジトリ契約
  - `CleArchKit.Application` — DTO、ユースケース、サービス
  - `CleArchKit.Infrastructure` — DBコンテキスト、リポジトリ、Unit Of Work
  - `CleArchKit.Api` / `CleArchKit.Presentation.Web` — REST / Web 表示層
- 意図: ドメインをインフラから分離し、API と UI でユースケースを共有します。

サンプルとしてUserドメインを定義しています。
このドメインが不要な場合は削除しても問題ありません。また列を追加したりすることも問題ありません。
もちろん変更したことによる対応（ビルドが通らなくなるなど）は各自で対応してください。

システム内で使用している日時はUtcNowであることは気をつけてください。

## マイグレーションについての説明
- 生成/適用は専用プロジェクト `CleArchKit.Migrations` の使用を想定します。
通常のdotnetコマンドのほかに、scripts 配下にあるスクリプトから簡易的に使用することが可能です。


生成

```bash
./scripts/container/add-migration.sh <作成するマイグレーション名>
```

適用
```bash
./scripts/container/migrate.sh
```

ホスト環境の場合は下記コマンドからでも適用が可能です。
```bash
./scripts/host/migrate.ps1
```


- 補足: `CleArchKit.MigrationChecker` が未適用マイグレーションを検出すると Blazor の起動を中断します。
マイグレーション適用のチェックが不要の場合は下記コードは削除してもかまいません。
```C#
var migrationsProjectRelativePath = "../CleArchKit.MigrationChecker";
var migrationsProjectFullPath = Path.GetFullPath(migrationsProjectRelativePath);

var migrationChecker = Process.Start(new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = $"run --project \"{migrationsProjectFullPath}\"",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
});
migrationChecker!.WaitForExit();
if (migrationChecker.ExitCode != 0)
{
    Console.Error.WriteLine("未適用のマイグレーションがあります。アプリケーションを終了します。");
    Environment.Exit(1);
}
```

## dev.ps1についての説明
- `dev.ps1` は Windows (PowerShell) 向けの開発ショートカットで、Docker や関連サービスの使用を簡略化します。
使用できるコマンドは下記です。

|コマンド|内容|
|----------|------|
|dev.ps1 up|docker compose up -d|
|dev.ps1 down|docker compose down|
|dev.ps1 ps|docker compose ps|
|dev.ps1 logs|docker compose logs -f|
|dev.ps1 open|docker compose exec -it [container service] bash|
|dev.ps1 qodo|qodo command|
|dev.ps1 help|Show Usage|

qodo コマンド
https://docs.qodo.ai/qodo-documentation/qodo-command/getting-started/list-of-cli-commands-and-flags

例）
dev.ps1 qodo status
dev.ps1 qodo chat


## ライセンス
- MIT
