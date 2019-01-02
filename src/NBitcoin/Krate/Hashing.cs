using System;
using System.Collections.Generic;
using System.Text;
using Konscious.Security.Cryptography;

namespace NBitcoin.Krate
{
    public static class Hashing
    {
        public static uint256 GetArgonHash(this byte[] data)
        {
            var argon2 = new Argon2d(data);

            // TODO: figure out the right numbers here.
            argon2.DegreeOfParallelism = 4;
            argon2.MemorySize = 8192;
            argon2.Iterations = 10;
            argon2.Salt = null;
            argon2.AssociatedData = null;

            var hash = argon2.GetBytes(256 / 8);
            return new uint256(hash, true);
        }
    }
}
