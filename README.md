# nlabs-data-grid-example

Bu proje, Angular tabanlı bir veri grid uygulaması ve .NET tabanlı bir OData backend içerir. Amaç, modern web uygulamalarında veri gridlerinin nasıl kullanılabileceğini ve OData ile backend entegrasyonunun nasıl yapılacağını göstermektir.

## Proje Yapısı

- **nlabs-grid-angular/**: Angular ile geliştirilmiş frontend uygulaması.
- **ODataBackend/**: .NET ile geliştirilmiş OData backend API.

## Kurulum ve Çalıştırma

### 1. Backend (ODataBackend)

```sh
cd ODataBackend
# Gerekli NuGet paketlerini yükleyin
dotnet restore
# Uygulamayı başlatın
dotnet run
```

Backend varsayılan olarak `https://localhost:5001` adresinde çalışır.

### 2. Frontend (nlabs-grid-angular)

```sh
cd nlabs-grid-angular
npm install
npm start
```

Frontend varsayılan olarak `http://localhost:4200` adresinde çalışır.


## Angular Kullanım Örnekleri

### 1. Dynamic Column Definition ile Kullanım

```html
<nlabs-data-grid
	[data]="data()"
	[totalRecords]="totalRecords()"
	[lazy]="false"
	[autoLoad]="false"
	[theme]="'light'"
	[showThemeSelector]="true"
	[showColumnChooser]="true"
	[showGlobalSearch]="true"
	[showAddButton]="true"
	[addButtonText]="'Add New User'"
	[showExport]="true"
	[exportFileName]="'users-export'"
	[showFooter]="true"
	(dataLoad)="onDataLoad($event)"
	(rowSelect)="onRowSelect($event)"
	(stateChange)="onStateChange($event)"
	(addClick)="onAddNewUser()"
	(excelExport)="onExcelExport($event)"
	(pdfExport)="onPdfExport($event)"
>
	<nlabs-grid-column field="id" title="ID" width="70px" [sortable]="true" [filterable]="false" />
	<nlabs-grid-column field="name" title="Full Name" [sortable]="true" />
	<nlabs-grid-column field="email" title="Email Address" [sortable]="true" />
	<!-- ... diğer kolonlar ... -->
	<ng-template flexiGridColumnCommandTemplate let-item>
		<button class="btn-icon btn-sm" title="View">👁️</button>
		<button class="btn-icon btn-sm" title="Edit">✏️</button>
		<button class="btn-icon btn-sm" title="Delete">🗑️</button>
	</ng-template>
	<ng-template flexiGridFooterTemplate let-data let-total="total">
		<div>
			📊 Total Records: <strong>{{ total }}</strong>
		</div>
	</ng-template>
</nlabs-data-grid>
```

### 2. Config Tabanlı Kullanım

```typescript
// app.ts
gridConfig: GridConfig = {
	columns: [
		{ field: 'id', header: 'ID', sortable: true },
		{ field: 'name', header: 'Name', sortable: true },
		// ... diğer kolonlar ...
	],
	pageSize: 10,
	sortable: true,
	filterable: true,
	// ... diğer ayarlar ...
};
```

```html
<nlabs-data-grid
	[config]="gridConfig"
	[data]="data()"
	[totalRecords]="totalRecords()"
	[adapter]="dataAdapter"
	[autoLoad]="true"
	[lazy]="false"
	[theme]="'light'"
	[showThemeSelector]="true"
	[showColumnChooser]="true"
	[showGlobalSearch]="true"
	[showAddButton]="true"
	[addButtonText]="'Add New User'"
	[showExport]="true"
	[exportFileName]="'users-export'"
	[showFooter]="true"
	(dataLoad)="onDataLoad($event)"
	(rowSelect)="onRowSelect($event)"
	(stateChange)="onStateChange($event)"
	(addClick)="onAddNewUser()"
	(excelExport)="onExcelExport($event)"
	(pdfExport)="onPdfExport($event)"
>
	<ng-template flexiGridFooterTemplate let-data let-total="total">
		<div>
			📊 Total Records: <strong>{{ total }}</strong>
		</div>
	</ng-template>
</nlabs-data-grid>
```

## Katkı

Katkıda bulunmak için lütfen bir fork oluşturun ve pull request gönderin.
