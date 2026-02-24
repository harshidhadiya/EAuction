using Microsoft.AspNetCore.Identity;

namespace MACUTION.Model.ActualObj
{
 public class User
 {
   PasswordHasher<object> hash;
    public User(PasswordHasher<object> _hash)
        {
            this.hash=_hash;
            this.Id=Guid.NewGuid().ToString("N");
        }
    public String Id{get;private set;}
    
    private String password;
    public String Name{get;  set;}
    public String role{get;set;}="USER";
    public async Task setGenerateAndSetPassword(String pass)
        {
        var result = hash.HashPassword(new object(),pass);
        password = result;
            
        }
        public String getPassword()
        {
            return password;
        }
 }   
}