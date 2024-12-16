namespace KodavimoTeorijaA5.Models
{
    public class EncodingModel
    {
        // Generates a generator matrix of size (1, m)
        public static int[,] GenerateGeneratorMatrix(int m)
        {
            int n = (int)Math.Pow(2, m); //2^m
            int[,] generatorMatrix = new int[m + 1, n];

            for (int i = 0; i < n; i++)
            {
                generatorMatrix[0, i] = 1;
            }

            for (int i = 0; i < m; i++)
            {
                int step = (int)Math.Pow(2, i);
                for (int j = 0; j < n; j++)
                {
                    generatorMatrix[i + 1, j] = (j / step) % 2;
                }
            }

            Console.WriteLine("Generator Matrix:");
            PrintGeneratorMatrix(generatorMatrix);

            return generatorMatrix;
        }

        // Prints out generated generator matrix
        private static void PrintGeneratorMatrix(int[,] generatorMatrix)
        {
            int rows = generatorMatrix.GetLength(0); // Rows
            int cols = generatorMatrix.GetLength(1); // Columns

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(generatorMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        // Multiplies generator matrix with received vector thus encoding the vector
        public static int[] Encode(int[] vectorInput, int[,] generatorMatrix)
        {
            int m = generatorMatrix.GetLength(0); // Rows
            int n = generatorMatrix.GetLength(1); // Columns


            int[] encodedVector = new int[n];
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < m; i++)
                {
                    encodedVector[j] = encodedVector[j] ^ (vectorInput[i] * generatorMatrix[i, j]);
                }
            }

            return encodedVector;
        }
    }
}
