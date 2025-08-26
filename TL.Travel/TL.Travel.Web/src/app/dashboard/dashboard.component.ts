import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="page-container">
      <div class="page-header">
        <div class="breadcrumb">
          <span class="breadcrumb-item">Табло</span>
        </div>
        <div class="user-menu">
          <div class="user-avatar">D</div>
        </div>
      </div>
      
      <div class="page-content">
        <h1 class="page-title">Добре дошли в TL Travel</h1>
        
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon bg-blue-100 text-blue-600">
              <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" class="w-6 h-6">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2-2v16a2 2 0 002 2z"></path>
              </svg>
            </div>
            <div class="stat-content">
              <div class="stat-number">24</div>
              <div class="stat-label">Активни резервации</div>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon bg-green-100 text-green-600">
              <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" class="w-6 h-6">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z"></path>
              </svg>
            </div>
            <div class="stat-content">
              <div class="stat-number">156</div>
              <div class="stat-label">Клиенти</div>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon bg-purple-100 text-purple-600">
              <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" class="w-6 h-6">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
              </svg>
            </div>
            <div class="stat-content">
              <div class="stat-number">42</div>
              <div class="stat-label">Хотели</div>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon bg-orange-100 text-orange-600">
              <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" class="w-6 h-6">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"></path>
              </svg>
            </div>
            <div class="stat-content">
              <div class="stat-number">8,450</div>
              <div class="stat-label">Общ оборот (лв.)</div>
            </div>
          </div>
        </div>
        
        <div class="quick-actions">
          <h2 class="section-title">Бързи действия</h2>
          <div class="action-grid">
            <a routerLink="/reservations/add" class="action-card">
              <div class="action-icon">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" class="w-6 h-6">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                </svg>
              </div>
              <div class="action-content">
                <div class="action-title">Нова резервация</div>
                <div class="action-description">Създайте нова резервация</div>
              </div>
            </a>
            
            <a routerLink="/clients/add" class="action-card">
              <div class="action-icon">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" class="w-6 h-6">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"></path>
                </svg>
              </div>
              <div class="action-content">
                <div class="action-title">Нов клиент</div>
                <div class="action-description">Добавете нов клиент</div>
              </div>
            </a>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class DashboardComponent {}