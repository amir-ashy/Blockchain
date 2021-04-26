using Blockchain.Miner;
using Blockchain.Server;
using System;

namespace Blockchain
{
    public class Startup
    {
        private readonly IBlockMiner blockMiner;
        private readonly IRpcServer rpcServer;

        public Startup(IBlockMiner blockMiner, IRpcServer rpcServer)
        {
            this.blockMiner = blockMiner;
            this.rpcServer = rpcServer;
        }

        public void Run()
        {
            blockMiner.Start();
            rpcServer.Start();
            Console.ReadKey();
            blockMiner.Stop();
            rpcServer.Stop();
        }
    }
}
