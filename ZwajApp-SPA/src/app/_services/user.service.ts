import { PaginationResult } from './../_models/Pagination';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { map, tap } from 'rxjs/operators';
import { Message } from '../_models/message';

/*const httpOptions = {
  headers: new HttpHeaders(
    {
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    }
  )
};*/
@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl =  environment.apiUrl + 'users/';
constructor(private http: HttpClient) { }
getUsers(page?, itemsPerPage?, userParams?, likeParam?): Observable<PaginationResult<User[]>> {
  // return this.http.get<User[]>(this.baseUrl, httpOptions);
  const paginationResult: PaginationResult<User[]> = new PaginationResult<User[]>();
  let params = new HttpParams();
  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page) ;
    params = params.append('pageSize', itemsPerPage);
  }
  if (userParams != null) {
    params = params.append('minAge', userParams.minAge) ;
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
  }
  if (likeParam === 'Likers') {
    params = params.append('likers', 'true');
  }
  if (likeParam === 'Likees') {
    params = params.append('likees', 'true');
  }
 // return this.http.get<User[]>(this.baseUrl);
  return this.http.get<User[]>(this.baseUrl, {observe: 'response', params}).pipe(
    map(response => {
      paginationResult.result = response.body;
      if (response.headers.get('Pagination') != null) {
        paginationResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginationResult;
    })
  );
}
getUser(id): Observable<User> {
  // return this.http.get<User>(this.baseUrl + id, httpOptions);
  return this.http.get<User>(this.baseUrl + id);
}
updateUser(id: number , user: User) {
  return this.http.put(this.baseUrl + id, user);
}
SetMainPhoto(userId: number, id: number) {
  return this.http.post(this.baseUrl + userId + '/photo/' + id + '/setMain', {});
}
deletePhoto(userId: number, id: number) {
  return this.http.delete(this.baseUrl + userId + '/photo/' + id);
}
sengLike(id: number, recipientId: number) {
  return this.http.post(this.baseUrl + id + '/like/' + recipientId, {});
}
getMessages(id: number, page?, itemsPerPage?, messageType?) {
  const paginationResult: PaginationResult<Message[]> = new PaginationResult<Message[]>();
  let params = new HttpParams();
  params = params.append('MessageType', messageType);
  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page) ;
    params = params.append('pageSize', itemsPerPage);
  }
  return this.http.get<Message[]>(this.baseUrl + id + '/messages', {observe: 'response', params}).pipe(
    map(response => {
      paginationResult.result = response.body;
      if (response.headers.get('Pagination') != null) {
        paginationResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginationResult;
    })
  );
}
getConversation(id: number, recipientId: number) {
return this.http.get<Message[]>(this.baseUrl + id + '/messages/chat/' + recipientId);
}
sendMessage(id: number, message: Message) {
return this.http.post(this.baseUrl + id + '/messages', message);
}
getUnreadCount(userId) {
  return this.http.get(this.baseUrl + userId + '/messages/count');
}
markAsRead(userId: number, messageId: number) {
  return this.http.post(this.baseUrl + userId + '/messages/read/' + messageId, {}).subscribe();
}
deleteMessage(id: number, userId: number) {
  return this.http.post(this.baseUrl + userId + '/messages/' + id, {});
}
charge(userId: number, stripeToken: string) {
return this.http.post(this.baseUrl + userId + '/charge/' + stripeToken, {});
}
getPaymentForUser(userId: number) {
  return this.http.get(this.baseUrl + userId + '/payment');
}
GetReportForUser(id: number): any  {
  return this.http.get(this.baseUrl + 'UserReport/' + id, { headers:
    new HttpHeaders({'Content-Type': 'application/pdf'}), responseType: 'blob'}).pipe (
  tap (
      () => console.log('تم إستلام الملف بنجاح'),
      error => console.log(error)
   )
 );
}
getAllUsers() {
  return this.http.get(this.baseUrl + 'GetAllUsersExceptAdmin');
}
}
