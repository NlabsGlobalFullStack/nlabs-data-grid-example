import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DataGridComponent, GridColumnCommandTemplateDirective, GridConfig, ODataAdapter } from '@nlabtech/nlabs-grid';

interface User {
  Id: number;
  Name: string;
  Email: string;
  Age: number;
  Salary: number;
  Department: string;
  JobTitle: string;
  City: string;
  Country: string;
  Phone: string;
  Active: boolean;
}

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [DataGridComponent, GridColumnCommandTemplateDirective],
  templateUrl: './users.html',
  styleUrls: ['./users.css']
})
export class UsersComponent {
  gridConfig: GridConfig = {
    columns: [
      { field: 'Id', header: 'ID', sortable: true, filterable: false, width: '80px', type: 'number' },
      { field: 'Name', header: 'Name', sortable: true, filterable: true, width: '180px' },
      { field: 'Email', header: 'Email', sortable: true, filterable: true, width: '220px' },
      { field: 'Department', header: 'Department', sortable: true, filterable: true, width: '150px' },
      { field: 'JobTitle', header: 'Job Title', sortable: true, filterable: true, width: '150px' },
      { field: 'City', header: 'City', sortable: true, filterable: true, width: '120px' },
      { field: 'Salary', header: 'Salary', sortable: true, filterable: true, width: '120px', type: 'number',
        format: (value: number) => '$' + value.toLocaleString() },
      { field: 'Active', header: 'Status', sortable: true, width: '100px', type: 'boolean',
        format: (value: boolean) => value ? '✓ Active' : '✗ Inactive' }
    ],
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    sortable: true,
    filterable: true,
    selectable: true,
    multiSelect: true,
    showCheckboxColumn: true,
    showActions: true,
    actionsHeader: 'Actions',
    actionsWidth: '160px',
    reorderable: true,
    resizable: true,
    emptyMessage: 'No users found'
  };

  dataAdapter: ODataAdapter<User>;

  constructor(private http: HttpClient) {
    this.dataAdapter = new ODataAdapter<User>(http, 'http://localhost:5210/odata/Users');
  }

  onAddUser(): void {
    alert('Add new user - Implement your modal or navigation here');
  }

  onEdit(user: User): void {
    alert(`Edit user: ${user.Name} (ID: ${user.Id})`);
  }

  onDelete(user: User): void {
    if (confirm(`Delete user ${user.Name}?`)) {
      alert(`User ${user.Name} deleted`);
    }
  }

  onExport(data: User[]): void {
    alert(`Exporting ${data.length} users`);
  }
}
