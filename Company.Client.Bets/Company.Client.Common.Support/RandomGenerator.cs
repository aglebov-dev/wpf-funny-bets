using System;

namespace Company.Client.Common.Support
{
    public class RandomGenerator
    {
        public static int[] GenerateInteger(int minValue, int maxValue, int count = 1)
        {
            if (count < 1)
                throw new Exception("count");

            int randomNumber = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
            var random = new Random(Math.Abs(randomNumber));

            var result = new int[count];

            for (int i = 0; i < count; i++)
            {
                var value = random.Next(minValue, maxValue);
                result[i] = value;
            }

            return result;
        }
    }
}
