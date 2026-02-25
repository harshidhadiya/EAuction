namespace MACUTION.Data
{
    public class Image
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public String Image_URL { get; set; }
        public Product product { get; set; }
    }
}