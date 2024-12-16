using System.Drawing;
using System.Text;

namespace KodavimoTeorijaA5.Models
{
    public static class ConversionModel
    {
        // Converts received sentence to array of bit vectors and if needed padding for last vector, adds it
        public static (char[], List<int[]>, int) TextToBinaryVectors(string text, int vectorSize)
        {
            string binaryString = TextToBinary(text);
            char[] binaryArray = binaryString.ToCharArray();

            int paddingBitsCount = 0;

            List<int[]> bitSequences = new List<int[]>();

            for (int i = 0; i < binaryArray.Length; i += vectorSize)
            {
                var chunk = binaryArray.Skip(i).Take(vectorSize).ToArray();
                if (chunk.Length < vectorSize)
                {
                    paddingBitsCount += vectorSize - chunk.Length;

                    char[] paddedChunk = new char[vectorSize];
                    Array.Copy(chunk, paddedChunk, chunk.Length);

                    chunk = paddedChunk;
                    Console.WriteLine("PADDED " + string.Join("", paddedChunk));
                }

                int[] chunkAsIntArr = chunk.Select(c => c == '1' ? 1 : 0).ToArray();
                bitSequences.Add(chunkAsIntArr);
            }

            return (binaryArray, bitSequences, paddingBitsCount);
        }

        // Converts binary text to a vector
        public static string TextToBinary(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text); ;
            StringBuilder binaryStringBuilder = new();

            foreach (byte b in bytes)
            {
                binaryStringBuilder.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }

            return binaryStringBuilder.ToString();
        }

        // Converting binary vector into a sentence
        public static string BinaryToText(int[] binaryMessage)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < binaryMessage.Length; i += 8)
            {
                int asciiValue = Convert.ToInt32(string.Join("", binaryMessage.Skip(i).Take(8)), 2);
                sb.Append((char)asciiValue);
            }

            return sb.ToString();
        }

        // Converting image to a binary vector
        public static (char[], int) ConvertImageToBinaryVectors(byte[] imageBytes, int vectorSize)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            using (Bitmap bmp = new Bitmap(ms))
            {
                int width = bmp.Width;
                int height = bmp.Height;
                int paddingBitsCount = 0;

                // Convert image to grayscale and then to binary
                char[] binaryData = new char[width * height];
                int index = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixelColor = bmp.GetPixel(x, y);
                        byte pixelValue = (byte)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);

                        // Convert pixel to binary (1 if pixel is light enough, 0 if dark)
                        binaryData[index++] = pixelValue > 127 ? '1' : '0';
                    }
                }

                int remainder = binaryData.Length % vectorSize;
                if (remainder != 0)
                {
                    paddingBitsCount = vectorSize - remainder;
                    Array.Resize(ref binaryData, binaryData.Length + paddingBitsCount);
                    for (int i = binaryData.Length - paddingBitsCount; i < binaryData.Length; i++)
                    {
                        binaryData[i] = '0';
                    }
                }

                return (binaryData, paddingBitsCount);
            }
        }

        // Convert binary vector back to byte array representing the image
        public static byte[] BinaryToBmpByteArray(string binaryData, int width, int height)
        {
            if (binaryData.Length != width * height)
            {
                throw new ArgumentException("The length of binary data does not match the image dimensions.");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bmp = new(width, height))
                {
                    int index = 0;

                    // Traverse through each pixel and assign the appropriate color (0 or 255)
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int newPixelValue = (binaryData[index] == '1') ? 255 : 0; // '1' -> White, '0' -> Black
                            Color newPixelColor = Color.FromArgb(newPixelValue, newPixelValue, newPixelValue);
                            bmp.SetPixel(x, y, newPixelColor);

                            index++;
                        }
                    }

                    // Save the image to memory stream as a BMP image
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    return ms.ToArray();
                }
            }
        }
    }
}