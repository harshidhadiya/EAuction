namespace MACUTION.Data
{
    public class Verifier
    {
        public int Id{get; set;}
        public int verifier_id{get;set;}
        public int ?product_id{get;set;}
        public string ?description{get;set;}
        public DateTime verificationDate{get;set;}
        public bool isverified{get;set;}
        public User user{get;set;}
        public Product product{get;set;}
    }
}