using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.Krate;
using Stratis.Bitcoin.Networks.Deployments;

namespace Stratis.Bitcoin.Networks
{
    public interface IKrateNetwork
    {
        uint256 CreateGenesisBlock(uint nonce, uint time);
    }

    public class KrateMain : Network, IKrateNetwork
    {
        public KrateMain()
        {
            this.Name = "Main";
            this.AdditionalNames = new List<string> { "Mainnet" };

            this.RootFolderName = KrateRootFolderName;
            this.DefaultConfigFilename = KrateDefaultConfigFilename;
            // The message start string is designed to be unlikely to occur in normal data.
            // The characters are rarely used upper ASCII, not valid as UTF-8, and produce
            // a large 4-byte int at any alignment.
            this.Magic = 0xDAB4BEF9;
            this.DefaultPort = 18998;
            this.DefaultMaxOutboundConnections = 8;
            this.DefaultMaxInboundConnections = 117;
            this.RPCPort = 18997;
            this.MaxTimeOffsetSeconds = KrateMaxTimeOffsetSeconds;
            this.MaxTipAge = KrateDefaultMaxTipAgeInSeconds;
            this.MinTxFee = 1000;
            this.FallbackFee = 20000;
            this.MinRelayTxFee = 1000;
            this.CoinTicker = "KR8";

            var consensusFactory = new KrateConsensusFactory();

            // Create the genesis block.
            this.GenesisTime = 1544806167;
            this.GenesisNonce = 1298;
            this.GenesisBits = new Target(uint256.Parse("000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")).ToCompact();
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Coins(50m);

            Block genesisBlock = CreateGenesisBlock(consensusFactory, this.GenesisTime, this.GenesisNonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward);

            this.Genesis = genesisBlock;

            var buriedDeployments = new BuriedDeploymentsArray
            {
                [BuriedDeployments.BIP34] = 0,
                [BuriedDeployments.BIP65] = 0,
                [BuriedDeployments.BIP66] = 0
            };

            var bip9Deployments = new KrateBIP9Deployments
            {
            };

            this.Consensus = new NBitcoin.Consensus(
                consensusFactory: consensusFactory,
                consensusOptions: new ConsensusOptions(), // Default - set to Bitcoin params.
                coinType: 0,
                hashGenesisBlock: genesisBlock.GetHash(),
                subsidyHalvingInterval: 210000,
                majorityEnforceBlockUpgrade: 750,
                majorityRejectBlockOutdated: 950,
                majorityWindow: 1000,
                buriedDeployments: buriedDeployments,
                bip9Deployments: bip9Deployments,
                bip34Hash: genesisBlock.GetHash(),
                ruleChangeActivationThreshold: 1916, // 95% of 2016,
                minerConfirmationWindow: 2016, // nPowTargetTimespan / nPowTargetSpacing
                maxReorgLength: 0,
                defaultAssumeValid: genesisBlock.GetHash(),
                maxMoney: 21000000 * Money.COIN,
                coinbaseMaturity: 1,
                premineHeight: 0,
                premineReward: Money.Zero,
                proofOfWorkReward: Money.Coins(50),
                powTargetTimespan: TimeSpan.FromSeconds(14 * 24 * 60 * 60), // two weeks
                powTargetSpacing: TimeSpan.FromSeconds(10 * 60),
                powAllowMinDifficultyBlocks: false,
                powNoRetargeting: false,
                powLimit: new Target(new uint256("00ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                minimumChainWork: new uint256("0x0000000000000000000000000000000000000000000000000000000000000001"),
                isProofOfStake: false,
                lastPowBlock: default(int),
                proofOfStakeLimit: null,
                proofOfStakeLimitV2: null,
                proofOfStakeReward: Money.Zero
            );

            this.Base58Prefixes = new byte[12][];
            this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { (0) };
            this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { (5) };
            this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (128) };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
            this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
            this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x88), (0xB2), (0x1E) };
            this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x88), (0xAD), (0xE4) };
            this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
            this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
            this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2a };
            this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 23 };
            this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

            var encoder = new Bech32Encoder("k8");
            this.Bech32Encoders = new Bech32Encoder[2];
            this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
            this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;

            this.Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                {0, new CheckpointInfo(uint256.Parse("0x0000112cd681fed02f66ca6d7591d08b2dbb8250cedaa479638471bc26d42789"))},
            };

            this.DNSSeeds = new List<DNSSeedData>
            {
            };

            string[] seedNodes =
            {
            };
            this.SeedNodes = ConvertToNetworkAddresses(seedNodes, this.DefaultPort).ToList();

            // TODO: calculate real genesis block, assert it here.
            Assert(this.Consensus.HashGenesisBlock == uint256.Parse("0x00065faa0a70f6f0a63f75785964e4bc23adfc453da1ed993ab41d14c69d40b8"));
            Assert(this.Genesis.Header.HashMerkleRoot == uint256.Parse("0xfe4d1a9594a269bcba4ae55afefdb9f9a776628c388315f29032815efabdb949"));
        }

        /// <summary> Bitcoin maximal value for the calculated time offset. If the value is over this limit, the time syncing feature will be switched off. </summary>
        public const int KrateMaxTimeOffsetSeconds = 70 * 60;

        /// <summary> Bitcoin default value for the maximum tip age in seconds to consider the node in initial block download (24 hours). </summary>
        public const int KrateDefaultMaxTipAgeInSeconds = 24 * 60 * 60 * 5000; //TODO: 5000 days for debug purposes

        /// <summary> The name of the root folder containing the different Bitcoin blockchains (Main, TestNet, RegTest). </summary>
        public const string KrateRootFolderName = "krate";

        /// <summary> The default name used for the Bitcoin configuration file. </summary>
        public const string KrateDefaultConfigFilename = "krate.conf";

        public static Block CreateGenesisBlock(ConsensusFactory consensusFactory, uint nTime, uint nNonce, uint nBits, int nVersion, Money genesisReward)
        {
            string pszTimestamp = "On the verge of a new distributed filesystem";

            Transaction txNew = consensusFactory.CreateTransaction();
            txNew.Version = 1;
            txNew.AddInput(new TxIn()
            {
                ScriptSig = new Script(Op.GetPushOp(0), new Op()
                {
                    Code = (OpcodeType)0x1,
                    PushData = new[] { (byte)4 }
                }, Op.GetPushOp(Encoders.ASCII.DecodeData(pszTimestamp)))
            });
            txNew.AddOutput(new TxOut()
            {
                Value = genesisReward,
            });

            Block genesis = consensusFactory.CreateBlock();
            genesis.Header.BlockTime = Utils.UnixTimeToDateTime(nTime);
            genesis.Header.Bits = nBits;
            genesis.Header.Nonce = nNonce;
            genesis.Header.Version = nVersion;
            genesis.Transactions.Add(txNew);
            genesis.Header.HashPrevBlock = uint256.Zero;
            genesis.UpdateMerkleRoot();
            return genesis;
        }

        public uint256 CreateGenesisBlock(uint nonce, uint time)
        {
            return CreateGenesisBlock(Consensus.ConsensusFactory, time, nonce, this.GenesisBits, this.GenesisVersion, this.GenesisReward).GetHash();
        }
    }
}
