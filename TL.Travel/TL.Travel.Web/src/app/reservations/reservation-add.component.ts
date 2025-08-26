import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ReservationsService } from '../api/reservations.service';
import { ClientsService } from '../api/clients.service';
import { HotelsService } from '../api/hotels.service';
import { HotelRoomsService } from '../api/hotel-rooms.service';
import { FeedingTypesService } from '../api/feeding-types.service';
import { PaymentTypesService } from '../api/payment-types.service';
import { PaymentChannelsService } from '../api/payment-channels.service';

@Component({
  selector: 'app-reservation-add',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  template: `
    <div class="page-container">
      <!-- Header -->
      <div class="page-header">
        <div class="breadcrumb">
          <a routerLink="/reservations" class="breadcrumb-item">Резервации</a>
          <span class="breadcrumb-separator">></span>
          <span class="breadcrumb-item active">Добавяне</span>
        </div>
        <div class="user-menu">
          <div class="user-avatar">D</div>
        </div>
      </div>

      <!-- Content -->
      <div class="page-content">
        <h1 class="page-title">Добави резервация</h1>

        <form [formGroup]="reservationForm" (ngSubmit)="onSubmit()" class="form-container">
          <!-- Basic Information -->
          <div class="form-section">
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Дата на пристигане</label>
                <input type="date" formControlName="dateFrom" class="form-input">
              </div>
              <div class="form-group">
                <label class="form-label">Дата на заминаване</label>
                <input type="date" formControlName="dateTo" class="form-input">
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Клиент</label>
                <select formControlName="clientId" class="form-select">
                  <option value="">Моля, изберете клиент...</option>
                  <option *ngFor="let client of clients" [value]="client.id">{{client.name}}</option>
                </select>
              </div>
              <div class="form-group">
                <label class="form-label">Хотел</label>
                <select formControlName="hotelId" class="form-select">
                  <option value="">Моля, изберете хотел...</option>
                  <option *ngFor="let hotel of hotels" [value]="hotel.id">{{hotel.name}}</option>
                </select>
              </div>
            </div>

            <div class="form-group">
              <label class="form-label">Бележки</label>
              <textarea formControlName="notes" class="form-textarea" rows="3" placeholder="Въведете бележки..."></textarea>
            </div>
          </div>

          <!-- Accommodation Section -->
          <div class="form-section">
            <div class="section-header">
              <h3 class="section-title">Настаняване</h3>
            </div>

            <div formArrayName="accommodations">
              <div *ngFor="let accommodation of accommodations.controls; let i = index" [formGroupName]="i" class="accommodation-item">
                <div class="accommodation-header">
                  <span class="accommodation-title">Стая {{i + 1}}</span>
                  <button type="button" (click)="removeAccommodation(i)" class="remove-btn" *ngIf="accommodations.length > 1">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                    </svg>
                  </button>
                </div>

                <div class="form-row">
                  <div class="form-group">
                    <label class="form-label">Тип стая <span class="required">*</span></label>
                    <select formControlName="roomType" class="form-select">
                      <option value="">Изберете тип стая...</option>
                      <option *ngFor="let room of roomTypes" [value]="room.id">{{room.name}}</option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label class="form-label">Изхранване <span class="required">*</span></label>
                    <select formControlName="feeding" class="form-select">
                      <option value="">Изберете изхранване...</option>
                      <option *ngFor="let feeding of feedingTypes" [value]="feeding.id">{{feeding.name}}</option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label class="form-label">Брой стаи <span class="required">*</span></label>
                    <input type="number" formControlName="roomsCount" class="form-input" min="1">
                  </div>
                  <div class="form-group">
                    <label class="form-label">Сума <span class="required">*</span></label>
                    <div class="input-group">
                      <input type="number" formControlName="amount" class="form-input" step="0.01" min="0">
                      <span class="input-suffix">лв.</span>
                    </div>
                  </div>
                </div>

                <div class="form-row">
                  <div class="form-group">
                    <label class="form-label">Възрастни</label>
                    <input type="number" formControlName="adults" class="form-input" min="1">
                  </div>
                  <div class="form-group">
                    <label class="form-label">Деца</label>
                    <input type="number" formControlName="children" class="form-input" min="0">
                  </div>
                  <div class="form-group">
                    <label class="form-label">Бебета</label>
                    <input type="number" formControlName="babies" class="form-input" min="0">
                  </div>
                  <div class="form-group"></div>
                </div>
              </div>
            </div>

            <button type="button" (click)="addAccommodation()" class="add-btn">
              Добави настаняване
            </button>
          </div>

          <!-- Payments Section -->
          <div class="form-section">
            <div class="section-header">
              <h3 class="section-title">Плащания от клиент</h3>
            </div>

            <div formArrayName="payments">
              <div *ngFor="let payment of payments.controls; let i = index" [formGroupName]="i" class="payment-item">
                <div class="payment-header">
                  <span class="payment-title">Плащане {{i + 1}}</span>
                  <button type="button" (click)="removePayment(i)" class="remove-btn" *ngIf="payments.length > 1">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                    </svg>
                  </button>
                </div>

                <div class="form-row">
                  <div class="form-group">
                    <label class="form-label">Сума <span class="required">*</span></label>
                    <div class="input-group">
                      <input type="number" formControlName="amount" class="form-input" step="0.01" min="0">
                      <span class="input-suffix">лв.</span>
                    </div>
                  </div>
                  <div class="form-group">
                    <label class="form-label">Тип <span class="required">*</span></label>
                    <select formControlName="type" class="form-select">
                      <option value="Вноска">Вноска</option>
                      <option *ngFor="let type of paymentTypes" [value]="type.name">{{type.name}}</option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label class="form-label">Начин на плащане <span class="required">*</span></label>
                    <select formControlName="paymentMethod" class="form-select">
                      <option value="">Избери начин на плащане...</option>
                      <option *ngFor="let method of paymentChannels" [value]="method.id">{{method.name}}</option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label class="form-label">Дата на плащане</label>
                    <input type="date" formControlName="paymentDate" class="form-input">
                  </div>
                </div>

                <div class="form-group">
                  <label class="checkbox-label">
                    <input type="checkbox" formControlName="isPaid" class="form-checkbox">
                    <span class="checkbox-text">Сумата е платена</span>
                  </label>
                </div>
              </div>
            </div>

            <button type="button" (click)="addPayment()" class="add-btn">
              Добави плащане
            </button>
          </div>

          <!-- Action Buttons -->
          <div class="form-actions">
            <button type="submit" class="btn btn-primary" [disabled]="reservationForm.invalid">
              Добави
            </button>
            <button type="button" (click)="submitAndAddAnother()" class="btn btn-secondary" [disabled]="reservationForm.invalid">
              Добави и добави друг
            </button>
            <button type="button" routerLink="/reservations" class="btn btn-outline">
              Откажи
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: []
})
export class ReservationAddComponent implements OnInit {
  reservationForm: FormGroup;
  clients: any[] = [];
  hotels: any[] = [];
  roomTypes: any[] = [];
  feedingTypes: any[] = [];
  paymentTypes: any[] = [];
  paymentChannels: any[] = [];

  constructor(
    private fb: FormBuilder,
    private reservationsService: ReservationsService,
    private clientsService: ClientsService,
    private hotelsService: HotelsService,
    private hotelRoomsService: HotelRoomsService,
    private feedingTypesService: FeedingTypesService,
    private paymentTypesService: PaymentTypesService,
    private paymentChannelsService: PaymentChannelsService
  ) {
    this.reservationForm = this.fb.group({
      dateFrom: ['', Validators.required],
      dateTo: ['', Validators.required],
      clientId: ['', Validators.required],
      hotelId: ['', Validators.required],
      notes: [''],
      accommodations: this.fb.array([]),
      payments: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.addAccommodation();
    this.addPayment();
    this.loadDropdowns();
  }

  get accommodations() {
    return this.reservationForm.get('accommodations') as FormArray;
  }

  get payments() {
    return this.reservationForm.get('payments') as FormArray;
  }

  addAccommodation() {
    this.accommodations.push(this.fb.group({
      roomType: ['', Validators.required],
      feeding: ['', Validators.required],
      roomsCount: [1, [Validators.required, Validators.min(1)]],
      amount: ['', [Validators.required, Validators.min(0)]],
      adults: [2, [Validators.required, Validators.min(1)]],
      children: [0, [Validators.required, Validators.min(0)]],
      babies: [0, [Validators.required, Validators.min(0)]]
    }));
  }

  removeAccommodation(index: number) {
    this.accommodations.removeAt(index);
  }

  addPayment() {
    this.payments.push(this.fb.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      type: ['Вноска', Validators.required],
      paymentMethod: ['', Validators.required],
      paymentDate: [''],
      isPaid: [false]
    }));
  }

  removePayment(index: number) {
    this.payments.removeAt(index);
  }

  loadDropdowns() {
    this.clientsService.getAll().subscribe(data => this.clients = data || []);
    this.hotelsService.getAll().subscribe(data => this.hotels = data || []);
    this.hotelRoomsService.getAll({}).subscribe(data => this.roomTypes = data || []);
    this.feedingTypesService.getAll().subscribe(data => this.feedingTypes = data || []);
    this.paymentTypesService.getAll().subscribe(data => this.paymentTypes = data || []);
    this.paymentChannelsService.getAll().subscribe(data => this.paymentChannels = data || []);
  }

  onSubmit() {
    if (this.reservationForm.valid) {
      this.reservationsService.addEdit(this.reservationForm.value).subscribe({
        next: () => {
          // Navigate back to reservations list
        },
        error: (error) => {
          console.error('Error creating reservation:', error);
        }
      });
    }
  }

  submitAndAddAnother() {
    if (this.reservationForm.valid) {
      this.reservationsService.addEdit(this.reservationForm.value).subscribe({
        next: () => {
          this.resetForm();
        },
        error: (error) => {
          console.error('Error creating reservation:', error);
        }
      });
    }
  }

  private resetForm() {
    this.reservationForm.reset();
    this.accommodations.clear();
    this.payments.clear();
    this.addAccommodation();
    this.addPayment();
  }
}