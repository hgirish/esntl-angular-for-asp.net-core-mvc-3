import { NgModule } from "@angular/core";
import { CategoryFilterComponent } from './categoryFilter.component';
import { ProductDetailComponent } from './productDetail.component';
import { ProductTableComponent } from './productTable.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';


@NgModule({
  declarations: [
    CategoryFilterComponent,
    ProductDetailComponent,
    ProductTableComponent
  ],
  imports: [CommonModule, RouterModule]
})
export class DummyModule { }
