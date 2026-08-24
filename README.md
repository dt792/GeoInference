# ZGeoInference (KM-SCR)

<div align="center">

**An Efficient Geometry Automated Reasoning System Based on Deductive Databases**

[中文文档 / Chinese](README.zh-CN.md)

</div>

---

## Introduction

Rule-based deductive databases perform logical derivation by combining known conditions of geometric propositions with inference rules such as axioms and theorems, offering rigorous logical reliability and strong knowledge discovery capabilities in automated geometry reasoning. However, when handling complex geometric problems, the inference search space expands exponentially with problem scale, causing a severe efficiency bottleneck. To address the redundancy in geometric knowledge representation and the over-rigidity of rule matching, this project tackles the problem from the dual perspective of knowledge and rule representation:

- **Geometry Object Merging** — uniformly models semantically equivalent or reducible geometric objects and their topological relations, eliminating redundant representations and compressing the inference space.
- **Chain-Equality Representation** — reduces the number of equation relations, enabling fast solving of algebraic equation systems during inference.
- **Semi-Conditional Rules** — decouples rule triggering conditions into preconditions and constraints with step-by-step matching, drastically reducing matches and overcoming combinatorial explosion while preserving reliability.

Based on this method, we implement the **KM-SCR** geometry theorem reasoning system. Experiments on the FormalGeo7k-L6 benchmark achieve **98.29%** solving accuracy with an average inference time of only **7.78 seconds**, significantly outperforming existing methods.

## Core Mechanisms

| Mechanism | Description |
| --- | --- |
| Geometry Object Merging | Unify semantically equivalent/reducible objects to eliminate redundancy and compress the inference space. |
| Chain-Equality Representation | Reduce equation relations and accelerate algebraic equation solving. |
| Semi-Conditional Rules | Decouple triggering conditions and match step-by-step to reduce matches and avoid combinatorial explosion. |

The figure below shows the compression/reduction effect of **rule instantiation** and **knowledge instantiation**.

![CompassionAndReduction](Result/FormalGeo7KL6_Reports/CompressionRate.png)

## Experimental Results

| Metric | Value |
| --- | --- |
| Solving Accuracy | **98.29%** |
| Average Inference Time | **7.78 s** |

## Environment Requirements

- **.NET 10** (target framework `net10.0`)
- **Maple 2024** (the system invokes the Maple kernel via `maplec.dll` for algebraic solving; install Maple 2024 and configure the runtime environment)
- Windows OS

## Adjusting Parallel Thread Count

The inference process supports parallel execution. You can adjust the number of concurrent threads according to your CPU. The default maximum concurrency is `16`.

Set the `MaxThreads` property in `GeoInference.Tests/Program.cs`:

```csharp
IntegratedTester tester = new IntegratedTester();
// Adjust the number of concurrent threads according to your CPU
tester.MaxThreads = 10;
await tester.Test();
```

> **Note**: Excessive threads during inference may cause calculation errors (e.g., returning a time string). Reducing the thread count can help mitigate this issue. Set the value reasonably according to your CPU core count.

## Build & Run

```bash
# Build the solution
dotnet build GeoInference.slnx

# Run the integrated test (batch inference on FormalGeo7k-L6)
dotnet run --project GeoInference.Tests
```

The program auto-locates the `Datasets/FormalGeo7KL6` dataset and outputs results to the `Result/` directory (inference JSON, Excel reports, and charts).

## Project Structure

```
ZGeoInference/
├── GeoInference/               # Core inference engine (.NET 10 executable)
├── GeoInference.Tests/         # Batch testing and result reporting
├── Datasets/
│   └── FormalGeo7KL6/          # FormalGeo7k-L6 benchmark dataset
├── Result/                     # Inference results, reports and charts
├── GeoInference.slnx           # Solution file
└── README.md
```
