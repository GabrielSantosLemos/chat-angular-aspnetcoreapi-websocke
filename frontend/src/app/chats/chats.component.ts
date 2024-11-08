import { Component, OnInit } from '@angular/core';
import { AppStateService } from '../app-state.service';
import { WebSocketService } from '../websocket.service';
import { Mensagem } from '../mensagem.model';

@Component({
  selector: 'app-chats',
  templateUrl: './chats.component.html',
  styleUrls: ['./chats.component.scss'],
})
export class ChatsComponent implements OnInit {
  mensagens: Mensagem[] = [];
  mensagemTexto = '';

  constructor(private _webSocketService: WebSocketService) {}

  ngOnInit(): void {
    this._webSocketService.receberMensagem().subscribe((mensagem: Mensagem) => {
      mensagem.de = 'Servidor';
      this.mensagens.push(mensagem);
    });
  }

  enviarMensagem(): void {
    if (this.mensagemTexto.trim() === '') return;
    const mensagem = { texto: this.mensagemTexto, de: 'Você' };
    this.mensagens.push(mensagem);
    this._webSocketService.enviarMensagem(mensagem);
    this.mensagemTexto = '';
  }
}
