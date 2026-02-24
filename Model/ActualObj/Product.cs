namespace MACUTION.Model
{
     public class Verification
    {
        public bool isVerified{get; private set;}=false;
        public String adminId{get;private set;}="";
        public String Description{get;private set;}="";
        
    }
    public class Product
    {
         public String NameOfProduct{get; private set;}
         public String Description {get;private set;}
         public String BuyDate{get; private set;}
         public String UserId{get;private set;}
         public String ProductId{get; private set;}
         public Verification verified;
         public Product()
        {
            
        }
         public Product(string name,String date,String userid,String Description="")
        {
            this.NameOfProduct=name;
            this.Description=Description;
            this.BuyDate=date;
            this.UserId=userid;
            this.ProductId=Guid.NewGuid().ToString("N");
            verified=new Verification();
        }
        public void changeName(String name)
        {
            this.NameOfProduct=name;
        }
        public void changeDescription(String desc)
        {
            this.Description=desc;
        }
        public void changeDate(string date)
        {
            this.BuyDate=date;
        }
    }
}