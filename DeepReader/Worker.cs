using System.Diagnostics;
using System.Threading.Channels;
using DeepReader.Classes;
using DeepReader.Types;

namespace DeepReader
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly ChannelWriter<Block> _blocksChannel;

        ParseCtx Ctx = new ParseCtx();

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

            string producerDir = "/app/config/producer/";
            using var producer = new Process();
            producer.StartInfo = new ProcessStartInfo
            {
                FileName = "nodeos",
                ArgumentList =
                {
                    /*"-e" ,"-p", "eosio",*/ "--delete-all-blocks", /*"--deep-mind",*/ "--config-dir", $"{producerDir}", "--data-dir", $"{producerDir}data"
//                    "-e -p eosio --delete-all-blocks --deep-mind --plugin eosio::producer_plugin --plugin eosio::producer_api_plugin --plugin eosio::chain_api_plugin --plugin eosio::http_plugin --plugin eosio::history_plugin --plugin eosio::history_api_plugin --filter-on='*' --access-control-allow-origin='*' --contracts-console --http-validate-host=false --verbose-http-errors"// >> nodeos.log 2 > &1 &
                }, // { "--delete-all-blocks --deep-mind" },
                UseShellExecute = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                CreateNoWindow = true
                //                WorkingDirectory = "/usr/bin",
            };
            producer.Start();

            await Task.Delay(3000);

            string mindreaderDir = "/app/config/mindreader/";
            using var mindreader = new Process();
            mindreader.StartInfo = new ProcessStartInfo
            {
                FileName = "nodeos",
                ArgumentList =
                {
                    /*"-e" ,"-p", "eosio",*/ "--delete-all-blocks", "--config-dir", $"{mindreaderDir}", "--data-dir", $"{mindreaderDir}data"
//                    "-e -p eosio --delete-all-blocks --deep-mind --plugin eosio::producer_plugin --plugin eosio::producer_api_plugin --plugin eosio::chain_api_plugin --plugin eosio::http_plugin --plugin eosio::history_plugin --plugin eosio::history_api_plugin --filter-on='*' --access-control-allow-origin='*' --contracts-console --http-validate-host=false --verbose-http-errors"// >> nodeos.log 2 > &1 &
                }, // { "--delete-all-blocks --deep-mind" },
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                //                WorkingDirectory = "/usr/bin",
            };

            if (mindreader != null)
            {
                mindreader.OutputDataReceived += async (sender, e) => await OnNodeosDataReceived(sender, e, stoppingToken); ;// async (sender, e) => await OnNodeosOutputDataReceived(sender, e, stoppingToken);
                mindreader.ErrorDataReceived += async (sender, e) => await OnNodeosDataReceived(sender, e, stoppingToken);
                mindreader.Exited += OnNodeosExited;

                mindreader.Start();
                mindreader.BeginErrorReadLine();
                mindreader.BeginOutputReadLine();
                //await process.WaitForExitAsync(stoppingToken);
                while (!mindreader.HasExited || !producer.HasExited)
                {
                    await Task.Delay(10000);

                    if(mindreader.HasExited)
                    {
                        Console.WriteLine("MINDREADER EXITED!");
                    }
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
            //try
            //{
                if (e.Data != null)
                {
                    if (e.Data.StartsWith("DMLOG"))
                    {
                        var data = e.Data.Split(' ');

                        switch (data[1])
                        {
                            case "RAM_OP":
                                Ctx.ReadRamOp(data[Range.StartAt(2)]);
                                break;
                            case "CREATION_OP":
                                Ctx.ReadCreationOp(data[Range.StartAt(2)]);
                                break;
                            case "DB_OP":
                                Ctx.ReadDbOp(data[Range.StartAt(2)]);
                                break;
                            case "RLIMIT_OP":
                                Ctx.ReadRlimitOp(data[Range.StartAt(2)]);
                                break;
                            case "TRX_OP":
                                Ctx.ReadTrxOp(data[Range.StartAt(2)]);
                                break;
                            case "APPLIED_TRANSACTION":
                                Ctx.ReadAppliedTransaction(data[Range.StartAt(2)]);
                                break;
                            case "TBL_OP":
                                Ctx.ReadTableOp(data[Range.StartAt(2)]);
                                break;
                            case "PERM_OP":
                                Ctx.ReadPermOp(data[Range.StartAt(2)]);
                                break;
                            case "DTRX_OP CREATE":
                                Ctx.ReadCreateOrCancelDTrxOp("CREATE", data[Range.StartAt(2)]);
                                break;
                            case "DTRX_OP MODIFY_CREATE":
                                Ctx.ReadCreateOrCancelDTrxOp("MODIFY_CREATE", data[Range.StartAt(2)]);
                                break;
                            case "DTRX_OP MODIFY_CANCEL":
                                Ctx.ReadCreateOrCancelDTrxOp("MODIFY_CANCEL", data[Range.StartAt(2)]);
                                break;
                            case "RAM_CORRECTION_OP":
                                Ctx.ReadRamCorrectionOp(data[Range.StartAt(2)]);
                                break;
                            case "DTRX_OP PUSH_CREATE":
                                Ctx.ReadCreateOrCancelDTrxOp("PUSH_CREATE", data[Range.StartAt(2)]);
                                break;
                            case "DTRX_OP CANCEL":
                                Ctx.ReadCreateOrCancelDTrxOp("CANCEL", data[Range.StartAt(2)]);
                                break;
                            case "DTRX_OP FAILED":
                                Ctx.ReadFailedDTrxOp(data[Range.StartAt(2)]);
                                break;
                            case "ACCEPTED_BLOCK":
                                var block = Ctx.ReadAcceptedBlock(data[Range.StartAt(2)]);
                                await _blocksChannel.WriteAsync(block, clt);
                                break;
                            case "START_BLOCK":
                                Ctx.ReadStartBlock(data[Range.StartAt(2)]);
                                break;
                            case "FEATURE_OP ACTIVATE":
                                Ctx.ReadFeatureOpActivate(data[Range.StartAt(2)]);
                                break;
                            case "FEATURE_OP PRE_ACTIVATE":
                                Ctx.ReadFeatureOpPreActivate(data[Range.StartAt(2)]);
                                break;
                            case "SWITCH_FORK":
                                //zlog.Info("fork signal, restarting state accumulation from beginning");
                                Ctx.ResetBlock();
                                break;
                            case "ABIDUMP START":
                                Ctx.ReadAbiStart(data[Range.StartAt(2)]);
                                break;
                            case "ABIDUMP ABI":
                                Ctx.ReadAbiDump(data[Range.StartAt(2)]);
                                break;
                            case "ABIDUMP END":
                                //noop
                                break;
                            case "DEEP_MIND_VERSION":
                                Ctx.ReadDeepmindVersion(data[Range.StartAt(2)]);
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
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    Console.WriteLine(ex.StackTrace);
            //}
        }
    }
}