import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PaymentTypesService } from '../api/payment-types.service';

@Component({
  selector: 'app-payment-types',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="page-container">
      <!-- Header -->
      <div class="page-header">
        <div class="breadcrumb">
          <span class="breadcrumb-item active">Начини На Плащане</span>
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
          <h1 class="page-title">Начини На Плащане</h1>
          <button class="btn btn-primary">
            Добави начин на плащане
          </button>
        </div>

        <div class="content-card">
          <!-- Search -->
          <div class="search-section">
            <input type="text" class="search-input" placeholder="Търсене...">
          </div>

          <!-- Table -->
          <div class="table-container" *ngIf="!loading; else loadingTemplate">
            <table class="data-table">
              <thead>
                <tr>
                  <th>Начин на плащане</th>
                  <th class="actions-column">Действия</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let paymentType of paymentTypes" class="table-row">
                  <td class="font-medium">{{paymentType.name}}</td>
                  <td>
                    <div class="action-buttons">
                      <button class="action-btn action-btn-edit">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                        </svg>
                      </button>
                      <button class="action-btn action-btn-delete">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <ng-template #loadingTemplate>
            <div class="loading-state">
              <div class="loading-spinner"></div>
              <span>Зареждане на начини на плащане...</span>
            </div>
          </ng-template>

          <!-- Pagination -->
          <div class="pagination">
            <div class="pagination-info">
              От 1 до 4 от 4 резултата
            </div>
            <div class="pagination-controls">
              <select class="pagination-select">
                <option value="10">10</option>
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
export class PaymentTypesComponent implements OnInit {
  paymentTypes: any[] = [];
  loading = false;

  constructor(private paymentTypesService: PaymentTypesService) {}

  ngOnInit(): void {
    this.fetchPaymentTypes();
  }

  fetchPaymentTypes() {
    this.loading = true;
    this.paymentTypesService.getAll().subscribe({
      next: (data) => { this.paymentTypes = data || []; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}