```

BenchmarkDotNet v0.13.12, Ubuntu 22.04.4 LTS (Jammy Jellyfish) WSL
13th Gen Intel Core i7-13800H, 1 CPU, 20 logical and 10 physical cores
.NET SDK 8.0.105
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method                   | Mean      | Error    | StdDev   |
|------------------------- |----------:|---------:|---------:|
| ScanEntireDirectory      |   9.804 s | 0.1047 s | 0.0874 s |
| ScanEachFileIndividually | 260.080 s | 5.0800 s | 8.0575 s |
