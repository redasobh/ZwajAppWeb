using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZwajApp.API.Data;
using ZwajApp.API.Dtos;
using ZwajApp.API.Helpers;
using ZwajApp.API.Models;

namespace ZwajApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    //[Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IZwajRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IZwajRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper =mapper;
        }
        [HttpGet("{id}", Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId,int id)
        {
            if(userId!= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);
            if(messageFromRepo ==null)
            return NotFound();
            return Ok(messageFromRepo);            
        }
        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if(userId!= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            messageParams.UserId=userId;
            var MessagesFromRepo = await _repo.GetMessageForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(MessagesFromRepo);
            Response.AddPagination(MessagesFromRepo.CurrentPage,MessagesFromRepo.PageSize, MessagesFromRepo.TotalCount,MessagesFromRepo.TotalPages);
            return Ok(messages);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId,MessageForCreationDto messageForCreationDto)
        {
            var sender = await _repo.GetUser(userId, true);
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            messageForCreationDto.SenderId = userId;
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId, false);
            if(recipient==null) return BadRequest("لم يتم الوصول للمرسل اليه");
            var message = _mapper.Map<Message>(messageForCreationDto);
            _repo.Add(message);
            if(await _repo.SaveAll()) 
            {
            var messageToReturn = _mapper.Map<MessageForCreationDto>(message);
            return CreatedAtRoute("GetMessage", new {id=message.Id}, messageToReturn);
            } 
            throw new Exception("حدثت مشكلة أثناء حفظ الرسالة الجديدة");
        }
        [HttpGet("chat/{recipientId}")]
        public async Task<IActionResult> GetConversation(int userId, int recipientId)
        {
            if(userId!= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var messagesFromRepo = await _repo.GetConversation(userId,recipientId);
            var messageToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            return Ok(messageToReturn);            
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetUnreadMessagesForUser(int userId)
        {
            if(userId!= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var count = await _repo.GetUnreadMessagesForUser(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            return Ok(count);
        }
        [HttpPost("read/{id}")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var message = await _repo.GetMessage(id);
            if(message.RecipientId !=userId) return Unauthorized();
            message.IsRead = true;
            message.DateRead = DateTime.Now;
            await _repo.SaveAll();
            return NoContent();
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id , int userId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var message = await _repo.GetMessage(id);
            if(message.SenderId == userId)
            message.SenderDeleted = true;
            if(message.RecipientId == userId)
            message.RecipientDeleted = true;
            if(message.SenderDeleted && message.RecipientDeleted)
            _repo.Delete(message);
            if(await _repo.SaveAll()) return NoContent();
            throw new Exception("حدث خطأ اثناء حذف الرسالة");
        }
    }
}