import { Component } from '@angular/core';
import { Repository } from '../models/repository';
import { Product } from '../models/product.model';
import { Cart } from '../models/cart.model';


@Component({
  selector: 'store-product-list',
  templateUrl: 'productList.component.html'
})
export class ProductListComponent {
  constructor(private repo: Repository, private cart: Cart) { }

  get products(): Product[] {
    if (this.repo.products != null && this.repo.products.length > 0) {
      const pageIndex = (this.repo.paginationObject.currentPage - 1)
        * this.repo.paginationObject.productsPerPage;
      return this.repo.products.slice(pageIndex,
        pageIndex + this.repo.paginationObject.productsPerPage);
    }
  }

  addToCart(product: Product) {
    this.cart.addProduct(product);
  }
}
