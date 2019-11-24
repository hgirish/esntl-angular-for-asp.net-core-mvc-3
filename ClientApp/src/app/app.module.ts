import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ModelModule } from './models/model.module';
import { StoreModule } from './store/store.module';
import { ExternalService } from './external.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ModelModule,
    FormsModule,
    StoreModule
  ],
  providers: [ExternalService],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(external: ExternalService) { }
}
