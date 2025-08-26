import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ReservationsService } from '../api/reservations.service';

@Component({
  selector: 'app-reservations',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <!-- Header -->
      <div class="page-header">
        <div class="breadcrumb">
          <span class="breadcrumb-item active">Резервации</span>
          <span class="breadcrumb-separator">></span>
          <span class="breadcrumb-item">Списък</span>
        </div>
        <div class="user-menu">
          <div class="user-avatar">D</div>
        </div>
      </div>

      <!-- Content -->
      <div class="page-content">
        <div class="content-header">
          <h1 class="page-title">Резервации</h1>
          <a routerLink="/reservations/add" class="btn btn-primary">
            Добави резервация
          </a>
        </div>

        <div class="content-card">
          <!-- Search -->
          <div class="search-section">
            <input 
              type="text" 
              class="search-input" 
              placeholder="Търсене по клиент, хотел..."
              [(ngModel)]="searchTerm"
              (input)="onSearch()"
            >
          </div>

          <!-- Table -->
          <div class="table-container" *ngIf="!loading; else loadingTemplate">
            <table class="data-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Клиент</th>
                  <th>Хотел</th>
                  <th>Дата пристигане</th>
                  <th>Дата заминаване</th>
                  <th>Статус</th>
                  <th class="actions-column">Действия</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let reservation of filteredReservations" class="table-row">
                  <td class="font-medium">{{reservation.id}}</td>
                  <td>{{reservation.clientName}}</td>
                  <td>{{reservation.hotelName}}</td>
                  <td>{{reservation.dateFrom | date:'dd.MM.yyyy'}}</td>
                  <td>{{reservation.dateTo | date:'dd.MM.yyyy'}}</td>
                  <td>
                    <span class="status-badge" [ngClass]="getStatusClass(reservation.status)">
                      {{reservation.status}}
                    </span>
                  </td>
                  <td>
                    <div class="action-buttons">
                      <button class="action-btn action-btn-edit" (click)="editReservation(reservation)">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                        </svg>
                      </button>
                      <button class="action-btn action-btn-delete" (click)="deleteReservation(reservation)">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>

            <div *ngIf="filteredReservations.length === 0" class="empty-state">
              <svg class="empty-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2-2v16a2 2 0 002 2z"></path>
              </svg>
              <p class="empty-text">Няма намерени резервации</p>
            </div>
          </div>

          <ng-template #loadingTemplate>
            <div class="loading-state">
              <div class="loading-spinner"></div>
              <span>Зареждане на резервации...</span>
            </div>
          </ng-template>

          <!-- Pagination -->
          <div class="pagination">
            <div class="pagination-info">
              Показани {{filteredReservations.length}} от {{reservations.length}} резултата
            </div>
            <div class="pagination-controls">
              <select class="pagination-select" [(ngModel)]="pageSize">
                <option value="10">10</option>
                <option value="25">25</option>
                <option value="50">50</option>
              </select>
              <span class="pagination-label">на страница</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class ReservationsComponent implements OnInit {
  reservations: any[] = [];
  filteredReservations: any[] = [];
  loading = false;
  searchTerm = '';
  pageSize = 10;

  constructor(private reservationsService: ReservationsService) {}

  ngOnInit(): void {
    this.fetchReservations();
  }

  fetchReservations() {
    this.loading = true;
    this.reservationsService.getAll().subscribe({
      next: (data) => { 
        this.reservations = data || []; 
        this.filteredReservations = [...this.reservations];
        this.loading = false; 
      },
      error: (error) => { 
        console.error('Error fetching reservations:', error);
        this.reservations = [];
        this.filteredReservations = [];
        this.loading = false; 
      }
    });
  }

  onSearch() {
    if (!this.searchTerm.trim()) {
      this.filteredReservations = [...this.reservations];
      return;
    }

    const term = this.searchTerm.toLowerCase();
    this.filteredReservations = this.reservations.filter(reservation =>
      reservation.clientName?.toLowerCase().includes(term) ||
      reservation.hotelName?.toLowerCase().includes(term)
    );
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'confirmed': return 'status-confirmed';
      case 'pending': return 'status-pending';
      case 'cancelled': return 'status-cancelled';
      default: return 'status-default';
    }
  }

  editReservation(reservation: any) {
    console.log('Edit reservation:', reservation);
  }

  deleteReservation(reservation: any) {
    console.log('Delete reservation:', reservation);
  }
}