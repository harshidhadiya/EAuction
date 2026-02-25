namespace MACUTION.Data
{
    public class Product
    {
        public int Id{get;set;}
        public DateTime Buy_Date{get;set;}
        public int ?user_id{get;set;}
        
        public string product_name{get;set;}
        
        
        public DateTime creation_date{get;set;}
        public Verifier verifier{get;set;}
        public User user{get;set;} 
        public ICollection<Image> Images{get;set;}
    }
}