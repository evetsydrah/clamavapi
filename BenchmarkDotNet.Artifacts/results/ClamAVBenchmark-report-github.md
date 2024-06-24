```

BenchmarkDotNet v0.13.12, Ubuntu 22.04.4 LTS (Jammy Jellyfish) WSL
13th Gen Intel Core i7-13800H, 1 CPU, 20 logical and 10 physical cores
.NET SDK 8.0.105
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method                   | Mean       | Error     | StdDev     | Median     |
|------------------------- |-----------:|----------:|-----------:|-----------:|
| ScanEntireDirectory      |   5.275 ms | 0.2850 ms |  0.8357 ms |   5.048 ms |
| ScanEachFileIndividually | 105.669 ms | 4.4165 ms | 12.5287 ms | 102.728 ms |
