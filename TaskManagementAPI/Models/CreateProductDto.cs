namespace TaskManagementAPI.Models
{
    public class CreateProductDto
    {
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? Stock { get; set; }
    }
}
