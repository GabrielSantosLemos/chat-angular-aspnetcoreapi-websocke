import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { WebSocketService } from './websocket.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppStateService } from './app-state.service';
import { LoginComponent } from './login/login.component';
import { ChatsComponent } from './chats/chats.component';

@NgModule({
  declarations: [AppComponent, LoginComponent, ChatsComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  providers: [WebSocketService, AppStateService],
  bootstrap: [AppComponent],
})
export class AppModule {}
