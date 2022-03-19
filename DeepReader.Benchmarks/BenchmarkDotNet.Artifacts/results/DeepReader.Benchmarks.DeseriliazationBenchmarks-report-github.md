``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1526 (21H2)
Intel Core i5-3470 CPU 3.20GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.201
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  DefaultJob : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


```
|                       Method |          Mean |         Error |        StdDev | Rank |  Gen 0 |  Gen 1 | Allocated |
|----------------------------- |--------------:|--------------:|--------------:|-----:|-------:|-------:|----------:|
|                       Direct |      5.722 ns |     0.0657 ns |     0.0582 ns |    1 | 0.0076 |      - |      24 B |
|                     UsingNew |     15.553 ns |     0.1067 ns |     0.0946 ns |    2 | 0.0153 |      - |      48 B |
|              UsingReflection |    138.970 ns |     0.5296 ns |     0.4423 ns |    5 | 0.0153 |      - |      48 B |
|       UsingFormatterServices |     66.075 ns |     0.3364 ns |     0.3146 ns |    4 | 0.0153 |      - |      48 B |
| UsingActivatorCreateInstance |     24.318 ns |     0.1360 ns |     0.1135 ns |    3 | 0.0153 |      - |      48 B |
|      UsingCompiledExpression | 71,782.253 ns |   539.6418 ns |   504.7813 ns |    6 | 1.2207 | 0.6104 |   3,848 B |
|          UsingReflectionEmit | 87,063.293 ns | 1,727.3185 ns | 2,364.3730 ns |    7 | 0.3662 | 0.1221 |   1,168 B |
