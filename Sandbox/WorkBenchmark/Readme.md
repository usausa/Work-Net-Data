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
|           Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|----------------- |---------:|---------:|---------:|-------:|----------:|
|     SimpleManual | 386.8 us | 1.463 us | 2.190 us | 0.9000 |   4.07 KB |
|  SimpleGenerated | 407.8 us | 2.776 us | 4.069 us | 0.9000 |   4.07 KB |
| SimpleGenerated2 | 412.2 us | 2.822 us | 4.137 us | 1.1000 |   4.55 KB |
