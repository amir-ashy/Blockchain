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
            DependencyManager.EmbedServer.Start();
            Console.ReadKey();
            DependencyManager.BlockMiner.Stop();
            DependencyManager.EmbedServer.Stop();
        }
    }
    public static class DependencyManager
    {
        public static TransactionPool TransactionPool => serviceProvider.GetService<TransactionPool>();
        public static BlockMiner BlockMiner => serviceProvider.GetService<BlockMiner>();
        public static EmbedServer EmbedServer => serviceProvider.GetService<EmbedServer>();

        private static ServiceProvider serviceProvider;
        public static void Fill()
        {
            serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<TransactionPool>()
            .AddSingleton<BlockMiner>()
            .AddSingleton(instance => new EmbedServer("5449"))
            .BuildServiceProvider();


            ILogger logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            logger.LogInformation("This is a test of the emergency broadcast system.");

        }
    }
}