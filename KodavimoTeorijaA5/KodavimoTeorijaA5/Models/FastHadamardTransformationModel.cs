using System;

namespace KodavimoTeorijaA5.Models
{
    public class FastHadamardTransformationModel
    {
        public static int[,] baseHadamard = new int[,] { { 1, 1 }, { 1, -1 } };

        // Decodes received bit vector using Fast Hadamard Transformation
        public static int[] Decode(int[] receivedVector, int m)
        {
            int[] w = ReplaceZeroWithMinusOne(receivedVector);
            int[] transformedVector = ComputeTransform(w, m);

            int maxValPosition = 0;
            int maxValue = Math.Abs(transformedVector[0]);
            for (int i = 1; i < transformedVector.Length; i++)
            {
                int absValue = Math.Abs(transformedVector[i]);
                if (absValue > maxValue)
                {
                    maxValue = absValue;
                    maxValPosition = i;
                }
            }

            int[] binaryRepresentation = ConvertToBinary(maxValPosition, m);
            int firstBit = (transformedVector[maxValPosition] > 0) ? 1 : 0;

            int[] decodedMessage = new int[m + 1];
            decodedMessage[0] = firstBit;
            Array.Copy(binaryRepresentation, 0, decodedMessage, 1, m);

            return decodedMessage;
        }

        // Replace zeros with -1 because this is required for Hadamard Transformation
        private static int[] ReplaceZeroWithMinusOne(int[] vector)
        {
            int[] result = new int[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                result[i] = (vector[i] == 0) ? -1 : 1;
            }
            return result;
        }

        private static int[] ComputeTransform(int[] vector, int m)
        {
            int n = vector.Length;
            int[] result = new int[n];
            Array.Copy(vector, result, n);

            for (int i = 1; i <= m; i++)
            {
                int size1 = (int)Math.Pow(2, m - i); // 2^(m-i)
                int size2 = (int)Math.Pow(2, i - 1); // 2^(i-1)
                int[,] identity1 = IdentityMatrix(size1);
                int[,] identity2 = IdentityMatrix(size2);


                // Compute H_m = I_{2^(m-i)} X H X I_{2^(i-1)}
                int[,] hMatrix = KroneckerProduct(identity1, baseHadamard);
                hMatrix = KroneckerProduct(hMatrix, identity2);

                // Apply the matrix multiplication
                result = MultiplyMatrixVector(hMatrix, result);
            }

            return result;
        }

        // Make an identity matrix (all zeroes but diagonally - ones)
        private static int[,] IdentityMatrix(int size)
        {
            int[,] matrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                matrix[i, i] = 1;
            }
            return matrix;
        }

        private static int[,] KroneckerProduct(int[,] a, int[,] b)
        {
            int aRows = a.GetLength(0), aCols = a.GetLength(1);
            int bRows = b.GetLength(0), bCols = b.GetLength(1);

            int[,] result = new int[aRows * bRows, aCols * bCols];

            for (int i = 0; i < aRows; i++)
            {
                for (int j = 0; j < aCols; j++)
                {
                    for (int k = 0; k < bRows; k++)
                    {
                        for (int l = 0; l < bCols; l++)
                        {
                            result[i * bRows + k, j * bCols + l] = a[i, j] * b[k, l];
                        }
                    }
                }
            }

            return result;
        }

        private static int[] MultiplyMatrixVector(int[,] matrix, int[] vector)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (cols != vector.Length)
                throw new ArgumentException("Matrix columns must match vector size.");

            int[] result = new int[rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
            }
            return result;
        }

        private static int[] ConvertToBinary(int value, int length)
        {
            string binaryString = Convert.ToString(value, 2);
            binaryString = binaryString.PadLeft(length, '0');
            binaryString = new string(binaryString.Reverse().ToArray());
            int[] binary = binaryString.Select(c => c - '0').ToArray();

            return binary;
        }

    }
}