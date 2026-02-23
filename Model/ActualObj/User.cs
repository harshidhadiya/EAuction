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
    public string Id{get;private set;}
    
    private string password;
    public string Name{get;  set;}
    public string role{get;set;}="USER";
    public async Task setGenerateAndSetPassword(string pass)
        {
        var result = hash.HashPassword(new object(),pass);
        password = result;
            
        }
 }   
}