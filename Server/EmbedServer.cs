using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Blockchain.Server
{
    public class EmbedServer : IRpcServer
    {
        private WebServer server;
        private string url;
        public EmbedServer(string port)
        {
            url = $"http://localhost:{port}/";

            server = CreateWebServer(url);
        }
        public void Stop()
        {
            server.Dispose();
            DependencyManager.GetLogger<EmbedServer>().LogInformation("http server stopped");
        }
        public void Start()
        {
            // Once we've registered our modules and configured them, we call the RunAsync() method.
            server.RunAsync();
            DependencyManager.GetLogger<EmbedServer>().LogInformation($"http server available at {url}");
        }

        private WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager()
                .WithWebApi("/api", m => m.WithController<Controller>())
                .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

            return server;
        }

        public sealed class Controller : WebApiController
        {

            //GET http://localhost:9696/api/blocks
            [Route(HttpVerbs.Get, "/blocks")]
            public string GetAllBlocks() => JsonConvert.SerializeObject(DependencyManager.BlockMiner.Blockchain);

            //GET http://localhost:9696/api/blocks/index/{index?}
            [Route(HttpVerbs.Get, "/blocks/index/{index?}")]
            public string GetAllBlocks(int index)
            {
                Model.Block block = null;
                if (index < DependencyManager.BlockMiner.Blockchain.Count)
                    block = DependencyManager.BlockMiner.Blockchain[index];
                return JsonConvert.SerializeObject(block);
            }

            //GET http://localhost:9696/api/blocks/latest
            [Route(HttpVerbs.Get, "/blocks/latest")]
            public string GetLatestBlocks()
            {
                var block = DependencyManager.BlockMiner.Blockchain.LastOrDefault();
                return JsonConvert.SerializeObject(block);
            }

            //Post http://localhost:9696/api/add
            //Body >> {"From":"amir","To":"bob","Amount":10}
            [Route(HttpVerbs.Post, "/add")]
            public void AddTransaction()
            {
                var data = HttpContext.GetRequestDataAsync<Model.Transaction>();
                if (data != null && data.Result != null)
                    DependencyManager.TransactionPool.AddRaw(data.Result);
            }
        }
    }
}