import { HttpClient } from '@angular/common/http';
import { Component, computed, resource, signal } from '@angular/core';
  import { 
  DataGridComponent, 
  GridConfig, 
  GridFooterTemplateDirective, 
  ODataAdapter, 
  User, 
  GridColumnCommandTemplateDirective } from 'nlabs-grid';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [
    DataGridComponent,
    GridFooterTemplateDirective,
    GridColumnCommandTemplateDirective,
],
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
})
export class App {
editingRow: any;
cancelEdit() {
throw new Error('Method not implemented.');
}
saveEdit(_t46: any) {
throw new Error('Method not implemented.');
}
openEditModal(_t46: any) {
throw new Error('Method not implemented.');
}
startEdit(_t46: any) {
throw new Error('Method not implemented.');
}
  // Data source toggle
  useOData = true; // OData'dan başlasın evett

  // Grid configuration
  gridConfig: GridConfig = {
    columns: [
      { field: 'id', header: 'ID', sortable: true, filterable: false, width: '80px', type: 'number' },
      { field: 'name', header: 'Name', sortable: true, filterable: true, width: '200px' },
      { field: 'age', header: 'Age', sortable: true, filterable: true, width: '100px', type: 'number' },
      { field: 'salary', header: 'Salary', sortable: true, filterable: true, width: '150px', type: 'number' },
      { field: 'phone', header: 'Phone', sortable: true, filterable: true, width: '180px' },
      {
        field: 'active',
        header: 'Active',
        sortable: true,
        filterable: false,
        width: '100px',
        type: 'boolean',
        format: (value: boolean) => value ? '✓ Active' : '✗ Inactive'
      }
    ],
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    sortable: true,
    filterable: true,
    selectable: true,
    multiSelect: true,
    showCheckboxColumn: true,
    checkboxColumnWidth: '60px',
    showActions: true,
    actionsHeader: 'İşlemler',
    actionsWidth: '180px',
    reorderable: true,
    resizable: true,
    showHeader: true,
    showFooter: true,
    emptyMessage: 'No users found'
  };

  // Adapters
  // mockAdapter: MockDataAdapter;
  odataAdapter: ODataAdapter<User>;
  dataAdapter: ODataAdapter<User>;

  // Resource-based data fetching
  readonly result = resource({
    loader: async () => {
      const url = 'http://localhost:5210/odata/Users?$top=500&$count=true';
      const res = await lastValueFrom(this.http.get<any>(url));
      
      // Transform PascalCase to camelCase
      const transformedData = (res.value || []).map((item: any) => ({
        id: item.Id,
        name: item.Name,
        age: item.Age,
        salary: item.Salary,
        phone: item.Phone,
        active: item.Active,
      }));

      return transformedData;
    }
  });

  readonly data = computed(() => this.result.value() ?? []);
  readonly loading = computed(() => this.result.isLoading());
  readonly totalRecords = signal(500);

  constructor(
    // private mockAdapterService: MockDataAdapter,
    private http: HttpClient
  ) {
    // this.mockAdapter = mockAdapterService;
    this.odataAdapter = new ODataAdapter<User>(http, 'http://localhost:5210/odata/Users');
    this.dataAdapter = this.odataAdapter; // OData ile başla
  }

  toggleDataSource(): void {
    this.useOData = !this.useOData;
    this.dataAdapter = this.useOData ? this.odataAdapter : this.odataAdapter;
    // this.dataAdapter = this.useOData ? this.odataAdapter : this.mockAdapter;
  }

  onDataLoad(result: any) {
  }

  onRowSelect(row: User) {
  }

  onStateChange(state: any) {
  }

  // New feature handlers
  onAddNewUser(): void {
    alert('Add new user functionality - You can navigate to add page or open a modal here!');
  }

  onExcelExport(data: User[]): void {
    // You can use libraries like xlsx, exceljs for advanced Excel export
    alert(`Exporting ${data.length} records to Excel/CSV`);
  }

  onPdfExport(data: User[]): void {
    // You can use libraries like jspdf, pdfmake for advanced PDF generation
    alert(`Exporting ${data.length} records to PDF`);
  }

  onEditUser(user: User): void {
    console.log('Editing user:', user);
    alert(`Editing user: ${user.name} (ID: ${user.id})`);
    // You can navigate to edit page or open a modal here
  }

  onDeleteUser(user: User): void {
    console.log('Deleting user:', user);
    if (confirm(`Are you sure you want to delete ${user.name}?`)) {
      alert(`User ${user.name} deleted successfully!`);
      // Implement actual delete logic here
    }
  }
}
