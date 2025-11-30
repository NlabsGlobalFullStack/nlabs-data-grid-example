# nlabs-data-grid-example

This repository contains two separate Angular projects for demonstrating and developing the **nlabs-grid** component library.

## Package Links

- **NPM Package**: [https://www.npmjs.com/package/nlabs-grid](https://www.npmjs.com/package/nlabs-grid)
- **GitHub Repository (Library)**: [https://github.com/NlabsNpmPackages/nlabs-grid](https://github.com/NlabsNpmPackages/nlabs-grid)
- **GitHub Repository (Angular Examples)**: [https://github.com/nlabsGlobalAngular/nlabs-grid](https://github.com/nlabsGlobalAngular/nlabs-grid)

## Project Structure

```
nlabs-data-grid-example/
├── nlabs-grid/              # Angular library project
│   ├── projects/
│   │   └── nlabs-grid/      # The grid component library source
│   └── dist/                # Built library output
│
└── nlabs-grid-angular/      # Demo/Example application
    └── src/
        └── app/             # Example implementation
```

## Projects

### 1. nlabs-grid (Library)

The main Angular component library that provides a modern, feature-rich data grid.

**Location**: `./nlabs-grid/`

**Key Features**:
- Modern Angular 20+ standalone component
- Full TypeScript support
- OData adapter for server-side data
- Custom checkboxes with modern design
- Customizable actions column
- Theme support (light/dark)
- Column sorting, filtering, resizing, reordering
- Row selection (single/multi)
- Pagination
- Custom templates support

**Commands**:
```bash
cd nlabs-grid

# Install dependencies
npm install

# Build library
npm run build

# Build in watch mode
ng build nlabs-grid --watch

# Run tests
ng test
```

**Documentation**: See [nlabs-grid README](./nlabs-grid/projects/nlabs-grid/README.md) for detailed API documentation.

### 2. nlabs-grid-angular (Demo App)

A demo Angular application showcasing the nlabs-grid component with real-world examples.

**Location**: `./nlabs-grid-angular/`

**Features Demonstrated**:
- OData integration with backend API
- Custom action buttons (Edit, Delete)
- Custom footer template
- Row selection with checkboxes
- Theme switching
- Sorting and filtering
- Column configuration

**Commands**:
```bash
cd nlabs-grid-angular

# Install dependencies
npm install

# Start development server
npm start

# Build for production
npm run build
```

**Local Development**: The demo app imports the library from local path:
```typescript
import { DataGridComponent } from '../../../nlabs-grid/projects/nlabs-grid/src/public-api';
```

## Development Workflow

### Option 1: Using Local Library (Current Setup)

The demo app currently imports directly from the library source for faster development:

1. Make changes in `nlabs-grid/projects/nlabs-grid/`
2. Changes are reflected immediately in the demo app
3. No build step required

### Option 2: Using Built Library

To test the published version:

1. Build the library:
   ```bash
   cd nlabs-grid
   ng build nlabs-grid
   ```

2. Update demo app imports to use the built package:
   ```typescript
   // In nlabs-grid-angular/src/app/app.ts
   import { DataGridComponent } from 'nlabs-grid';
   ```

3. Start demo app:
   ```bash
   cd nlabs-grid-angular
   npm start
   ```

## Quick Start

### 1. Setup Both Projects

```bash
# Clone repository
git clone <repository-url>
cd nlabs-data-grid-example

# Install library dependencies
cd nlabs-grid
npm install

# Install demo app dependencies
cd ../nlabs-grid-angular
npm install
```

### 2. Run Demo Application

```bash
# From nlabs-grid-angular directory
npm start

# Open browser to http://localhost:4200
```

### 3. Start Backend (Optional)

The demo uses OData backend at `http://localhost:5210/odata/Users`.

To run the demo with live data, ensure your OData backend is running.

## Example Usage

Here's a basic example from the demo app:

```typescript
import { Component } from '@angular/core';
import {
  DataGridComponent,
  GridConfig,
  ODataAdapter,
  GridColumnCommandTemplateDirective
} from 'nlabs-grid';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    DataGridComponent,
    GridColumnCommandTemplateDirective
  ],
  template: `
    <nlabs-data-grid
      [config]="gridConfig"
      [adapter]="dataAdapter"
      [theme]="'dark'"
      (rowSelect)="onRowSelect($event)">

      <!-- Custom Actions -->
      <ng-template nlabsGridColumnCommandTemplate="actions" let-row>
        <button class="btn-edit" (click)="onEdit(row)">
          ✏️ Edit
        </button>
        <button class="btn-delete" (click)="onDelete(row)">
          🗑️ Delete
        </button>
      </ng-template>
    </nlabs-data-grid>
  `
})
export class App {
  gridConfig: GridConfig = {
    columns: [
      { field: 'id', header: 'ID', width: '80px', type: 'number' },
      { field: 'name', header: 'Name', sortable: true, filterable: true },
      { field: 'email', header: 'Email', sortable: true, filterable: true }
    ],
    pageSize: 10,
    selectable: true,
    multiSelect: true,
    showCheckboxColumn: true,
    showActions: true,
    actionsHeader: 'Actions',
    actionsWidth: '180px'
  };

  dataAdapter: ODataAdapter;

  constructor(private http: HttpClient) {
    this.dataAdapter = new ODataAdapter(
      http,
      'http://localhost:5210/odata/Users'
    );
  }

  onEdit(row: any) {
    console.log('Editing:', row);
  }

  onDelete(row: any) {
    console.log('Deleting:', row);
  }

  onRowSelect(row: any) {
    console.log('Selected:', row);
  }
}
```

## Key Components

### Data Grid Component

**Location**: `nlabs-grid/projects/nlabs-grid/src/lib/components/data-grid/`

Main component providing the grid functionality.

### OData Adapter

**Location**: `nlabs-grid/projects/nlabs-grid/src/lib/adapters/odata-adapter.ts`

Handles server-side data fetching with OData protocol.

### Directives

- **GridColumnCommandTemplateDirective**: For custom action templates
- **GridFooterTemplateDirective**: For custom footer templates

### Services

- **GridDataService**: Manages grid data operations
- **ThemeService**: Handles theme switching

## Styling

The grid uses CSS variables for theming. See the library README for complete theming guide.

### Example Custom Styles

```scss
// In your app.scss
:root {
  --grid-primary-color: #4096ff;
  --grid-bg-primary: #ffffff;
  --grid-text-primary: #262626;
}

[data-theme='dark'] {
  --grid-bg-primary: #1f1f1f;
  --grid-text-primary: #e0e0e0;
}

.btn-edit {
  color: var(--grid-primary-color);
  border: 1px solid var(--grid-primary-color);

  &:hover {
    background: var(--grid-primary-color);
    color: white;
  }
}
```

## Features Showcase

### ✅ Checkbox Selection
- Modern custom checkbox design
- Single and multi-select modes
- Select all functionality
- Corporate-style with smooth animations

### ✅ Actions Column
- Fully customizable via ng-template
- Pre-styled button classes (btn-edit, btn-delete, btn-view)
- Theme-aware styling

### ✅ Sorting & Filtering
- Multi-column sorting
- Per-column filters
- Server-side and client-side support

### ✅ Column Management
- Drag-and-drop reordering
- Resizable columns
- Column visibility toggle

### ✅ Themes
- Built-in light/dark themes
- CSS variable-based
- Easy customization

## Browser Requirements

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Technology Stack

- **Angular**: 20.0.0
- **TypeScript**: 5.8.2
- **RxJS**: 7.8.0
- **Zone.js**: 0.15.0

## Contributing

This is an example/demo repository. For contributing to the nlabs-grid library itself, please see the library's documentation.

## License

MIT License

## Authors

nLabs Development Team

## Support

For issues and questions:
- Library issues: See nlabs-grid documentation
- Demo app issues: Use this repository's issue tracker

## Additional Resources

- [Angular Documentation](https://angular.dev)
- [OData Protocol](https://www.odata.org/)
- [TypeScript Documentation](https://www.typescriptlang.org/)

## Notes

- The `nlabs-grid-angular` app uses direct imports from the library source for development convenience
- For production use, the library should be built and published to npm
- The demo requires an OData backend running on port 5210 for full functionality
- Both projects are kept in the same repository for easier cross-reference during development
