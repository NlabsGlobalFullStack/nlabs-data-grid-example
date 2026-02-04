import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { DataGridComponent, GridColumnCommandTemplateDirective, GridConfig, RestAdapter } from '@nlabtech/nlabs-grid';

interface Order {
  id: number;
  orderNumber: string;
  orderDate: string;
  customerName: string;
  customerEmail: string;
  city: string;
  country: string;
  status: string;
  totalAmount: number;
}

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [DataGridComponent, GridColumnCommandTemplateDirective],
  templateUrl: './orders.html',
  styleUrls: ['./orders.css']
})
export class OrdersComponent {
  selectedOrder: Order | null = null;

  gridConfig: GridConfig = {
    columns: [
      { field: 'id', header: 'ID', sortable: true, filterable: false, width: '70px', type: 'number' },
      { field: 'orderNumber', header: 'Order #', sortable: true, filterable: true, width: '140px' },
      { field: 'orderDate', header: 'Date', sortable: true, filterable: false, width: '120px',
        format: (value: string) => new Date(value).toLocaleDateString() },
      { field: 'customerName', header: 'Customer', sortable: true, filterable: true, width: '180px' },
      { field: 'city', header: 'City', sortable: true, filterable: true, width: '120px' },
      { field: 'country', header: 'Country', sortable: true, filterable: true, width: '100px' },
      { field: 'status', header: 'Status', sortable: true, filterable: true, width: '110px',
        format: (value: string) => this.getStatusBadge(value) },
      { field: 'totalAmount', header: 'Total', sortable: true, filterable: false, width: '120px', type: 'number',
        format: (value: number) => '$' + value.toLocaleString('en-US', { minimumFractionDigits: 2 }) }
    ],
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    sortable: true,
    filterable: true,
    selectable: true,
    multiSelect: false,
    showCheckboxColumn: false,
    showActions: true,
    actionsHeader: 'Actions',
    actionsWidth: '140px',
    reorderable: true,
    resizable: true,
    emptyMessage: 'No orders found'
  };

  dataAdapter: RestAdapter<Order>;

  constructor(private http: HttpClient) {
    this.dataAdapter = new RestAdapter<Order>(http, 'http://localhost:5210/api/orders');
  }

  getStatusBadge(status: string): string {
    const badges: Record<string, string> = {
      'Pending': '🟡 Pending',
      'Processing': '🔵 Processing',
      'Shipped': '📦 Shipped',
      'Delivered': '✅ Delivered',
      'Cancelled': '❌ Cancelled'
    };
    return badges[status] || status;
  }

  onView(order: Order): void {
    alert(`View order: ${order.orderNumber}\nCustomer: ${order.customerName}\nTotal: $${order.totalAmount}`);
  }

  onEdit(order: Order): void {
    alert(`Edit order: ${order.orderNumber}`);
  }

  onRowSelect(order: Order): void {
    this.selectedOrder = order;
    console.log('Row selected:', order);
  }

  onRowUnselect(order: Order): void {
    this.selectedOrder = null;
    console.log('Row unselected:', order);
  }
}
