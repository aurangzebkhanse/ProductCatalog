using System.Text.Json.Serialization;

namespace WebApp.Models
{
    public class ProductDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("stock")]
        public int Stock { get; set; }
    }
}
