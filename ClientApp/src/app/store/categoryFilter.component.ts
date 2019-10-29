import { Component } from "@angular/core";
import { Repository } from '../models/repository';

@Component({
  selector: 'store-category-filter',
  templateUrl: 'categoryFilter.component.html'
})
export class CategoryFilterComponent {


  constructor(private repo: Repository) { }


}