import { Component } from "@angular/core";
import { Repository } from '../models/repository';
import { Product } from '../models/product.model';


@Component({
  selector: 'store-product-list',
  templateUrl: 'productList.component.html'
})
export class ProductListComponent {
  constructor(private repo: Repository) { }

  get products(): Product[] {
    return this.repo.products;
  }
}