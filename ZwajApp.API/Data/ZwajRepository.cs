using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZwajApp.API.Helpers;
using ZwajApp.API.Models;

namespace ZwajApp.API.Data
{
    public class ZwajRepository : IZwajRepository
    {
        private readonly DataContext _context;
        public ZwajRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(l=>l.LikerId==userId&& l.LikeeId==recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser (int userId){
            return await _context.Photo.Where(u=>u.UserId==userId).FirstOrDefaultAsync(p=>p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photo.IgnoreQueryFilters().FirstOrDefaultAsync(p=>p.Id==id);
            return photo;
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            var query = _context.Users.Include(u=>u.Photo).AsQueryable();
            if(isCurrentUser) query= query.IgnoreQueryFilters();
            var user= await query.FirstOrDefaultAsync(u=> u.Id==id);
            return user;
        }
        public async Task<PageList<User>> GetUsers(UserParams userParams)
        {
            //var users = await _context.Users.Include(u=>u.Photo).ToListAsync();
            // return users;
            var users =  _context.Users.Include(u=>u.Photo).OrderByDescending(u=>u.LastActive).AsQueryable();
            users = users.Where(u=>u.Id != userParams.UserId);
            users = users.Where(u=>u.Gender == userParams.Gender);
            if(userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(u=>userLikers.Contains(u.Id));
            }
            if(userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(u=>userLikees.Contains(u.Id));
            }
            if(userParams.MinAge != 18 || userParams.MaxAge!= 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge-1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u=>u.DateOfBirth>= minDob && u.DateOfBirth<=maxDob);

            }
            if(!string.IsNullOrEmpty(userParams.OrderBy)){
                switch (userParams.OrderBy)
                {
                    case "created":
                    users= users.OrderByDescending(u=>u.Created);
                    break;
                    default:
                    users = users.OrderByDescending(u=>u.LastActive);
                    break;
                }
            }
            return await PageList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }
        private async Task<IEnumerable<int>> GetUserLikes(int id, bool Likers)
        {
            var user = await _context.Users.Include(u=>u.Likers).Include(u=>u.Likees).FirstOrDefaultAsync(u=>u.Id==id);
            if(Likers)
            {
                return user.Likers.Where(u=>u.LikeeId==id).Select(l=>l.LikerId);
            }else{
                return user.Likees.Where(u=>u.LikerId==id).Select(l=>l.LikeeId);
            }
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m=>m.Id==id);
        }

        public async Task<PageList<Message>> GetMessageForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(m=>m.Sender).ThenInclude(u=>u.Photo).Include(m=>m.Recipient).ThenInclude(u=>u.Photo).AsQueryable();
            switch(messageParams.MessageType)
            {
                case "Inbox":
                messages = messages.Where(m=>m.RecipientId==messageParams.UserId && m.RecipientDeleted == false);
                break;
                case "Outbox" :
                messages= messages.Where(m=>m.SenderId == messageParams.UserId && m.SenderDeleted == false);
                break;
                default:
                messages = messages.Where(m=>m.RecipientId == messageParams.UserId && m.RecipientDeleted == false && m.IsRead ==false);
                break;
            }
            messages = messages.OrderByDescending(m=>m.MessageSent);
            return await PageList<Message>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
        }
        public async Task<IEnumerable<Message>> GetConversation(int userId, int recipientId)
        {
            var messages = await _context.Messages.Include(m=>m.Sender).ThenInclude(u=>u.Photo).Include(m=>m.Recipient).ThenInclude(u=>u.Photo).Where(m=>m.RecipientId==userId && m.RecipientDeleted == false && m.SenderId == recipientId || m.RecipientId == recipientId && m.SenderDeleted == false && m.SenderId == userId)
            .OrderByDescending(m=>m.MessageSent).ToListAsync();
            return messages;
        }
        public async Task<int> GetUnreadMessagesForUser(int userId)
        {
            var messages = await _context.Messages.Where(m=>m.IsRead==false && m.RecipientId ==userId).ToListAsync();
            var count =messages.Count();
            return count;
        }

        public async Task<Payment> GetPaymentForUser(int userId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<ICollection<User>> GetLikersOrLikees(int userId, string type)
        {
             var users = _context.Users.Include(u=>u.Photo).OrderBy(u=>u.UserName).AsQueryable();
            if(type=="likers")
           {
               var userLikers = await GetUserLikes(userId,true);
               users =  users.Where(u=>userLikers.Contains(u.Id));
           }
           else if(type == "likees")
           {
               var userLikees = await GetUserLikes(userId,false);
               users =  users.Where(u=>userLikees.Contains(u.Id));
           }
           else{
               throw new Exception("لا توجد بيانات متاحة");
           }
           return users.ToList();            
        }

        public async Task<ICollection<User>> GetAllUsersExceptAdmin()
        {
            return await _context.Users.OrderBy(u=>u.NormalizedUserName).Where(u => u.NormalizedUserName!="ADMIN").ToListAsync();            
        }
    }
}