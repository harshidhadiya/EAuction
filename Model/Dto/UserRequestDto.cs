using System.ComponentModel.DataAnnotations;
using MACUTION.Model.ActualObj;
using Microsoft.AspNetCore.SignalR;

namespace MACUTION.Model.Dto
{
    // This DTO is used when a new user signs up.
    // It matches the important fields from MACUTION.Data.User.
    public class UserCreation
    {
        required
        public string Name { get; set; }
        required
        public string Email { get; set; }
        required
        public int MobileNumber { get; set; }
        required
         public string Address { get; set; }
        required
         public string Password   { get; set; }
        public string ProfileImageUrl { get; set; } = "";
        public string role { get; set; } = "USER";
    }

    // This DTO is used only for login.
    public class UserLoginDto
    {
        required
        public string Email
        { get; set; }
        required public string Password { get; set; }
        public string Role { get; set; } = "USER";
    }

    public class changePasswordDto
    {
        required
        public string password
        { get; set; }
        required
        public string ConfirmPassword
        { get; set; }

    }
    public class changeProfileDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? MobileNumber { get; set; }
        public string? Address { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
    public class productDto
    {
        required
        public string Name
        { get; set; }
        public string? Description { get; set; }
        required
        public string date
        { get; set; }
    }
    public class changeProductDto
    {
        public string? NameOfProduct { get; set; }
        public string? Description { get; set; }
        public string? BuyDate { get; set; }
    }

}