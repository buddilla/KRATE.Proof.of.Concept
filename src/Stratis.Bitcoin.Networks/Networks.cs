using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace Stratis.Bitcoin.Networks
{
    public static class Networks
    {
        public static NetworksSelector Krate
        {
            get
            {
                return new NetworksSelector(() => new KrateMain(), () => new KrateTest(), () => new KrateRegTest());
            }
        }

        public static NetworksSelector Bitcoin
        {
            get
            {
                return new NetworksSelector(() => new BitcoinMain(), () => new BitcoinTest(), () => new BitcoinRegTest());
            }
        }

        public static NetworksSelector Stratis
        {
            get
            {
                return new NetworksSelector(() => new StratisMain(), () => new StratisTest(), () => new StratisRegTest());
            }
        }
    }
}
