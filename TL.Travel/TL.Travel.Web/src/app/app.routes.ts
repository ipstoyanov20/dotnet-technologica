
import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { 
    path: 'dashboard', 
    loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent) 
  },  
  { 
    path: 'clients', 
    loadComponent: () => import('./clients/clients.component').then(m => m.ClientsComponent) 
  },
  { 
    path: 'operators', 
    loadComponent: () => import('./operators/operators.component').then(m => m.OperatorsComponent) 
  },
  { 
    path: 'hotels', 
    loadComponent: () => import('./hotels/hotels.component').then(m => m.HotelsComponent) 
  },
  { 
    path: 'hotel-rooms', 
    loadComponent: () => import('./hotel-rooms/hotel-rooms.component').then(m => m.HotelRoomsComponent) 
  },
  { 
    path: 'locations', 
    loadComponent: () => import('./locations/locations.component').then(m => m.LocationsComponent) 
  },
  { 
    path: 'reservations', 
    loadComponent: () => import('./reservations/reservations.component').then(m => m.ReservationsComponent) 
  },
  { 
    path: 'reservations/add', 
    loadComponent: () => import('./reservations/reservation-add.component').then(m => m.ReservationAddComponent) 
  },
  { 
    path: 'feeding-types', 
    loadComponent: () => import('./feeding-types/feeding-types.component').then(m => m.FeedingTypesComponent) 
  },
  { 
    path: 'payment-types', 
    loadComponent: () => import('./payment-types/payment-types.component').then(m => m.PaymentTypesComponent) 
  },
  { 
    path: 'extras', 
    loadComponent: () => import('./extras/extras.component').then(m => m.ExtrasComponent) 
  },
  { path: '**', redirectTo: '/dashboard' }
];
