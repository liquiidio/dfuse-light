using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeepReader
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
        
        private async Task StartNodeos(CancellationToken stoppingToken)
        {
            using var process = Process.Start(
                new ProcessStartInfo
                {
                    // TOOD
                    FileName = "echo",
                    ArgumentList = { "hello world" },
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                });

            if (process != null)
            {
                process.OutputDataReceived += OnNodeosOutputDataReceived;

                process.Start();
                process.BeginOutputReadLine();

                await process.WaitForExitAsync(stoppingToken);
            }
        }
        
        private void OnNodeosOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.StartsWith("DMLOG"))
                {
                    var ctx = new ParseCtx();

					var data = e.Data.Split(' ');

                    switch (data[1])
                    {
						case "RAM_OP":
                            ctx.ReadRAMOp(data[Range.StartAt(2)]);
							break;
                        case "CREATION_OP":
                            ctx.ReadCreationOp(data[Range.StartAt(2)]);
                            break;
                        case "DB_OP":
                            ctx.ReadDBOp(data[Range.StartAt(2)]);
							break;
                        case "RLIMIT_OP":
                            ctx.ReadRlimitOp(data[Range.StartAt(2)]);
							break;
                        case "TRX_OP":
                            ctx.ReadTrxOp(data[Range.StartAt(2)]);
							break;
                        case "APPLIED_TRANSACTION":
                            ctx.ReadAppliedTransaction(data[Range.StartAt(2)]);
							break;
                        case "TBL_OP":
                            ctx.ReadTableOp(data[Range.StartAt(2)]);
							break;
                        case "PERM_OP":
                            ctx.ReadPermOp(data[Range.StartAt(2)]);
							break;
                        case "DTRX_OP CREATE":
                            ctx.ReadCreateOrCancelDTrxOp("CREATE", data[Range.StartAt(2)]);
							break;
                        case "DTRX_OP MODIFY_CREATE":
                            ctx.ReadCreateOrCancelDTrxOp("MODIFY_CREATE", data[Range.StartAt(2)]);
							break;
                        case "DTRX_OP MODIFY_CANCEL":
                            ctx.ReadCreateOrCancelDTrxOp("MODIFY_CANCEL", data[Range.StartAt(2)]);
							break;
                        case "RAM_CORRECTION_OP":
                            ctx.ReadRAMCorrectionOp(data[Range.StartAt(2)]);
							break;
                        case "DTRX_OP PUSH_CREATE":
                            ctx.ReadCreateOrCancelDTrxOp("PUSH_CREATE", data[Range.StartAt(2)]);
							break;
                        case "DTRX_OP CANCEL":
                            ctx.ReadCreateOrCancelDTrxOp("CANCEL", data[Range.StartAt(2)]);
							break;
                        case "DTRX_OP FAILED":
                            ctx.ReadFailedDTrxOp(data[Range.StartAt(2)]);
							break;
                        case "ACCEPTED_BLOCK":
							var block = ctx.ReadAcceptedBlock(data[Range.StartAt(2)]);

            			    /*if err != nil {
							    return null;, l.formatError(line, err);
                            }*/
//                            return block;
                            break;
                        case "START_BLOCK":
                            ctx.ReadStartBlock(data[Range.StartAt(2)]);
							break;
                        case "FEATURE_OP ACTIVATE":
                            ctx.ReadFeatureOpActivate(data[Range.StartAt(2)]);
							break;
                        case "FEATURE_OP PRE_ACTIVATE":
                            ctx.ReadFeatureOpPreActivate(data[Range.StartAt(2)]);
							break;
                        case "SWITCH_FORK":
                            //zlog.Info("fork signal, restarting state accumulation from beginning");
                            ctx.ResetBlock();
							break;
                        case "ABIDUMP START":
                            ctx.ReadABIStart(data[Range.StartAt(2)]);
							break;
                        case "ABIDUMP ABI":
                            ctx.ReadABIDump(data[Range.StartAt(2)]);
							break;
                        case "ABIDUMP END":
						//noop
    						break;
						case "DEEP_MIND_VERSION":
                            ctx.ReadDeepmindVersion(data[Range.StartAt(2)]);
							break;
                        default:
                            break;
                            //zlog.Info("unknown log line", zap.String("line", data));
                    }
				}
            }
        }
    }
}