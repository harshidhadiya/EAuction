using System.Diagnostics;

namespace MACUTION.Data
{
    public enum Role { ADMIN = 0, USER = 1 }
    public class User
    {
        public int Id { get; set; }
        public String Password { get;  set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public int MobileNumber { get; set; }
        public String ProfileImageUrl { get; set; }
        public String role { get; set; }
        public String Address{get; set;}
        public DateTime createdAt { get; set; }
        public bool right_to_add { get; set; }

        public ICollection<Product> ?products{get;set;}=new List<Product>();
        public ICollection<Verifier> ?verifiers{get;set;}=new List<Verifier>();
        public Request_admin ?request{get;set;}
        public ICollection<Request_admin> ?given_access{get;set;}=new List<Request_admin>();
        public void setPassword(string pass)
        {
            this.Password = pass;
        }
    }
}