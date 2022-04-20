``` ini

BenchmarkDotNet=v0.13.1.1738-nightly, OS=Windows 10 (10.0.19044.1526/21H2/November2021Update)
Intel Core i5-3470 CPU 3.20GHz (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.201
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  DefaultJob : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


```
|                 Method |     Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
|----------------------- |---------:|----------:|----------:|-----:|-------:|----------:|
| DeserializeBlockHeader | 5.110 ns | 0.1695 ns | 0.2878 ns |    1 | 0.0076 |      24 B |
