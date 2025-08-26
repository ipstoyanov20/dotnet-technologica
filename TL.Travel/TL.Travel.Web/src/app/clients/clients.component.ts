import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ClientsService } from '../api/clients.service';
import { ReservationsService } from '../api/reservations.service';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="page-container">
      <!-- Header -->
      <div class="page-header">
        <div class="breadcrumb">
          <span class="breadcrumb-item active">Клиенти</span>
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
          <h1 class="page-title">Клиенти</h1>
          <button class="btn btn-primary" (click)="addClient()">
            Добави клиент
          </button>
        </div>

        <div class="content-card">
          <!-- Search -->
          <div class="search-section">
            <input 
              type="text" 
              class="search-input" 
              placeholder="Търсене по име, имейл или телефон..."
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
                  <th>Имейл</th>
                  <th>Телефон</th>
                  <th class="actions-column">Действия</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let client of filteredClients" class="table-row">
                  <td class="font-medium">{{client.name}}</td>
                  <td>{{client.email}}</td>
                  <td>{{client.phone}}</td>
                  <td>
                    <div class="action-buttons">
                      <button class="action-btn action-btn-edit" (click)="editClient(client)">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                        </svg>
                      </button>
                      <button class="action-btn action-btn-delete" (click)="deleteClient(client)">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>

            <div *ngIf="filteredClients.length === 0" class="empty-state">
              <svg class="empty-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z"></path>
              </svg>
              <p class="empty-text">Няма намерени клиенти</p>
            </div>
          </div>

          <ng-template #loadingTemplate>
            <div class="loading-state">
              <div class="loading-spinner"></div>
              <span>Зареждане на клиенти...</span>
            </div>
          </ng-template>

          <!-- Pagination -->
          <div class="pagination">
            <div class="pagination-info">
              Показани {{filteredClients.length}} от {{clients.length}} резултата
            </div>
            <div class="pagination-controls">
              <select class="pagination-select" [(ngModel)]="pageSize" (change)="onPageSizeChange()">
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
export class ClientsComponent implements OnInit {
  reservations: any[] = [];
  filteredReservations: any[] = [];
  clients: any[] = [];
  filteredClients: any[] = [];
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

  onPageSizeChange() {
    // Implementation for pagination if needed
  }

  addClient() {
    console.log('Add client clicked');
  }

  editClient(client: any) {
    console.log('Edit client:', client);
  }

  deleteClient(client: any) {
    console.log('Delete client:', client);
  }
}