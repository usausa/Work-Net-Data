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
|     SimpleManual | 384.6 us | 2.924 us | 4.376 us | 0.9000 |   4.07 KB |
|  SimpleGenerated | 392.9 us | 1.333 us | 1.995 us | 1.1000 |   4.55 KB |
