using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ZwajApp.API.Models
{
    public class User : IdentityUser<int>
    {
        // public int Id { get; set; }
        // public string Username { get; set; }
        // public byte[] PasswordHash { get; set; }
        // public byte[] PasswordSalt { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created {get;set;}
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookFor { get; set; }
        public string  Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photo { get; set; }
        public ICollection<Like> Likers { get; set; }
        public ICollection<Like> Likees { get; set; }
        public ICollection<Message> MessageSent { get; set; }
        public ICollection<Message> MessageReceived { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}