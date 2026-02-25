namespace MACUTION.Data
{
    public enum Role { ADMIN = 0, USER = 1 }
    public class User
    {
        public int Id { get; set; }
        public String Password { get; private set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public int MobileNumber { get; set; }
        public String ProfileImageUrl { get; set; }
        public String role { get; set; }
        public String Address{get; set;}
        public DateTime createdAt { get; set; }
        
        public ICollection<Product> products{get;set;}
        public ICollection<Verifier> verifiers{get;set;}
        public void setPassword(string pass)
        {
            this.Password = pass;
        }
    }
}