using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZwajApp.API.Data;
using ZwajApp.API.Helpers;
using CloudinaryDotNet;
using System.Threading.Tasks;
using ZwajApp.API.Dtos;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using ZwajApp.API.Models;
using System.Linq;

namespace ZwajApp.API.Controllers
{
    //[Authorize]
    [Route("api/users/{userId}/photo")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IZwajRepository _repo;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IMapper _mapper;
        private Cloudinary _cloudinary;
        public PhotoController(IZwajRepository repo, IOptions<CloudinarySettings> cloudinaryConfig, IMapper mapper)
        {
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            _repo = repo;
             Account acc = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey,_cloudinaryConfig.Value.ApiSecret) ;
             _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}",Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id){
            var photoFromRepository =await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepository);
            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreateDto photoForCreateDto){
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId, true);
            var file = photoForCreateDto.File;
            var uploadResult = new ImageUploadResult();
            if (file !=null && file.Length>0)
            {
                using(var stream = file.OpenReadStream()){
                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription(file.Name,stream),
                        Transformation = new Transformation()
                        .Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreateDto.Url = uploadResult.Uri.ToString();
            photoForCreateDto.publicId=uploadResult.PublicId;
            var photo =_mapper.Map<Photo>(photoForCreateDto);
            if(!userFromRepo.Photo.Any(p=>p.IsMain))
            photo.IsMain=true;
            userFromRepo.Photo.Add(photo);
            if(await _repo.SaveAll())
            {
                var PhotoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id=photo.Id},PhotoToReturn);
            }
            return BadRequest("خطأ في أضافة الصورة");
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id) {
           if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId, true);
            if(!userFromRepo.Photo.Any(p=>p.Id == id))
            return Unauthorized();
            var DesiredMainPhoto = await _repo.GetPhoto(id);
            if(DesiredMainPhoto.IsMain)
            return BadRequest("هذه الصورة الاساسية بالفعل ");
            var CurrentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            CurrentMainPhoto.IsMain = false;
            DesiredMainPhoto.IsMain = true;
            if(await _repo.SaveAll())
            return NoContent();
            return BadRequest("لايمكن تعديل الصورة الاساسية");
            
        }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId,int id)
    {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId, true);
            if(!userFromRepo.Photo.Any(p=>p.Id == id))
            return Unauthorized();
            var Photo = await _repo.GetPhoto(id);
            if(Photo.IsMain)
            return BadRequest("لا يمكن حذف الصورة الاساسية  ");
            if(Photo.PublicId != null){
                var deleteParams = new DeletionParams(Photo.PublicId);
                var result = this._cloudinary.Destroy(deleteParams);
                if(result.Result =="ok"){
                    _repo.Delete(Photo);
                }
            }
            if(Photo.PublicId == null){
                _repo.Delete(Photo);
            }
            if(await _repo.SaveAll())
                return Ok();
                return BadRequest("فشل حضف الصورة");            
    }
    }
}