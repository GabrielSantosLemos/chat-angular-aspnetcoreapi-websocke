import { Component } from '@angular/core';
import { AppStateService } from '../app-state.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  get usuario() {
    return this._state.usuario;
  }
  set usuario(value: string) {
    this._state.usuario = value;
  }

  constructor(
    private _state: AppStateService,
    private _route: Router
  ) {}

  login() {
    this._route.navigate(['/chats']);
  }
}
