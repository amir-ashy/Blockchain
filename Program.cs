using Blockchain.Miner;
using Blockchain.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Blockchain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DependencyManager.Fill();
            DependencyManager.BlockMiner.Start();
            DependencyManager.RpcServer.Start();
            Console.ReadKey();
            DependencyManager.BlockMiner.Stop();
            DependencyManager.RpcServer.Stop();
        }
    }
    public static class DependencyManager
    {
        public static TransactionPool TransactionPool => serviceProvider.GetService<TransactionPool>();
        public static IBlockMiner BlockMiner => serviceProvider.GetService<IBlockMiner>();
        public static IRpcServer RpcServer => serviceProvider.GetService<IRpcServer>();
        public static ILogger<T> GetLogger<T>() => serviceProvider.GetService<ILoggerFactory>().CreateLogger<T>();

        private static ServiceProvider serviceProvider;
        public static void Fill()
        {
            serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<TransactionPool>()
            .AddSingleton<IBlockMiner, BlockMiner>()
            .AddSingleton<IRpcServer>(instance => new EmbedServer("5449"))
            .BuildServiceProvider();

        }


    }
}