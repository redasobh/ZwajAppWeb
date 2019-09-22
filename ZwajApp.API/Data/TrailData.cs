using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ZwajApp.API.Models;

namespace ZwajApp.API.Data {
    public class TrailData {
        // private readonly DataContext _context;
        // DataContext context
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public TrailData (UserManager<User> userManager, RoleManager<Role> roleManager) {
            _roleManager = roleManager;
            _userManager = userManager;
            //  _context = context;

        }
        public void TrailUsers () {
            if (!_userManager.Users.Any ()) {
                var userDate = System.IO.File.ReadAllText ("Data/UserTrialData.json");
                var users = JsonConvert.DeserializeObject<List<User>> (userDate);
                var roles = new List<Role> {
                    new Role{Name="Admin"},
                    new Role{Name="Moderator"},
                    new Role{Name="Member"},
                    new Role{Name="VIP"},
                };
                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }
                foreach (var user in users) {
                     user.Photo.ToList().ForEach(p=>p.IsApproved=true);
                    _userManager.CreateAsync (user, "password").Wait ();
                    _userManager.AddToRoleAsync(user,"Member").Wait();
                    // byte[] passwordHash, passwordSalt;
                    //CreatePasswordHash ("password", out passwordHash, out passwordSalt);
                    // user.PasswordHash=passwordHash;
                    //user.PasswordSalt=passwordSalt;
                    // user.UserName = user.UserName.ToLower ();
                    // _context.Add (user);
                }
                //  _context.SaveChanges ();
                // var adminUser = new User{ UserName= "Admin" };
                // IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;
                var admin = _userManager.FindByNameAsync("Admin").Result;
                _userManager.AddToRolesAsync(admin, new []{"Admin","Moderator"}).Wait();

            }
        }
        // private void CreatePasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt) {
        //   using (var hmac = new System.Security.Cryptography.HMACSHA512 ()) {
        //   passwordSalt = hmac.Key;
        //   passwordHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
        // }
        // }
    }
}