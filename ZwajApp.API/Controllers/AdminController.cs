using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZwajApp.API.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ZwajApp.API.Models;
using ZwajApp.API.Dtos;
using Microsoft.Extensions.Options;
using ZwajApp.API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ZwajApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public AdminController(DataContext context, UserManager<User> userManager, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _userManager = userManager;
            _context = context;
             Account acc = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey,_cloudinaryConfig.Value.ApiSecret) ;
             _cloudinary = new Cloudinary(acc);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await (from user in _context.Users
                                  orderby user.UserName
                                  select new
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      Roles = (from userRole in user.UserRoles
                                               join role in _context.Roles
       on userRole.RoleId equals role.Id
                                               select role.Name).ToList()
                                  }).ToListAsync();
            return Ok(userList);
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _context.Photo
                .Include(u => u.User)
                .IgnoreQueryFilters()
                .Where(p => p.IsApproved == false)
                .Select(u => new
                {
                    Id = u.Id,
                    UserName = u.User.UserName,
                    KnownAs = u.User.KnownAs,
                    Url = u.Url,
                    IsApproved = u.IsApproved
                }).ToListAsync();

            return Ok(photos);
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _context.Photo.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == photoId);
            photo.IsApproved = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhoto(int photoId)
        {
            var photo = await _context.Photo.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == photoId);
            if (photo.IsMain)
                return BadRequest("لا يمكنك رفض الصورة الأساسية");
            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    _context.Photo.Remove(photo);
                }
            }
            if (photo.PublicId == null)
            {
                _context.Photo.Remove(photo);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    	
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editroles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] { };
            // selectedRoles = selectedRoles != null? selectedRoles: new string[] {}
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            // Member ==> Member + Moderator
            if (!result.Succeeded) return BadRequest("حدث خطأ أثناء إضافة الأدوار");
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("حدث خطأ أثناء حذف الأدوار");
            return Ok(await _userManager.GetRolesAsync(user));
        }
    }

}