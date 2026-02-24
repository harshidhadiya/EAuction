using MACUTION.Model.ActualObj;
using Microsoft.AspNetCore.SignalR;

namespace MACUTION.Model.Dto
{
    public class UserCreation
    {
        
        public String Name{get;set;}
        public String password {get;set;}
        public String role {get; set;}="USER";
    }
    public class changePasswordDto
    {
        public String password {get;   set;}
        public String ConfirmPassword{get;  set;}

    }
    public class changeProfileDto
    {
        public String Name{get; set;}
  
    }
    public class productDto
    {
        public String Name {get;set;}
        public String Description{get;set;}
        public String date{get;set;}
    }
    public class changeProductDto
    {
         public String ?NameOfProduct{get;  set;}
         public String ?Description {get; set;}
         public String ?BuyDate{get;  set;}
    }
    
}