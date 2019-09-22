using System.Threading.Tasks;
using ZwajApp.API.Models;
using System.Collections.Generic;
using ZwajApp.API.Helpers;

namespace ZwajApp.API.Data
{
    public interface IZwajRepository
    {
        void Add<T>(T entity) where T:class;
        void Delete<T>(T entity) where T:class;
        Task<bool> SaveAll();
       // Task<IEnumerable<User>> GetUsers();
        Task<PageList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id, bool isCurrentUser);
        Task<Photo> GetPhoto (int id);
        Task<Photo> GetMainPhotoForUser (int userId);
        Task<Like> GetLike(int userId,int recipientId);
        Task<Message> GetMessage(int id);
        Task<PageList<Message>> GetMessageForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetConversation(int userId, int recipientId);
        Task<int> GetUnreadMessagesForUser(int userId);
        Task<Payment> GetPaymentForUser(int userId);
        Task<ICollection<User>> GetLikersOrLikees(int userId, string type);
        Task<ICollection<User>> GetAllUsersExceptAdmin();
    }
}
