# ZGeoInference（KM-SCR）

<div align="center">

**基于演绎数据库的高效几何自动推理系统**

[English / 英文文档](README.md)

</div>

---

## 简介

基于规则的演绎数据库通过结合几何命题的已知条件与公理、定理等推理规则进行逻辑推导，在几何自动推理中具备严密的逻辑可靠性与卓越的知识发现能力。然而，在处理复杂几何问题时，推理搜索空间随问题规模呈指数级扩张，导致求解效率遭遇严重瓶颈。针对几何知识表示的冗余以及规则匹配机制过于刚性等问题，本文从知识与规则的双重表示层面切入：

- **几何对象合并表示** —— 对语义等价或可归约的几何对象及其拓扑关系统一建模，消除冗余表示，压缩推理空间。
- **连等式表示** —— 减少等式关系表示数量，加速推理中代数方程组的求解。
- **半条件规则表示** —— 将规则触发条件解耦为前置条件与约束条件，分步匹配，大幅减少匹配次数，克服组合爆炸。

基于该方法实现的 **KM-SCR** 几何定理推理系统，在 FormalGeo7k-L6 基准数据集上以平均 **4.06 秒**的推理耗时实现了 **98.29%** 的求解准确率，显著优于已有方法。

## 核心机制

| 机制 | 说明 |
| --- | --- |
| 几何对象合并表示 | 统一建模语义等价/可归约对象，消除冗余，压缩推理空间。 |
| 连等式表示 | 减少等式关系数量，加速代数方程组求解。 |
| 半条件规则表示 | 解耦触发条件、分步匹配，减少匹配次数，避免组合爆炸。 |

下图展示了本方法中**规则实例化**与**知识实例化**的压缩归约效果。

![CompassionAndReduction](Result/FormalGeo7KL6_Reports/CompressionRate.png)

## 实验结果

| 指标 | 数值 |
| --- | --- |
| 求解准确率 | **98.29%** |
| 平均推理耗时 | **4.06 秒** |

## 运行环境

- **.NET 10**（目标框架 `net10.0`）
- **Maple 2024**（系统通过 `maplec.dll` 调用 Maple 内核进行代数求解；请安装 Maple 2024 并配置运行环境）
- Windows 操作系统

## 调整并行线程数量

推理过程支持多线程并行处理，可通过设置最大并发线程数控制同时运行的推理任务数量。默认最大并发数为 `5`。

在 `GeoInference.Tests/Program.cs` 中通过 `MaxThreads` 属性进行设置：

```csharp
IntegratedTester tester = new IntegratedTester();
// 根据机器性能调整并发线程数量
tester.MaxThreads = 5;
await tester.Test();
```

由于推理过程中涉及代数求解以及较为密集的计算任务，**增加线程数并不一定能够提高整体推理速度**。当并发线程过多时，多个推理任务可能产生 CPU、内存及 Maple 计算资源的竞争，从而导致单个任务运行时间增加，甚至出现计算异常（例如返回时间字符串）。

因此，建议根据实际硬件配置选择合适的并发数；在部分机器上，**适当降低 `MaxThreads` 反而可以获得更稳定、更快的推理性能**。

## 构建与运行

建议使用 **Release 配置**运行系统，以减少 Debug 模式带来的额外运行开销，并获得更稳定的推理性能。

```bash
# Release 模式构建
dotnet build GeoInference.slnx -c Release

# Release 模式运行集成测试（在 FormalGeo7k-L6 上批量推理）
dotnet run --project GeoInference.Tests -c Release
```

运行前，请根据机器性能适当调整 `GeoInference.Tests/Program.cs` 中的 `MaxThreads`。例如：

```csharp
IntegratedTester tester = new IntegratedTester();
tester.MaxThreads = 3;
await tester.Test();
```

系统会自动定位 `Datasets/FormalGeo7KL6` 数据集，并将推理结果输出至 `Result/` 目录，包括推理 JSON、Excel 报告及相关图表。

> **运行建议：** 为保证推理性能和结果稳定性，建议使用 Release 配置运行，并根据机器实际性能适当降低并发线程数。线程数过高可能因计算资源竞争导致推理速度下降或出现异常。

## 项目结构

```
GeoInference/
├── GeoInference/               # 核心推理引擎（.NET 10 可执行程序）
├── GeoInference.Tests/         # 批量测试与结果报告生成
├── Datasets/
│   └── FormalGeo7KL6/          # FormalGeo7k-L6 基准数据集
├── Result/                     # 推理结果、报告与图表
├── GeoInference.slnx           # 解决方案文件
├── README.md                   # 英文文档
└── README.zh-CN.md             # 中文文档
```
