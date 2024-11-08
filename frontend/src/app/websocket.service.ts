import { Injectable } from '@angular/core';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { Mensagem } from './mensagem.model';
import { environment } from 'src/environments/environment';

@Injectable()
export class WebSocketService {
  private wsSubject: WebSocketSubject<Mensagem>;
  private baseUrl = environment.baseUrl;

  constructor() {
    this.wsSubject = webSocket(`ws://${this.baseUrl}/ws/v3`);
  }

  enviarMensagem(mensagem: Mensagem): void {
    this.wsSubject.next(mensagem);
  }

  receberMensagem() {
    return this.wsSubject.asObservable();
  }
}
