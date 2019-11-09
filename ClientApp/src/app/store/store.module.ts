import { NgModule, NO_ERRORS_SCHEMA } from "@angular/core";
import { CartSummaryComponent } from './cartSummary.component';
import { CategoryFilterComponent } from './categoryFilter.component';
import { ProductListComponent } from './productList.component';
import { RatingsComponent } from './ratings.component';
import { ProductSelectionComponent } from './productSelection.component';
import { PaginationComponent } from './pagination.component';
import { BrowserModule } from '@angular/platform-browser';
import { CartDetailComponent } from './cartDetail.component';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CheckoutDetailsComponent } from './checkout/checkoutDetail.component';
import { CheckoutPaymentComponent } from './checkout/checkoutPayment.component';
import { CheckoutSummaryComponent } from './checkout/checkoutSummary.component';
import { OrderConfirmationComponent } from './checkout/orderConfirmation.component';
import { BlazorLoader } from './blazorLoader.component';

@NgModule({
  declarations: [CartSummaryComponent,
    CategoryFilterComponent,
    PaginationComponent,
    ProductListComponent,
    RatingsComponent,
    ProductSelectionComponent,
    CartDetailComponent,
    CheckoutDetailsComponent,
    CheckoutPaymentComponent,
    CheckoutSummaryComponent,
    OrderConfirmationComponent,
    BlazorLoader
  ],
  imports: [BrowserModule, FormsModule, RouterModule],
  exports: [ProductSelectionComponent],
  schemas: [NO_ERRORS_SCHEMA]
})
export class StoreModule { }
