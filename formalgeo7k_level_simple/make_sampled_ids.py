# -*- coding: utf-8 -*-
"""从 level_index.json 抽取分层抽样题目 ID 清单（sampled_ids_seed<SEED>.json）。

规则（与正式实验口径一致）：
- L1-L5：random.Random(SEED).sample(该级全部题, 每级 PER_LEVEL 题)，无放回；
- L6：不用随机抽样，直接取 level_index.json 中 L6 的全部题号；
- 各层 ID 升序输出，结果为 40*5 + 293 = 493 题。

用法：
    python make_sampled_ids.py                # 生成 sampled_ids_seed42.json
    python make_sampled_ids.py --seed 7 --per-level 50
"""
import argparse
import json
import os
import random

BASE = os.path.dirname(os.path.abspath(__file__))
INDEX = os.path.join(BASE, "level_index.json")
LEVELS = ["L1", "L2", "L3", "L4", "L5", "L6"]
SAMPLED_LEVELS = ["L1", "L2", "L3", "L4", "L5"]  # 随机抽样的层级
FIXED_LEVELS = ["L6"]                           # 全量保留的层级
SEED = 42
PER_LEVEL = 40


def build_sampled(index, seed=SEED, per_level=PER_LEVEL):
    """按层级索引生成抽样字典：L1-L5 定种子随机抽样，L6 全量。"""
    rng = random.Random(seed)
    sampled = {}
    for lv in SAMPLED_LEVELS:
        pool = index[lv]
        if len(pool) < per_level:
            raise ValueError(f"{lv} 只有 {len(pool)} 题，不足每级 {per_level} 题")
        sampled[lv] = sorted(rng.sample(pool, per_level))
    for lv in FIXED_LEVELS:
        sampled[lv] = sorted(index[lv])
    return {lv: sampled[lv] for lv in LEVELS}  # 保证键顺序固定


def main():
    parser = argparse.ArgumentParser(description="从 level_index.json 分层抽样题目 ID")
    parser.add_argument("--seed", type=int, default=SEED)
    parser.add_argument("--per-level", type=int, default=PER_LEVEL, help="L1-L5 每级抽样题数")
    args = parser.parse_args()

    with open(INDEX, encoding="utf-8") as f:
        index = {lv: [int(i) for i in ids] for lv, ids in json.load(f).items()}

    sampled = build_sampled(index, seed=args.seed, per_level=args.per_level)

    out = os.path.join(BASE, f"sampled_ids_seed{args.seed}.json")
    with open(out, "w", encoding="utf-8") as f:
        json.dump(sampled, f, ensure_ascii=False, indent=1)

    for lv in LEVELS:
        print(f"{lv}: pool={len(index[lv])} sampled={len(sampled[lv])}")
    print(f"total={sum(len(v) for v in sampled.values())} -> {out}")


if __name__ == "__main__":
    main()
