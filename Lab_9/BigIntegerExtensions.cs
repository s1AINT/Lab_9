using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_9
{
    public static class BigIntegerExtensions
    {
        public static BigInteger GenerateRandomBigInteger(int bitLength)
        {
            Random random = new Random();
            byte[] data = new byte[bitLength / 8];
            random.NextBytes(data);
            return new BigInteger(data);
        }

        public static BigInteger GenerateRandomNumberBelow(BigInteger below, Random random)
        {
            BigInteger result;
            do
            {
                byte[] bytes = new byte[below.ToByteArray().Length];
                random.NextBytes(bytes);
                result = new BigInteger(bytes);
            } while (result >= below);
            return result;
        }

        public static BigInteger GenerateRandomNumberInRange(BigInteger min, BigInteger max)
        {
            Random random = new Random();
            BigInteger result;
            do
            {
                byte[] bytes = new byte[max.ToByteArray().Length];
                random.NextBytes(bytes);
                result = new BigInteger(bytes);
            } while (result < min || result > max);
            return result;
        }
    }
}
