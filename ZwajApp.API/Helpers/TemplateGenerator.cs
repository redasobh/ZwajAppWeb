using System.Text;
using AutoMapper;
using ZwajApp.API.Data;
using ZwajApp.API.Dtos;

namespace ZwajApp.API.Helpers
{
    public class TemplateGenerator
    {
        private readonly IMapper _mapper;
        private readonly IZwajRepository _repo;

        public TemplateGenerator(IZwajRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public string GetHTMLStringForUser(int userId)
        {
			//exception of Global Query Filter we use false
            var user = _repo.GetUser(userId, false).Result;
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);

            var likers = _repo.GetLikersOrLikees(userId, "likers").Result;
            var likees = _repo.GetLikersOrLikees(userId, "likees").Result;
            var likersCount=likers.Count;
            var likeesCount=likees.Count;

            var sb = new StringBuilder();
            sb.Append(@"
                        <html dir='rtl'>
                            <head>
                            </head>
                            <body>
                                <div class='page-header'><h2 class='header-container'>بطاقة " + userToReturn.KnownAs + @"</h2></div>                         
                                <div class='card-data'>
                                 <img src='" + userToReturn.PhotoURL + @"'>
                                <table style='display:inline;width: 50%;height: 300px;'>
                                <div>
                                <tr>
                                <td>الإسم</td>
                                    <td>" + userToReturn.KnownAs + @"</td>
                                </tr>
                                <tr>
                                    <td>العمر</td>
                                    <td>" + userToReturn.Age + @"</td>
                                </tr>    
                                <tr>
                                    <td>البلد</td>
                                    <td>" + userToReturn.Country + @"</td>
                                </tr>    
                                <tr>
                                    <td>تاريخ الإشتراك</td>
                                    <td>" + userToReturn.Created.ToShortDateString() + @"</td>
                                </tr> 
                                </div>   
                              </table>
                                </div>
                                <div class='page-header'><h2 class='header-container'>المعجبين &nbsp;&nbsp;["+likersCount+@"]</h2></div>
                                <table align='center'>
                                    <tr>
                                        <th>الإسم</th>
                                        <th>تاريخ الإشتراك</th>
                                        <th>العمر</th>
                                        <th>البلد</th>
                                    </tr>");

            foreach (var liker in likers)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>", liker.KnownAs, liker.Created.ToShortDateString(), liker.DateOfBirth.CalculateAge(), liker.Country);
            }
            sb.Append(@"
                                </table>
                                <div class='page-header'><h2 class='header-container'>المعجب بهم  &nbsp;&nbsp;["+likeesCount+@"] </h2></div>
                                <table align='center'>
                                <tr>
                                 <th>الإسم</th>
                                        <th>تاريخ الإشتراك</th>
                                        <th>العمر</th>
                                        <th>البلد</th>
                                </tr>");
            foreach (var likee in likees)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>", likee.KnownAs, likee.Created.ToShortDateString(), likee.DateOfBirth.CalculateAge(), likee.Country);
            }
            sb.Append(@"     </table>                   
                            </body>
                        </html>");

            return sb.ToString();
        }
		
    }
}