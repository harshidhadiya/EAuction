using Microsoft.AspNetCore.Identity;

namespace MACUTION
{
    public class HashPassword
    {
        PasswordHasher<object> hash;
        public HashPassword()
        {
            hash=new PasswordHasher<object>();
        }


        
    }
}