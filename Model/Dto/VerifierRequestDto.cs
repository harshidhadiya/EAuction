
namespace MACUTION.Model.Dto
{
    public class supportClass
    {
        public int id{get;set;}
        public string Name {get;set;}
        public string email {get;set;}
        public string Address{get;set;}
        public string ?imageurl{get;set;}
        
        public supportClass(int id,string name,string email,string address,string imageurl="")
        {
            this.id=id;
            this.Name=name;
            this.email=email;
            this.Address=address;
            this.imageurl=imageurl;
        }
    }
    public class allUnverifiedProduct_responce
    {
        public int Id{get;set;}
        public string productname{get;set;}
        public DateTime buydate{get;set;}
        public supportClass ?ownerDetails{get;set;}
    }

    public class VerifyProductRequestDto
    {
        public string? description { get; set; }
    }

    public class VerifiedByYouProductResponse
    {
        public int Id { get; set; }
        public string productname { get; set; } = "";
        public DateTime buydate { get; set; }
        public supportClass? ownerDetails { get; set; }
        public DateTime verificationDate { get; set; }
        public string? verificationDescription { get; set; }
    }
}