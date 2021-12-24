using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DeepReader.Classes;
using DeepReader.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeepReader
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly ChannelWriter<Block> _blocksChannel;

        public Worker(ILogger<Worker> logger, ChannelWriter<Block> blocksChannel)
        {
            _logger = logger;
            _blocksChannel = blocksChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartNodeos(stoppingToken);
        }

        private async Task StartNodeos(CancellationToken stoppingToken)
        {
            // TODO check nodeos version, provide arguments-list

//nodeos -e -p eosio \
//--delete-all-blocks \
//--deep-mind \
//--plugin eosio::producer_plugin \
//--plugin eosio::producer_api_plugin \
//--plugin eosio::chain_api_plugin \
//--plugin eosio::http_plugin \
//--plugin eosio::history_plugin \
//--plugin eosio::history_api_plugin \
//--filter-on="*" \
//--access-control-allow-origin='*' \
//--contracts-console \
//--http-validate-host=false \
//--verbose-http-errors >> nodeos.log 2 > &1 &

            //var args = new System.Collections.ObjectModel.Collection<string>()
            //{
            //    "-e", "-p eosio",
            //    "--delete-all-blocks",
            //    "--deep-mind",
            //    "--plugin eosio::producer_plugin",
            //    "--plugin eosio::producer_api_plugin",
            //    "--plugin eosio::chain_api_plugin",
            //    "--plugin eosio::http_plugin",
            //    "--plugin eosio::history_plugin",
            //    "--plugin eosio::history_api_plugin",
            //    "--filter-on = \"*\"",
            //    "--access-control-allow-origin = '*'",
            //    "--contracts-console",
            //    "--http-validate-host = false",
            //    "--verbose-http-errors >> nodeos.log 2 > &1 &"
            //};

            //ArgumentList =
            //    {
            //    "-e", "-p eosio",
            //        "--delete-all-blocks",
            //        "--deep-mind",
            //        "--plugin eosio::producer_plugin",
            //        "--plugin eosio::producer_api_plugin",
            //        "--plugin eosio::chain_api_plugin",
            //        "--plugin eosio::http_plugin",
            //        "--plugin eosio::history_plugin",
            //        "--plugin eosio::history_api_plugin",
            //        "--filter-on = \"*\"",
            //        "--access-control-allow-origin = '*'",
            //        "--contracts-console",
            //        "--http-validate-host = false",
            //        "--verbose-http-errors >> nodeos.log 2 > &1 &"
            //    }, // { "--delete-all-blocks --deep-mind" },


            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "nodeos",
                ArgumentList =
                {
                    "-e" ,"-p", "eosio", "--delete-all-blocks", "--deep-mind"
//                    "-e -p eosio --delete-all-blocks --deep-mind --plugin eosio::producer_plugin --plugin eosio::producer_api_plugin --plugin eosio::chain_api_plugin --plugin eosio::http_plugin --plugin eosio::history_plugin --plugin eosio::history_api_plugin --filter-on='*' --access-control-allow-origin='*' --contracts-console --http-validate-host=false --verbose-http-errors"// >> nodeos.log 2 > &1 &
                }, // { "--delete-all-blocks --deep-mind" },
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WorkingDirectory = "/usr/bin",
            };

            if (process != null)
            {
                process.OutputDataReceived += async (sender, e) => await OnNodeosDataReceived(sender, e, stoppingToken); ;// async (sender, e) => await OnNodeosOutputDataReceived(sender, e, stoppingToken);
                process.ErrorDataReceived += async (sender, e) => await OnNodeosDataReceived(sender, e, stoppingToken);
                process.Exited += OnNodeosExited;

                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                //await process.WaitForExitAsync(stoppingToken);
                while (!process.HasExited)
                {
                    await Task.Delay(10000);
                    // do nothing
                    string test = "";
                };
                Console.WriteLine("EXITED 1");
            }
        }

        private void OnNodeosExited(object? sender, EventArgs e)
        {
            Console.WriteLine($"Nodeos exited2");
        }

        private async Task OnNodeosDataReceived(object sender, DataReceivedEventArgs e, CancellationToken clt)
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
                            ctx.ReadRamOp(data[Range.StartAt(2)]);
                            break;
                        case "CREATION_OP":
                            ctx.ReadCreationOp(data[Range.StartAt(2)]);
                            break;
                        case "DB_OP":
                            ctx.ReadDbOp(data[Range.StartAt(2)]);
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
                            ctx.ReadRamCorrectionOp(data[Range.StartAt(2)]);
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
                            await _blocksChannel.WriteAsync(block, clt);
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
                            ctx.ReadAbiStart(data[Range.StartAt(2)]);
                            break;
                        case "ABIDUMP ABI":
                            ctx.ReadAbiDump(data[Range.StartAt(2)]);
                            break;
                        case "ABIDUMP END":
                            //noop
                            break;
                        case "DEEP_MIND_VERSION":
                            ctx.ReadDeepmindVersion(data[Range.StartAt(2)]);
                            break;
                        default:
                            Console.WriteLine(e.Data);
                            break;
                            //zlog.Info("unknown log line", zap.String("line", data));
                    }
                }
                else
                    Console.WriteLine($"{e.Data}");
            }
            else
                Console.WriteLine("data is null");
        }
    }
}