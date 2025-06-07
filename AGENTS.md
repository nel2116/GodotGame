# プロジェクト運用ルール

## コーディング

-   `.editorconfig` の設定に従い、インデントは 4 スペース、改行コードは LF を使用します。
-   ゲームの処理は C# (Godot 4.x) で実装し、GDScript は使用しません。各関数には日本語で簡潔な説明コメントを付けてください。
-   クラス名はパスカルケース、変数名はスネークケース、定数は大文字スネークケースとします。詳細な命名規則は `Docs/10_CoreDocs/DevelopmentGuidelines.md` を参照してください。
-   コードは読みやすさと保守性を重視し、不要な処理や曖昧な表現は避けます。

## 設計

-   新規機能やシステム設計を行う際は、`Docs/10_CoreDocs/14_TechDocs/14.19_MVVMReactiveDesign.md` を参照し、MVVM + リアクティブプログラミングの方針に沿ってください。

## ドキュメント

-   `Docs/` 以下の Markdown を更新する際は `Docs/99_Reference/DocumentManagementRules.md` の指針に従います。
    -   冒頭に `title`、`version`、`status`、`updated`、`tags`、`linked_docs` を含む YAML メタデータを記述します。
    -   `updated` は UTC の `YYYY-MM-DD` 形式で現在の日付を記載し、変更履歴を更新します。

## コミットと PR

-   コミットメッセージは 1 行目に要約を書き、必要に応じて `Closes #<番号>` などで関連 Issue を明記します。
-   詳細なフォーマットや Prefix ルールは `Docs/99_Reference/CommitMessageRules.md` を参照してください。
-   1 コミットでは関連する変更のみをまとめ、無関係な修正を含めないでください。
-   プルリクエストでは変更点、テスト方法、関連 Issue を日本語で説明します。
-   プルリクエストを作成する際は、`Docs/99_Reference/PullRequestProcedure.md` のガイドラインに従ってください。

## テスト

-   テストが存在する場合はコミット前に以下を実行して結果を確認してください。
    ```bash
    godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json
    ```
-   詳細な手順は `Docs/20_UserGuides/TestExecutionGuide.md` を参照してください。
-   テスト通過後、`git status` を確認し作業ツリーがクリーンであることを確かめます。

## 言語

-   コードコメント、ドキュメント、PR など文章はすべて日本語で記述してください。

# AI エージェント向けドキュメント

## テスト関連ドキュメント

1. [テスト実行ガイド](Docs/20_UserGuides/TestExecutionGuide.md)

    - テストの実行手順
    - 環境構築
    - トラブルシューティング

2. [テストガイドライン](Docs/20_UserGuides/TestGuidelines.md)

    - テスト戦略
    - テストの種類と使い分け
    - 推奨アプローチ

3. [AI エージェント向けテスト実行ワークフロー](Docs/99_Reference/AI_Agent_TestWorkflow.md)
    - 自動化されたテスト実行
    - テスト結果の解析
    - エラー処理

## テスト実行の推奨アプローチ

1. **ハイブリッドアプローチ**

    - ビジネスロジック → C#テスト（NUnit）
    - ゲームエンジン機能 → GUT テスト
    - 統合テスト → 必要に応じて両方を使用

2. **テストの優先順位**

    - 重要なビジネスロジック
    - 頻繁に変更されるコード
    - 複雑なロジック
    - エッジケース

3. **自動化の活用**
    - CI/CD パイプラインでのテスト実行
    - テスト結果の自動解析
    - エラー時の自動リカバリー

## 注意事項

-   テスト実行前に必ず環境構築を完了してください
-   テスト結果は必ず確認し、失敗した場合は原因を特定してください
-   テストコードの品質を維持し、定期的なレビューを行ってください
