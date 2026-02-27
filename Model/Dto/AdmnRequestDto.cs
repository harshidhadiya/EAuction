namespace MACUTION.Model.Dto
{
    public class signupRequestDto
    {
       
            required
            public string Name { get; set; }
            required
            public string Email { get; set; }
            required
            public int MobileNumber  { get; set; }
            required
             public string Address { get; set; }
            required
             public string Password { get; set; }
            public string ProfileImageUrl { get; set; } = "";
            public string role { get; set; } = "ADMIN";
    
    }
    public class loginRequestDto
    {
        public string email{get;set;}
        public string password {get;set;}
        public string role{get;set;}
    }
    public class loginResponceDto
    {
        public string token{get;set;}
        public string name{get;set;}
        public int id{get;set;}
        public string imageurl{get;set;}="";
        public bool request_accepted{get;set;}=false;
        public string email {get;set;}
        public bool  addUserAccess{get;set;}=false;
    }
   
    public class acceptResponceDto
    {
        public string status{get;set;}
        public string message{get;set;}
        public object data{get;set;}
    }
    public class restrictionResponceDto
    {
        public bool ?add_to_antoher{get;set;}=false;
        public bool ?delete_verification{get;set;}=false;
    }
}