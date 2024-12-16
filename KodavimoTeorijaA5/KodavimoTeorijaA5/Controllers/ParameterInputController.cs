using KodavimoTeorijaA5.Models;
using Microsoft.AspNetCore.Mvc;
using static KodavimoTeorijaA5.Models.ConversionModel;

namespace KodavimoTeorijaA5.Controllers
{
    public class ParameterInputController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new FlowViewModel { Step = 1 };
            return View(model);
        }

        [HttpPost]
        public ActionResult Step1(FlowViewModel model)
        {
            model.K = CalculateK(model.M);
            model.Step = 2;
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Step2(FlowViewModel model)
        {
            var (vectors, convertedBitArray, paddingBits) = ProcessInput(model);
            model.PaddingBitsCount = paddingBits;

            int[,] generatorMatrix = EncodingModel.GenerateGeneratorMatrix(model.M);
            var encodedMessages = vectors.Select(vector => EncodingModel.Encode(vector, generatorMatrix)).ToList();

            ProcessChannel(model, convertedBitArray, encodedMessages);

            model.Step = 3;
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Step3(FlowViewModel model)
        {
            int vectorSize = (int)Math.Pow(2, model.M);

            int[] receivedVector = model.ChannelMessage
                .ToCharArray()
                .Select(c => int.Parse(c.ToString()))
                .ToArray();

            var decodedChunks = DecodeReceivedVectors(receivedVector, vectorSize, model.M);
            var fullDecodedMessage = decodedChunks.SelectMany(chunk => chunk).ToArray();

            int usefulBits = fullDecodedMessage.Length - model.PaddingBitsCount;
            int[] finalMessage = fullDecodedMessage.Take(usefulBits).ToArray();

            ProcessDecodedData(model, finalMessage);
            model.Step = 4;

            return View("Index", model);
        }

        // From model input, check what type the input is and splits to bit vectors accordingly
        private static (List<int[]> Vectors, char[] ConvertedBits, int PaddingBits) ProcessInput(FlowViewModel model)
        {
            var vectors = new List<int[]>();
            char[] convertedBits = [];
            int paddingBits = 0;

            switch (model.InputType)
            {
                // Stores vector received as string like an int array as only list element
                case "Vector":
                    vectors.Add(model.Vector.ToCharArray()
                        .Select(c => int.Parse(c.ToString()))
                        .ToArray());
                    break;

                // Calls method to split received vector to int array vectors of determined size k (m + 1)
                case "Text":
                    (convertedBits, vectors, paddingBits) = TextToBinaryVectors(model.Text, model.K);
                    break;

                // Calls method which converts image as byte array to vectors of determined size k
                case "Image" when model.Image != null:
                    using (var ms = new MemoryStream())
                    {
                        model.Image.CopyTo(ms);
                        byte[] imageBytes = ms.ToArray();

                        char[] binaryData = [];
                        (binaryData, paddingBits) = ConvertImageToBinaryVectors(imageBytes, model.K);
                        vectors = CreateVectors(binaryData, model.K);
                    }
                    break;
            }

            return (vectors, convertedBits, paddingBits);
        }

        private static List<int[]> CreateVectors(char[] binaryData, int vectorSize)
        {
            int vectorCount = binaryData.Length / vectorSize;
            var vectors = new List<int[]>();

            for (int i = 0; i < vectorCount; i++)
            {
                var vector = binaryData
                    .Skip(i * vectorSize)
                    .Take(vectorSize)
                    .Select(c => c == '1' ? 1 : 0)
                    .ToArray();
                vectors.Add(vector);
            }

            return vectors;
        }

        private static void ProcessChannel(FlowViewModel model, char[] convertedBitArray, List<int[]> encodedMessages)
        {
            var encodedData = encodedMessages.SelectMany(msg => msg).ToArray();

            // Simulate encoded data through channel
            var (receivedVector, changes) = SimulatingChannelModel.SimulateChannel(
                encodedData, model.Pe, model.PaddingBitsCount);

            // Simulate original input through channel
            var (receivedOriginal, _) = SimulatingChannelModel.SimulateChannel(
                convertedBitArray.Select(c => int.Parse(c.ToString())).ToArray(),
                model.Pe, model.PaddingBitsCount);

            model.ConvertedVector = string.Join("", convertedBitArray);
            model.EncodedVector = string.Join("", encodedData);
            model.ChannelMessage = string.Join("", receivedVector);
            model.ConvertedVectorThroughChannel = string.Join("", receivedOriginal);

            model.Changes = changes
                .Select(change => $"Position {change.position}: {change.originalValue} -> {change.changedValue}")
                .ToList();
        }

        private static List<int[]> DecodeReceivedVectors(int[] receivedVector, int vectorSize, int m)
        {
            return Enumerable.Range(0, receivedVector.Length / vectorSize)
                .Select(i => receivedVector.Skip(i * vectorSize).Take(vectorSize).ToArray())
                .Select(chunk => FastHadamardTransformationModel.Decode(chunk, m))
                .ToList();
        }

        private static void ProcessDecodedData(FlowViewModel model, int[] finalMessage)
        {
            switch (model.InputType)
            {
                case "Text":
                    model.DecodedMessage = BinaryToText(finalMessage);
                    model.ConvertedBackVector = BinaryToText(model.ConvertedVectorThroughChannel
                        .ToCharArray()
                        .Select(c => int.Parse(c.ToString()))
                        .ToArray());
                    break;

                case "Image":
                    int imageWidth = (int)Math.Sqrt(finalMessage.Length);
                    int imageHeight = imageWidth;

                    byte[] imageBytes = BinaryToBmpByteArray(
                        string.Join("", finalMessage), imageWidth, imageHeight);

                    model.DecodedImage = Convert.ToBase64String(imageBytes);
                    break;

                default:
                    model.DecodedMessage = string.Join("", finalMessage);
                    break;
            }
        }

        private static int CalculateK(int m) => m + 1;
    }
}
