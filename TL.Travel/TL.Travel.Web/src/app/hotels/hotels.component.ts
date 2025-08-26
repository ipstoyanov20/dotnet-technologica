import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HotelsService } from '../api/hotels.service';

@Component({
  selector: 'app-hotels',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <!-- Header -->
      <div class="page-header">
        <div class="breadcrumb">
          <span class="breadcrumb-item active">Хотели</span>
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
          <h1 class="page-title">Хотели</h1>
          <button class="btn btn-primary" (click)="addHotel()">
            Добави хотел
          </button>
        </div>

        <div class="content-card">
          <!-- Search -->
          <div class="search-section">
            <input 
              type="text" 
              class="search-input" 
              placeholder="Търсене по име или локация..."
              [(ngModel)]="searchTerm"
              (input)="onSearch()"
            >
          </div>

          <!-- Table -->
          <div class="table-container" *ngIf="!loading; else loadingTemplate">
            <table class="data-table">
              <thead>
                <tr>
                  <th>Име</th>
                  <th>Локация</th>
                  <th>Звезди</th>
                  <th>Описание</th>
                  <th class="actions-column">Действия</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let hotel of filteredHotels" class="table-row">
                  <td class="font-medium">{{hotel.name}}</td>
                  <td>{{hotel.location}}</td>
                  <td>
                    <div class="flex items-center">
                      <span class="text-yellow-500">★</span>
                      <span class="ml-1">{{hotel.stars}}</span>
                    </div>
                  </td>
                  <td class="max-w-xs truncate">{{hotel.description}}</td>
                  <td>
                    <div class="action-buttons">
                      <button class="action-btn action-btn-edit" (click)="editHotel(hotel)">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                        </svg>
                      </button>
                      <button class="action-btn action-btn-delete" (click)="deleteHotel(hotel)">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>

            <div *ngIf="filteredHotels.length === 0" class="empty-state">
              <svg class="empty-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
              </svg>
              <p class="empty-text">Няма намерени хотели</p>
            </div>
          </div>

          <ng-template #loadingTemplate>
            <div class="loading-state">
              <div class="loading-spinner"></div>
              <span>Зареждане на хотели...</span>
            </div>
          </ng-template>

          <!-- Pagination -->
          <div class="pagination">
            <div class="pagination-info">
              Показани {{filteredHotels.length}} от {{hotels.length}} резултата
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
export class HotelsComponent implements OnInit {
  hotels: any[] = [];
  filteredHotels: any[] = [];
  loading = false;
  searchTerm = '';
  pageSize = 10;

  constructor(private hotelsService: HotelsService) {}

  ngOnInit(): void {
    this.fetchHotels();
  }

  fetchHotels() {
    this.loading = true;
    this.hotelsService.getAll().subscribe({
      next: (data) => { 
        this.hotels = data || []; 
        this.filteredHotels = [...this.hotels];
        this.loading = false; 
      },
      error: (error) => { 
        console.error('Error fetching hotels:', error);
        this.hotels = [];
        this.filteredHotels = [];
        this.loading = false; 
      }
    });
  }

  onSearch() {
    if (!this.searchTerm.trim()) {
      this.filteredHotels = [...this.hotels];
      return;
    }

    const term = this.searchTerm.toLowerCase();
    this.filteredHotels = this.hotels.filter(hotel =>
      hotel.name?.toLowerCase().includes(term) ||
      hotel.location?.toLowerCase().includes(term)
    );
  }

  addHotel() {
    console.log('Add hotel clicked');
  }

  editHotel(hotel: any) {
    console.log('Edit hotel:', hotel);
  }

  deleteHotel(hotel: any) {
    console.log('Delete hotel:', hotel);
  }
}