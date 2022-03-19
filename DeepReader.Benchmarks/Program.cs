using BenchmarkDotNet.Running;
using DeepReader.Benchmarks;

var summary = BenchmarkRunner.Run<DeseriliazationBenchmarks>();