namespace Krate.KrateD
{
    using System;
    using System.Threading.Tasks;
    using NBitcoin;
    using Stratis.Bitcoin;
    using Stratis.Bitcoin.Builder;
    using Stratis.Bitcoin.Configuration;
    using Stratis.Bitcoin.Features.Api;
    using Stratis.Bitcoin.Features.Apps;
    using Stratis.Bitcoin.Features.BlockStore;
    using Stratis.Bitcoin.Features.Consensus;
    using Stratis.Bitcoin.Features.MemoryPool;
    using Stratis.Bitcoin.Features.Miner;
    using Stratis.Bitcoin.Features.RPC;
    using Stratis.Bitcoin.Features.Wallet;
    using Stratis.Bitcoin.Networks;
    using Stratis.Bitcoin.Utilities;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var nodeSettings = new NodeSettings(networksSelector: Networks.Krate, args: args);

                IFullNode node = new FullNodeBuilder()
                    .UseNodeSettings(nodeSettings)
                    .UseBlockStore()
                    .UsePowConsensus()
                    .UseMempool()
                    .AddMining()
                    .AddRPC()
                    .UseWallet()
                    .UseApi()
                    .UseApps()
                    .Build();

                if (node != null)
                {
                    var config = nodeSettings.ConfigReader;
                    if (config.GetOrDefault("create-genesis", false))
                    {
                        await GenerateGenesisHash(node, nodeSettings);
                        return;
                    }

                    if (config.GetOrDefault("create-private-key", false))
                    {
                        await GeneratePrivateKey(node, nodeSettings);
                        return;
                    }

                    await node.RunAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.ToString());
            }
        }

        private static async Task GeneratePrivateKey(IFullNode node, NodeSettings nodeSettings)
        {
            var privateKey = new Key();
            var addressString = privateKey.PubKey.GetAddress(nodeSettings.Network).ToString();
            var privateKeyString = privateKey.GetWif(nodeSettings.Network).ToString();

            Console.WriteLine($"Private Key: {privateKeyString}");
            Console.WriteLine($"Address: {addressString}");
        }

        private static async Task GenerateGenesisHash(IFullNode node, NodeSettings nodeSettings)
        {
            var network = node.Network as IKrateNetwork;
            var config = nodeSettings.ConfigReader;

            var target = new Target(node.Network.GenesisBits).ToUInt256();

            uint currentTime = node.Network.GenesisTime;
            string spaces = new string(' ', 40);

            uint bestNonce = config.GetOrDefault("genesis-best-nonce", (uint)0);
            uint256 bestHash = network.CreateGenesisBlock(bestNonce, currentTime);
            uint currentNonce = config.GetOrDefault("genesis-start-nonce", (uint)bestNonce + 1);
            Console.WriteLine($"{DateTime.Now} best: {bestNonce} => {bestHash}");

            object nonceLocker = new object();

            (uint Nonce, uint Time) nextNonce()
            {
                lock (nonceLocker)
                {
                    if (++currentNonce == 0)
                    {
                        // ran out of nonces, try new time.
                        ++currentTime;
                    }

                    return (currentNonce, currentTime);
                }
            }

            object bestLocker = new object();

            void Miner()
            {
                while (bestHash > target)
                {
                    var current = nextNonce();

                    var hash = network.CreateGenesisBlock(current.Nonce, current.Time);

                    Console.Write($"{current.Nonce} = {hash}\r");

                    lock (bestLocker)
                    {
                        if (hash < bestHash)
                        {
                            bestNonce = current.Nonce;
                            bestHash = hash;

                            Console.WriteLine($"{DateTime.Now} best: {bestNonce} => {bestHash}");
                        }
                    }

                    if (current.Nonce % 1000 == 0)
                    {
                        Console.WriteLine($"{DateTime.Now} Checked {current.Nonce}{spaces}");
                    }
                }
            }

            Task.WaitAll(
                Task.Run(() => Miner()),
                Task.Run(() => Miner()),
                Task.Run(() => Miner())
            );

            if (bestHash < target)
            {
                Console.WriteLine($"Found nonce = {bestNonce}, hash = {bestHash}, time = {currentTime}");
            }
            else
            {
                Console.WriteLine("Cannot find candidate nonce for genesis block");
            }

        }
    }
}
