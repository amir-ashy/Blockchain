using Blockchain.Miner;
using Blockchain.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Blockchain
{
    public class Program
    {
        private static ServiceProvider serviceProvider;
        public static void Main(string[] args)
        {
            FillDependency();

            serviceProvider.CreateScope().ServiceProvider.GetRequiredService<Startup>().Run();
        }
        public static void FillDependency()
        {
            serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddTransient<Startup>()
            .AddSingleton<TransactionPool>()
            .AddSingleton<IBlockMiner, BlockMiner>()
            .AddSingleton<IRpcServer, EmbedServer>()
            .BuildServiceProvider();
        }
    }
}