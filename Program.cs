using Blockchain.Miner;
using Blockchain.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Blockchain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = FillDependency();

            serviceProvider.CreateScope().ServiceProvider.GetRequiredService<Startup>().Run();
        }
        public static ServiceProvider FillDependency()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json", false)
            .Build();

            return new ServiceCollection()
             .AddLogging(builder => builder.AddConsole())
             .AddTransient<Startup>()
             .AddSingleton(configuration)
             .AddSingleton<TransactionPool>()
             .AddSingleton<IBlockMiner, BlockMiner>()
             .AddSingleton<IRpcServer, EmbedServer>()
             .BuildServiceProvider();
        }
    }
}