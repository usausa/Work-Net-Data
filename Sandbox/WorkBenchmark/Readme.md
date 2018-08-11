``` ini

BenchmarkDotNet=v0.11.0, OS=Windows 10.0.17134.112 (1803/April2018Update/Redstone4)
Intel Core i7-4771 CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3415986 Hz, Resolution=292.7412 ns, Timer=TSC
.NET Core SDK=2.1.302
  [Host]    : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT
  MediumRun : .NET Core 2.1.2 (CoreCLR 4.6.26628.05, CoreFX 4.6.26629.01), 64bit RyuJIT

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  

```
|           Method |      Mean |      Error |     StdDev |  Gen 0 |  Gen 1 | Allocated |
|----------------- |----------:|-----------:|-----------:|-------:|-------:|----------:|
|       FileManual | 389.88 us |  2.2694 us |  3.3967 us | 0.9000 |      - |   4.08 KB |
|    FileGenerated | 405.15 us |  4.7787 us |  7.0046 us | 0.9000 |      - |   4.08 KB |
|   FileGenerated2 | 423.10 us | 10.7775 us | 15.4568 us | 1.1000 |      - |   4.57 KB |
|     MemoryManual |  10.33 us |  0.4239 us |  0.6213 us | 0.5333 | 0.1000 |    2.2 KB |
|  MemoryGenerated |  10.78 us |  0.2058 us |  0.3081 us | 0.5200 | 0.1200 |    2.2 KB |
| MemoryGenerated2 |  13.05 us |  0.2195 us |  0.3286 us | 0.6333 | 0.1333 |   2.69 KB |
