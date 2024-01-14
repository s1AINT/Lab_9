using System.Numerics;
using System.Text;

namespace Lab_9
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var keys = GenerateKeyPair(64);

            Console.WriteLine($"Публiчнi ключi : {keys.Item1[0]}, {keys.Item1[1]}, {keys.Item1[2]}");
            Console.WriteLine($"Приватний ключ: {keys.Item2}");

            string message = "Hello world!";
            Console.WriteLine($"Текст повiдомлення - {message}");

            var encryptedMessage = EncryptString(keys.Item1, message);
            Console.WriteLine("\nЗашифрований текст :");
            for (int i = 0; i < encryptedMessage.Count; i++)
            {
                Console.WriteLine($"  Елемент {i + 1}: {encryptedMessage[i].Item1}, {encryptedMessage[i].Item2}");
            }

            var decryptedMessage = DecryptString(keys.Item2, encryptedMessage, keys.Item1[0]);
            Console.WriteLine($"Розшифроване повiдомлення: {decryptedMessage}");
        }

        static bool IsPrimeNumber(BigInteger number, int iterations = 10)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number.IsEven) return false;
            BigInteger d = number - 1;
            int r = 0;
            while (d.IsEven)
            {
                d /= 2;
                r += 1;
            }
            Random random = new Random();
            for (int i = 0; i < iterations; i++)
            {
                BigInteger a = 2 + BigIntegerExtensions.GenerateRandomNumberBelow(number - 4, random);
                BigInteger x = ComputeModularExponentiation(a, d, number);
                if (x == 1 || x == number - 1)
                    continue;
                for (int j = 0; j < r - 1; j++)
                {
                    x = ComputeModularExponentiation(x, 2, number);
                    if (x == 1)
                        return false;
                    if (x == number - 1)
                        break;
                }
                if (x != number - 1)
                    return false;
            }
            return true;
        }
        static BigInteger ComputeModularExponentiation(BigInteger baseValue, BigInteger exponent, BigInteger modulus)
        {
            return BigInteger.ModPow(baseValue, exponent, modulus);
        }

        static BigInteger FindPrimitiveRoot(BigInteger prime)
        {
            BigInteger phi = prime - 1;
            var factors = PrimeFactors(phi);
            for (BigInteger root = 2; root < phi; root++)
            {
                bool isPrimitiveRoot = factors.All(factor => ComputeModularExponentiation(root, phi / factor, prime) != 1);
                if (isPrimitiveRoot)
                    return root;
            }
            throw new InvalidOperationException("Примітивний корінь не знайдено!");
        }

        static List<BigInteger> PrimeFactors(BigInteger number)
        {
            List<BigInteger> factors = new List<BigInteger>();
            BigInteger d = 2;
            while (d * d <= number)
            {
                while (number % d == 0)
                {
                    if (!factors.Contains(d))
                        factors.Add(d);
                    number /= d;
                }
                d += 1;
            }
            if (number > 1)
                factors.Add(number);
            return factors;
        }

        static BigInteger GenerateLargePrimeNumber(int bits)
        {
            BigInteger prime;
            do
            {
                prime = BigIntegerExtensions.GenerateRandomBigInteger(bits);
                prime |= BigInteger.One << (bits - 1) | BigInteger.One;
            } while (!IsPrimeNumber(prime));
            return prime;
        }

        static string BytesToString(IEnumerable<byte> bytes)
        {
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        static List<BigInteger> StringToNumbers(string str)
        {
            return Encoding.UTF8.GetBytes(str).Select(b => (BigInteger)b).ToList();
        }

        static List<(BigInteger, BigInteger)> EncryptString(BigInteger[] publicKey, string text)
        {
            var numbers = StringToNumbers(text);
            return numbers.Select(number => EncryptMessage(publicKey, number)).ToList();
        }

        static string DecryptString(BigInteger privateKey, List<(BigInteger, BigInteger)> encryptedData, BigInteger prime)
        {
            var decryptedNumbers = encryptedData.Select(data => DecryptMessage(privateKey, data, prime));
            return BytesToString(decryptedNumbers.Select(b => (byte)b));
        }

        static (BigInteger[], BigInteger) GenerateKeyPair(int bits)
        {
            BigInteger prime = GenerateLargePrimeNumber(bits);
            BigInteger primitiveRoot = FindPrimitiveRoot(prime);
            BigInteger privateKey = BigIntegerExtensions.GenerateRandomNumberInRange(2, prime - 1);
            BigInteger publicKey = ComputeModularExponentiation(primitiveRoot, privateKey, prime);
            return (new[] { prime, primitiveRoot, publicKey }, privateKey);
        }

        static (BigInteger, BigInteger) EncryptMessage(BigInteger[] publicKey, BigInteger message)
        {
            BigInteger prime = publicKey[0];
            BigInteger primitiveRoot = publicKey[1];
            BigInteger publicKeyValue = publicKey[2];
            BigInteger randomKey = BigIntegerExtensions.GenerateRandomNumberInRange(2, prime - 1);
            BigInteger a = ComputeModularExponentiation(primitiveRoot, randomKey, prime);
            BigInteger b = (message * ComputeModularExponentiation(publicKeyValue, randomKey, prime)) % prime;
            return (a, b);
        }

        static BigInteger DecryptMessage(BigInteger privateKey, (BigInteger, BigInteger) encryptedMessage, BigInteger prime)
        {
            BigInteger a = encryptedMessage.Item1;
            BigInteger b = encryptedMessage.Item2;
            BigInteger s = ComputeModularExponentiation(a, privateKey, prime);
            return (b * ComputeModularExponentiation(s, prime - 2, prime)) % prime;
        }

               
    }
}


