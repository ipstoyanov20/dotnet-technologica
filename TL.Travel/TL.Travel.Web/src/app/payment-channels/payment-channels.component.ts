import { Component, OnInit } from '@angular/core';
import { PaymentChannelsService } from '../api/payment-channels.service';

@Component({
  selector: 'app-payment-channels',
  templateUrl: './payment-channels.component.html',
  styleUrls: ['./payment-channels.component.css']
})
export class PaymentChannelsComponent implements OnInit {
  paymentChannels: any[] = [];
  loading = false;

  constructor(private paymentChannelsService: PaymentChannelsService) {}

  ngOnInit(): void {
    this.fetchPaymentChannels();
  }

  fetchPaymentChannels() {
    this.loading = true;
    this.paymentChannelsService.getAll().subscribe({
      next: (data) => { this.paymentChannels = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
