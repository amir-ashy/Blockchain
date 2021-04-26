using Blockchain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Miner
{
    public interface IBlockMiner
    {
        List<Block> Blockchain { get; }

        void Start();

        void Stop();
    }
}
