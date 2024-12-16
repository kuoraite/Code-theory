namespace KodavimoTeorijaA5.Models
{
    public class SimulatingChannelModel
    {
        private static readonly Random random = new();

        // Channel simulation
        public static (int[] receivedVector, List<(int position, int originalValue, int changedValue)>) 
            SimulateChannel(int[] encodedVector, double pe, int paddingBits = 0)
        {
            int[] receivedVector = new int[encodedVector.Length];
            var changes = new List<(int position, int originalValue, int changedValue)>();

            int usefulBits = encodedVector.Length - paddingBits;
            int scaledProbability = (int)(pe * 10000);

            for (int i = 0; i < encodedVector.Length; i++)
            {
                if(i >= usefulBits)
                {
                    receivedVector[i] = encodedVector[i];
                }
                else
                {
                    int randomValue = random.Next(1, 10001);
                    //Console.WriteLine("rand: " + randomValue + " pe " + scaledProbability);

                    if (randomValue < scaledProbability)
                    {
                        int originalValue = encodedVector[i];
                        int changedValue = (originalValue == 0) ? 1 : 0;

                        receivedVector[i] = changedValue;

                        changes.Add((i, originalValue, changedValue));
                    }
                    else
                    {
                        receivedVector[i] = encodedVector[i];
                    }
                }
            }

            return (receivedVector, changes);
        }
    }
}
