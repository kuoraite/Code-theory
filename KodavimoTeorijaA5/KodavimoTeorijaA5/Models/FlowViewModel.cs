using System.Drawing;

namespace KodavimoTeorijaA5.Models
{
    public class FlowViewModel
    {
        public int Step { get; set; }
        public int M { get; set; }
        public int K { get; set; }
        public double Pe { get; set; }

        public string InputType { get; set; }
        public string Vector { get; set; }
        public string Text { get; set; }
        public IFormFile Image { get; set; }

        public int PaddingBitsCount { get; set; }

        public string ConvertedVector { get; set; }
        public string ConvertedVectorThroughChannel { get; set; }
        public string ConvertedBackVector { get; set; }
        public string EncodedVector { get; set; }
        public string ChannelMessage { get; set; }
        public List<string> Changes { get; set; }
        public string DecodedMessage { get; set; }

        public string DecodedImage {get; set;}
    }
}
