using NBitcoin;

namespace Stratis.Bitcoin.Networks.Deployments
{
    /// <summary>
    /// BIP9 deployments for the Bitcoin network.
    /// </summary>
    public class KrateBIP9Deployments : BIP9DeploymentsArray
    {
        // The position of each deployment in the deployments array.
        public const int TestDummy = 0;
        public const int CSV = 1;
        public const int Segwit = 2;

        // The number of deployments.
        public const int NumberOfDeployments = 3;

        /// <summary>
        /// Constructs the BIP9 deployments array.
        /// </summary>
        public KrateBIP9Deployments() : base(NumberOfDeployments)
        {
        }

        /// <summary>
        /// Gets the deployment flags to set when the deployment activates.
        /// </summary>
        /// <param name="deployment">The deployment number.</param>
        /// <returns>The deployment flags.</returns>
        public override BIP9DeploymentFlags GetFlags(int deployment)
        {
            BIP9DeploymentFlags flags = new BIP9DeploymentFlags();

            return flags;
        }
    }
}
