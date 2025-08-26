import { Injectable, Injector } from '@angular/core';
import { CrudService } from '../core/services/crud.service';

@Injectable({ providedIn: 'root' })
export class HotelRoomsService extends CrudService<any> {
  constructor(injector: Injector) { super(injector); }

  override getResourceUrl(): string { return 'HotelRooms'; }

  getAll(request: any) { return this.httpClient.post<any[]>(`${this.APIUrl}/GetAll`, request); }
  getById(id: number) { return this.httpClient.get<any>(`${this.APIUrl}/GetById`, { params: { id } }); }
  addEdit(model: any, id = 0) { return this.httpClient.post<any>(`${this.APIUrl}/AddEdit`, model, { params: { id } }); }
  override delete(id: number) { return this.httpClient.delete(`${this.APIUrl}/Delete/${id}`); }
  getHotelRoomImagesList(id: number) { return this.httpClient.get<any[]>(`${this.APIUrl}/GetHotelRoomImagesList`, { params: { id } }); }
  downloadHotelRoomImage(imageId: number) { return this.httpClient.get(`${this.APIUrl}/DownloadHotelRoomImage`, { params: { imageId }, responseType: 'blob' }); }
  downloadHotelRoomPhotosPdf(id: number) { return this.httpClient.get(`${this.APIUrl}/DownloadHotelRoomPhotosPdf`, { params: { id }, responseType: 'blob' }); }
}