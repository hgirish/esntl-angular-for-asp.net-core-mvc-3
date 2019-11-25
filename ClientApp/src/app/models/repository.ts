import { Product } from './product.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Filter, Pagination } from './configClasses.repository';
import { Supplier } from './supplier.model';
import { Observable } from 'rxjs';
import { Order } from './order.model';
import { OrderConfirmation } from './OrderConfirmation';
import { ProductsMetadata } from './productsMetadata';

const productUrl = '/api/products';
const supplierUrl = '/api/suppliers';
const sessionUrl = '/api/session';
const ordersUrl = '/api/orders';

@Injectable()
export class Repository {
  product: Product;
  products: Product[];
  suppliers: Supplier[] = [];
  categories: string[] = [];
  filter: Filter = new Filter();
  paginationObject = new Pagination();
  orders: Order[] = [];


  constructor(
    private http: HttpClient
  ) {
    this.filter.related = true;

  }

  getProduct(id: number) {
    this.http.get<Product>(`${productUrl}/${id}`)
      .subscribe(p => {
        this.product = p;
      });
  }
  getProducts(): Promise<ProductsMetadata> {
    let url = `${productUrl}?related=${this.filter.related}`;
    if (this.filter.category) {
      url += `&category=${this.filter.category}`;
    }
    if (this.filter.search) {
      url += `&search=${this.filter.search}`;
    }
    url += '&metadata=true';

    return this.http.get<ProductsMetadata>(url)
      .toPromise<ProductsMetadata>()
      .then(md => {
        this.products = md.data;
        this.categories = md.categories;
        return md;
      });
  }

  getSuppliers() {
    this.http.get<Supplier[]>(supplierUrl)
      .subscribe(sups => this.suppliers = sups);
  }

  createProduct(prod: Product) {
    const data = {
      name: prod.name,
      category: prod.category,
      description: prod.description,
      price: prod.price,
      supplier: prod.supplier ? prod.supplier.supplierId : 0
    };
    this.http.post<number>(productUrl, data)
      .subscribe(id => {
        prod.productId = id;
        this.products.push(prod);
      });
  }

  createProductAndSupplier(prod: Product, supp: Supplier) {
    const data = {
      name: supp.name, city: supp.city, state: supp.state
    };
    this.http.post<number>(supplierUrl, data)
      .subscribe(id => {
        supp.supplierId = id;
        prod.supplier = supp;
        this.suppliers.push(supp);
        if (prod != null) {
          this.createProduct(prod);
        }
      });
  }

  replaceProduct(prod: Product) {
    const data = {
      name: prod.name,
      category: prod.category,
      description: prod.description,
      price: prod.price,
      supplier: prod.supplier ? prod.supplier.supplierId : 0
    };
    this.http.put(`${productUrl}/${prod.productId}`, data)
      .subscribe(() => this.getProducts());
  }

  replaceSupplier(supp: Supplier) {
    const data = {
      name: supp.name,
      city: supp.city,
      state: supp.state
    };

    this.http.put(`${supplierUrl}/${supp.supplierId}`, data)
      .subscribe(() => this.getProducts());
  }

  updateProduct(id: number, changes: Map<string, any>) {
    const patch = [];
    changes.forEach((value, key) =>
      patch.push({ op: 'replace', path: key, value }));
    this.http.patch(`${productUrl}/${id}`, patch)
      .subscribe(() => this.getProducts());
  }

  deleteProduct(id: number) {
    this.http.delete(`${productUrl}/${id}`)
      .subscribe(() => this.getProducts());
  }

  deleteSupplier(id: number) {
    this.http.delete(`${supplierUrl}/${id}`)
      .subscribe(() => {
        this.getProducts();
        this.getSuppliers();
      });
  }

  storeSessionData<T>(dataType: string, data: T) {
    return this.http.post(`${sessionUrl}/${dataType}`, data)
      .subscribe(response => { });
  }

  getSesionData<T>(dataType: string): Observable<T> {
    return this.http.get<T>(`${sessionUrl}/${dataType}`);
  }

  getOrders() {
    this.http.get<Order[]>(ordersUrl)
      .subscribe(data => this.orders = data);
  }

  createOrder(order: Order) {
    this.http.post<OrderConfirmation>(ordersUrl, {
      name: order.name,
      address: order.address,
      payment: order.payment,
      products: order.products
    }).subscribe(data => {
      order.orderConfirmation = data;
      order.cart.clear();
      order.clear();
    });
  }

  shipOrder(order: Order) {
    this.http.post(`${ordersUrl}/${order.orderId}`, {})
      .subscribe(() => this.getOrders());
  }

  login(name: string, password: string): Observable<boolean> {
    return this.http.post<boolean>("/api/account/login", { name: name, password: password });
  }

  logout() {
    this.http.post("/api/account/logout", null).subscribe(response => { });
  }


}
