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
|           Method |       Mean |     Error |    StdDev |  Gen 0 |  Gen 1 | Allocated |
|----------------- |-----------:|----------:|----------:|-------:|-------:|----------:|
|       FileManual | 386.678 us | 3.8050 us | 5.5774 us | 0.9766 |      - |   4.09 KB |
|    FileGenerated | 390.399 us | 2.3770 us | 3.5578 us | 0.9766 |      - |   4.09 KB |
|   FileGenerated2 | 394.520 us | 1.9585 us | 2.8708 us | 0.9766 |      - |   4.57 KB |
|     MemoryManual |   9.711 us | 0.3156 us | 0.4526 us | 0.5341 | 0.1221 |    2.2 KB |
|  MemoryGenerated |  11.584 us | 0.0971 us | 0.1423 us | 0.5341 | 0.1221 |    2.2 KB |
| MemoryGenerated2 |  12.821 us | 0.2546 us | 0.3652 us | 0.6409 | 0.1526 |   2.69 KB |
