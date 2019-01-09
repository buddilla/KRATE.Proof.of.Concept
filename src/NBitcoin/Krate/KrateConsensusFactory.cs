using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NBitcoin.Krate
{
    public class KrateConsensusFactory : ConsensusFactory
    {
        public KrateConsensusFactory() : base()
        { }

        public override Block CreateBlock()
        {
            return new KrateBlock(CreateBlockHeader());
        }

        public override BlockHeader CreateBlockHeader()
        {
            return new KrateBlockHeader();
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public class KrateBlock : Block
#pragma warning restore CS0618 // Type or member is obsolete
    {
        public KrateBlock(BlockHeader header) : base(header)
        { }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public class KrateBlockHeader : BlockHeader
#pragma warning restore CS0618 // Type or member is obsolete
    {
        public override uint256 GetHash()
        {

            uint256 hash = null;
            uint256[] hashes = this.hashes;

            if (hashes != null)
                hash = hashes[0];

            if (hash != null)
                return hash;

            using (var ms = new MemoryStream())
            {
                var stream = new BitcoinStream(ms, true);
                ReadWriteHashingStream(stream);

                hash = ms.ToArray().GetArgonHash();
            }

            hashes = this.hashes;
            if (hashes != null)
            {
                hashes[0] = hash;
            }

            return hash;

        }
    }
}
