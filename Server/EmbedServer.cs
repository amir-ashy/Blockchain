using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
using Blockchain.Miner;

namespace Blockchain.Server
{
    public class EmbedServer : IRpcServer
    {
        private readonly IBlockMiner blockMiner;
        private readonly TransactionPool transactionPool;
        private readonly ILogger<EmbedServer> logger;

        private WebServer server;
        private string url;
        public EmbedServer(TransactionPool transactionPool, IBlockMiner blockMiner, ILoggerFactory loggerFactory)
        {
            string port = "5449";
            url = $"http://localhost:{port}/";

            server = CreateWebServer(url);
            this.transactionPool = transactionPool;
            this.blockMiner = blockMiner;
            this.logger = loggerFactory.CreateLogger<EmbedServer>();
        }
        public void Stop()
        {
            server.Dispose();
            logger.LogInformation("http server stopped");
        }
        public void Start()
        {
            // Once we've registered our modules and configured them, we call the RunAsync() method.
            server.RunAsync();
            logger.LogInformation($"http server available at {url}api");
        }

        private WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager()
                .WithWebApi("/api", m => m.WithController(() => new Controller(blockMiner, transactionPool)))
                .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

            return server;
        }

        public sealed class Controller : WebApiController
        {
            private readonly IBlockMiner blockMiner;
            private readonly TransactionPool transactionPool;

            public Controller(IBlockMiner blockMiner, TransactionPool transactionPool)
            {
                this.blockMiner = blockMiner;
                this.transactionPool = transactionPool;
            }

            //GET http://localhost:9696/api/blocks
            [Route(HttpVerbs.Get, "/blocks")]
            public string GetAllBlocks() => JsonConvert.SerializeObject(blockMiner.Blockchain);

            //GET http://localhost:9696/api/blocks/index/{index?}
            [Route(HttpVerbs.Get, "/blocks/index/{index?}")]
            public string GetAllBlocks(int index)
            {
                Model.Block block = null;
                if (index < blockMiner.Blockchain.Count)
                    block = blockMiner.Blockchain[index];
                return JsonConvert.SerializeObject(block);
            }

            //GET http://localhost:9696/api/blocks/latest
            [Route(HttpVerbs.Get, "/blocks/latest")]
            public string GetLatestBlocks()
            {
                var block = blockMiner.Blockchain.LastOrDefault();
                return JsonConvert.SerializeObject(block);
            }

            //Post http://localhost:9696/api/add
            //Body >> {"From":"amir","To":"bob","Amount":10}
            [Route(HttpVerbs.Post, "/add")]
            public void AddTransaction()
            {
                var data = HttpContext.GetRequestDataAsync<Model.Transaction>();
                if (data != null && data.Result != null)
                    transactionPool.AddRaw(data.Result);
            }
        }
    }
}