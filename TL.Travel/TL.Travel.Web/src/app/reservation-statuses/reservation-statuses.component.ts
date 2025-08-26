import { Component, OnInit } from '@angular/core';
import { ReservationStatusesService } from '../api/reservation-statuses.service';

@Component({
  selector: 'app-reservation-statuses',
  templateUrl: './reservation-statuses.component.html',
  styleUrls: ['./reservation-statuses.component.css']
})
export class ReservationStatusesComponent implements OnInit {
  reservationStatuses: any[] = [];
  loading = false;

  constructor(private reservationStatusesService: ReservationStatusesService) {}

  ngOnInit(): void {
    this.fetchReservationStatuses();
  }

  fetchReservationStatuses() {
    this.loading = true;
    this.reservationStatusesService.getAll().subscribe({
      next: (data) => { this.reservationStatuses = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
