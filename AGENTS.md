# プロジェクト運用ルール

## コーディング
- `.editorconfig` の設定に従い、インデントは4スペース、改行コードは LF を使用します。
- ゲームの処理は C# (Godot 4.x) で実装し、GDScript は使用しません。各関数には日本語で簡潔な説明コメントを付けてください。
- クラス名はパスカルケース、変数名はスネークケース、定数は大文字スネークケースとします。詳細な命名規則は `Docs/10_CoreDocs/DevelopmentGuidelines.md` を参照してください。
- コードは読みやすさと保守性を重視し、不要な処理や曖昧な表現は避けます。

## ドキュメント
- `Docs/` 以下の Markdown を更新する際は `Docs/99_Reference/DocumentManagementRules.md` の指針に従います。
    - 冒頭に `title`、`version`、`status`、`updated`、`tags`、`linked_docs` を含む YAML メタデータを記述します。
    - `updated` は UTC の `YYYY-MM-DD` 形式で現在の日付を記載し、変更履歴を更新します。

## コミットと PR
- コミットメッセージは1行目に要約を書き、必要に応じて `Closes #<番号>` などで関連 Issue を明記します。
- 1コミットでは関連する変更のみをまとめ、無関係な修正を含めないでください。
- プルリクエストでは変更点、テスト方法、関連 Issue を日本語で説明します。

## テスト
- テストが存在する場合はコミット前に以下を実行して結果を確認してください。
  ```bash
  godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json
  ```
- テスト通過後、`git status` を確認し作業ツリーがクリーンであることを確かめます。

## 言語
- コードコメント、ドキュメント、PR など文章はすべて日本語で記述してください。
