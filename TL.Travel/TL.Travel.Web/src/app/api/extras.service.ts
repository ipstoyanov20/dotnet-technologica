import { Injectable, Injector } from '@angular/core';
import { CrudService } from '../core/services/crud.service';

@Injectable({ providedIn: 'root' })
export class ExtrasService extends CrudService<any> {
  constructor(injector: Injector) { super(injector); }

  override getResourceUrl(): string { return 'Extras'; }

  getAll() { return this.httpClient.get<any[]>(`${this.APIUrl}/GetAll`); }
  getById(id: number) { return this.httpClient.get<any>(`${this.APIUrl}/GetById`, { params: { id } }); }
  addEdit(model: any, id = 0) { return this.httpClient.post<any>(`${this.APIUrl}/AddEdit`, model, { params: { id } }); }
  override delete(id: number) { return this.httpClient.delete(`${this.APIUrl}/Delete/${id}`); }
}