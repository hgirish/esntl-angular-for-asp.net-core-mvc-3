import { NgModule } from "@angular/core";
import { CartSummaryComponent } from './cartSummary.component';
import { CategoryFilterComponent } from './categoryFilter.component';
import { ProductListComponent } from './productList.component';
import { RatingsComponent } from './ratings.component';
import { ProductSelectionComponent } from './productSelection.component';
import { PaginationComponent } from './pagination.component';
import { BrowserModule } from '@angular/platform-browser';

@NgModule({
  declarations: [CartSummaryComponent,
    CategoryFilterComponent,
    PaginationComponent,
    ProductListComponent,
    RatingsComponent,
    ProductSelectionComponent
  ],
  imports: [BrowserModule],
  exports: [ProductSelectionComponent]
})
export class StoreModule { }
