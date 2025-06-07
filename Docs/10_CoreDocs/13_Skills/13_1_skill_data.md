---
title: スキルデータ仕様書
version: 0.1.0
status: draft
updated: 2025-06-01
tags:
    - Skills
    - Data
    - Specification
    - Core
linked_docs:
    - "[[13_Skills/00_index|スキル関連]]"
---

```dataview
TABLE WITHOUT ID skill_id,branch,tier,skill_name,skill_type,active_slot,description,base_value,scaling_stat,scaling_ratio,cooldown_sec,cost_type,cost,hitbox,range_m,duration_sec,unlock_condition,tags,vfx_id,sfx_id
FROM csv("SkilData.csv")
```
