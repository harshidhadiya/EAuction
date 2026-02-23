using MACUTION.Model.ActualObj;

namespace MACUTION.Model.Dto
{
    public class UserCreation
    {
        
        public string Name{get;set;}
        public string password {get;set;}
        public string role {get; set;}="USER";
    }
}