import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HotelRoomsService } from '../api/hotel-rooms.service';

@Component({
  selector: 'app-hotel-rooms',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  template: `
    <div class="page-container">
      <!-- Header -->
      <div class="page-header">
        <div class="breadcrumb">
          <span class="breadcrumb-item active">Стаи</span>
          <span class="breadcrumb-separator">></span>
          <span class="breadcrumb-item">Добавяне</span>
        </div>
        <div class="user-menu">
          <div class="user-avatar">D</div>
        </div>
      </div>

      <!-- Content -->
      <div class="page-content">
        <h1 class="page-title">Добави стая</h1>

        <form [formGroup]="roomForm" (ngSubmit)="onSubmit()" class="form-container">
          <div class="form-section">
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Име <span class="required">*</span></label>
                <input type="text" formControlName="name" class="form-input">
              </div>
              <div class="form-group">
                <label class="form-label">Цена <span class="required">*</span></label>
                <div class="input-group">
                  <input type="number" formControlName="price" class="form-input" step="0.01" min="0">
                  <span class="input-suffix">лв.</span>
                </div>
              </div>
              <div class="form-group">
                <label class="form-label">Максимум възрастни</label>
                <input type="number" formControlName="maxAdults" class="form-input" min="1">
              </div>
              <div class="form-group">
                <label class="form-label">Максимум деца</label>
                <input type="number" formControlName="maxChildren" class="form-input" min="0">
              </div>
            </div>

            <div class="form-group">
              <label class="form-label">Описание</label>
              <textarea formControlName="description" class="form-textarea" rows="3" placeholder="Въведете описание на стаята..."></textarea>
            </div>
          </div>

          <div class="form-actions">
            <button type="submit" class="btn btn-primary" [disabled]="roomForm.invalid">
              Добави
            </button>
            <button type="button" class="btn btn-secondary">
              Добави и добави друг
            </button>
            <button type="button" class="btn btn-outline">
              Откажи
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: []
})
export class HotelRoomsComponent implements OnInit {
  roomForm: FormGroup;
  hotelRooms: any[] = [];
  loading = false;

  constructor(
    private hotelRoomsService: HotelRoomsService,
    private fb: FormBuilder
  ) {
    this.roomForm = this.fb.group({
      name: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      maxAdults: [2, [Validators.required, Validators.min(1)]],
      maxChildren: [0, [Validators.required, Validators.min(0)]],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.fetchHotelRooms();
  }

  fetchHotelRooms() {
    this.loading = true;
    this.hotelRoomsService.getAll({}).subscribe({
      next: (data) => { this.hotelRooms = data || []; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  onSubmit() {
    if (this.roomForm.valid) {
      console.log('Room form submitted:', this.roomForm.value);
    }
  }
}