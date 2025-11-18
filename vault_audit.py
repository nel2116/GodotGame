import os
import re
from pathlib import Path
from datetime import datetime
import csv

# 🔁 GodotGameプロジェクトのDocsフォルダを対象に設定
VAULT_ROOT = Path("./Docs")

# 結果出力先
OUTPUT_DIR = Path("./vault_analysis")
OUTPUT_DIR.mkdir(exist_ok=True)

tag_set = set()
link_list = []
file_summary = []

for path in VAULT_ROOT.rglob("*.md"):
    rel_path = path.relative_to(VAULT_ROOT)
    with open(path, encoding="utf-8", errors="ignore") as f:
        content = f.read()
        # タグ抽出 (#tag)
        tag_set.update(re.findall(r'#([A-Za-z0-9_\-]+)', content))
        # 内部リンク抽出 ([[link]])
        link_list += re.findall(r'\[\[([^\]]+)\]\]', content)
    file_summary.append({
        "Path": str(rel_path),
        "Size(Bytes)": os.path.getsize(path),
        "LastModified": datetime.fromtimestamp(path.stat().st_mtime).isoformat()
    })

# 1. ファイル一覧 CSV 出力
csv_path = OUTPUT_DIR / "godot_game_docs_structure.csv"
with open(csv_path, "w", encoding="utf-8", newline="") as csvfile:
    writer = csv.DictWriter(csvfile, fieldnames=["Path", "Size(Bytes)", "LastModified"])
    writer.writeheader()
    writer.writerows(file_summary)

# 2. タグ一覧
with open(OUTPUT_DIR / "godot_game_tags.txt", "w", encoding="utf-8") as f:
    f.write("# GodotGame プロジェクト タグ一覧\n\n")
    for tag in sorted(tag_set):
        f.write(f"#{tag}\n")

# 3. リンク一覧
with open(OUTPUT_DIR / "godot_game_links.txt", "w", encoding="utf-8") as f:
    f.write("# GodotGame プロジェクト 内部リンク一覧\n\n")
    for link in sorted(set(link_list)):
        f.write(f"[[{link}]]\n")

print(f"✅ GodotGame プロジェクト ドキュメント解析完了: {OUTPUT_DIR.resolve()}")
print(f"📊 解析結果:")
print(f"   - ファイル数: {len(file_summary)}")
print(f"   - タグ数: {len(tag_set)}")
print(f"   - 内部リンク数: {len(set(link_list))}")
