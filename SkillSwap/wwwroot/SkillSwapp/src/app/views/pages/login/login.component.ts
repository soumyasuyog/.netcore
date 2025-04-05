import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { API_BASE_URL } from 'api.config';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  apiData: any;
  token: string | null = null;
  constructor(private http: HttpClient, private router: Router) { }


  getApiData(username: string, password: string) {
    const params = new HttpParams()
      .set('username', username)
      .set('password', password)
      .set('grant_type', 'password');

      this.http.post(`${API_BASE_URL}Authenticate/token`, params.toString(), {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
      }
    }).subscribe((response: any) => {
      this.apiData = response;
      this.token = response.access_token;
      console.log(this.apiData, this.token);
      this.router.navigate(['/dashboard']);
    }, (error) => {
      console.error(error);
    });
  }
}
