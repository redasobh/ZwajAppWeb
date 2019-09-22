using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZwajApp.API.Data;
using ZwajApp.API.Dtos;
using ZwajApp.API.Models;

namespace ZwajApp.API.Controllers {
    [AllowAnonymous]
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
      //  private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        // IAuthRepository repo
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthController (IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager) {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
            //_repo = repo;
        }

        [HttpPost ("register")]
        public async Task<IActionResult> Register (UserForRegisterDto userForRegisterDto) {
            //validation
            //if(!ModelState.IsValid)return BadRequest(ModelState);
            // userForRegisterDto.Username = userForRegisterDto.Username.ToLower ();
            //if (await _repo.UserExists (userForRegisterDto.Username))
            //  return BadRequest ("هذا المستخدم مسجل من قبل");
            // var userToCreate = new User()
            // {
            //  Username = userForRegisterDto.Username
            // };
            var userToCreate = _mapper.Map<User> (userForRegisterDto);
            //  var CreatedUser = await _repo.Register (userToCreate, userForRegisterDto.Password);
            //return StatusCode(201);
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserForDetailsDto> (userToCreate);
            if(result.Succeeded) {
            return CreatedAtRoute ("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost ("login")]
        public async Task<IActionResult> Login (UserForLoginDto userForLoginDto) {
            //throw new Expception("Api Sys Nooo!")
           // var userFromRepo = await _repo.Login (userForLoginDto.username.ToLower (), userForLoginDto.password);
           // if (userFromRepo == null) return Unauthorized ();
            /*  var claims = new[]{
                  new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                  new Claim(ClaimTypes.Name,userFromRepo.UserName)
              };
              var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
              var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
              var tokedDescripror = new SecurityTokenDescriptor
              {
                  Subject = new ClaimsIdentity(claims),
                  Expires = DateTime.Now.AddDays(1),
                  SigningCredentials = creds
              };
              var tokenHandler = new JwtSecurityTokenHandler();
              var token = tokenHandler.CreateToken(tokedDescripror);
              */
              var user = await _userManager.FindByNameAsync(userForLoginDto.UserName);
              var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);
              if(result.Succeeded)
              {
                var appUser = await _userManager.Users.Include(p=>p.Photo).FirstOrDefaultAsync(u=>u.NormalizedUserName == userForLoginDto.UserName.ToUpper());
                var userToReturn = _mapper.Map<UserForListDto> (appUser);
                 return Ok (new {
                token = GenerateJwtToken (appUser).Result,
                user = userToReturn
            });
            }
            return Unauthorized();           
        }
        private async Task<string> GenerateJwtToken (User user) {
            var claims = new List<Claim> {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Name, user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config.GetSection ("AppSettings:Token").Value));
            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512);
            var tokedDescripror = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (claims),
                Expires = DateTime.Now.AddDays (1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler ();
            var token = tokenHandler.CreateToken (tokedDescripror);
            return tokenHandler.WriteToken (token);
        }
    }
}