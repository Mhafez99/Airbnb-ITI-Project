import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BehaviorSubject } from 'rxjs';
import { lastValueFrom } from 'rxjs';
import { Order } from '../models/order.model';
import { User } from '../models/user.model';
import { HttpService } from './http.service';
import { OrderService } from './order.service';
import { StayService } from './stay.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(
    private httpService: HttpService,
    private stayService: StayService,
    public orderService: OrderService,
    public snackBar: MatSnackBar
  ) {
    const user = this.getUser();
    this._user$.next(user);
  }

  private USER_URL = 'user/';
  private STORAGE_KEY_LOGGEDIN_USER = 'user';
  private AUTH_URL = 'auth/';

  private _user$ = new BehaviorSubject<User | null>(null);
  public user$ = this._user$.asObservable();

  hostFunction = this.updateHostMsg.bind(this);
  userFunction = this.updateUserMsg.bind(this);

  public getUser(): User {
    return JSON.parse(
      sessionStorage.getItem(this.STORAGE_KEY_LOGGEDIN_USER) as string
    );
  }

  public async login(credentials: User) {
    try {
      const res = (await lastValueFrom(
        this.httpService.post(this.AUTH_URL + 'login', credentials)
      )) as any;

      if (res?.token) {
        sessionStorage.setItem('token', res.token);
      }

      if (res?.user) {
        this.saveLocalUser(res.user);
        this._user$.next(res.user);
      }
    } catch (err) {
      throw err;
    }
  }

  public async signup(user: User) {
    try {
      const res = (await lastValueFrom(
        this.httpService.post(this.AUTH_URL + 'signup', user)
      )) as any;

      if (res?.token) {
        sessionStorage.setItem('token', res.token);
      }

      if (res?.user) {
        this.saveLocalUser(res.user);
        this._user$.next(res.user);
      }
    } catch (err) {
      throw err;
    }
  }

  public getEmptyUser() {
    return {
      username: '',
      fullname: '',
      password: '',
      imgUrl: '',
      userMsg: 0,
      hostMsg: 0,
    };
  }

  async update(user: User) {
    try {
      if (!user) return;
      const updatedUser = (await lastValueFrom(
        this.httpService.put(this.USER_URL, user)
      )) as User;
      this.saveLocalUser(updatedUser);
      this._user$.next(updatedUser);
      return updatedUser;
    } catch (err) {
      throw err;
    }
  }

  async updateHostMsg(order: Order) {
    try {
      const user = this.getUser();
      user.hostMsg++;
      await this.update(user);
      const msg = `${order.buyer.fullname} invite your place`;
      this.snackBar.open(msg, 'Close', { duration: 3000 });
      this.orderService.loadOrders();
    } catch (err) {
      console.log('err:', err);
    }
  }

  async updateUserMsg(order: Order) {
    try {
      const user = this.getUser();
      user.userMsg++;
      await this.update(user);
      const msg = `${order.stay.name} update your vacation status`;
      this.snackBar.open(msg, 'Close', { duration: 3000 });
      this.orderService.loadOrders();
    } catch (err) {
      console.log('err:', err);
    }
  }

  public async logout() {
    try {
      await lastValueFrom(this.httpService.post(this.AUTH_URL + 'logout'));
      sessionStorage.clear();
      this._user$.next(null);
      window.location.assign('/');
    } catch (err) {
      console.log('err:', err);
    }
  }

  private saveLocalUser(user: User) {
    sessionStorage.setItem(
      this.STORAGE_KEY_LOGGEDIN_USER,
      JSON.stringify(user)
    );
    return user;
  }
}
