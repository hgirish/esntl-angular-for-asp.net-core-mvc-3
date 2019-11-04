import { NgModule } from "@angular/core";
import { HttpClientModule } from '@angular/common/http'
import { Repository } from './repository';
import { NavigationService } from './navigation.service';

@NgModule({
  imports: [HttpClientModule],
  providers: [Repository, NavigationService]
})
export class ModelModule {

}
