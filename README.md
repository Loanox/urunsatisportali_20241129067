# Production Sale Admin Panel

A comprehensive admin panel for managing production sales, built with ASP.NET Core MVC 8.0.

## Features

- **Dashboard**: Overview with statistics, recent sales, and low stock alerts
- **Product Management**: Create, edit, delete, and view products with categories
- **Category Management**: Organize products by categories
- **Customer Management**: Manage customer information and contact details
- **Sales Management**: Create sales orders, view sale details, and track revenue
- **Inventory Tracking**: Real-time stock quantity management
- **Modern UI**: Responsive design with Bootstrap 5 and Bootstrap Icons

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or SQL Server Express)
- Visual Studio 2022 or Visual Studio Code (optional)

## Setup Instructions

1. **Clone or navigate to the project directory**
   ```bash
   cd urunsatisportali
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update the database connection string** (if needed)
   - Open `appsettings.json`
   - Modify the `DefaultConnection` string if you're using a different SQL Server instance

4. **Create and apply database migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

   If you don't have Entity Framework tools installed globally:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the admin panel**
   - Navigate to: `https://localhost:7047` or `http://localhost:5240`
   - The application will automatically redirect to the Admin Dashboard

## Database Schema

The application uses Entity Framework Core with the following entities:

- **Categories**: Product categories
- **Products**: Product information with pricing and inventory
- **Customers**: Customer contact and address information
- **Sales**: Sales orders with totals and status
- **SaleItems**: Individual items within each sale

## Admin Panel Sections

### Dashboard
- Total products, customers, and sales statistics
- Total revenue overview
- Low stock product alerts
- Recent sales list

### Products
- View all products with search and filter by category
- Create new products with pricing and inventory
- Edit product details
- View product details
- Delete products

### Categories
- Manage product categories
- Create, edit, and delete categories

### Customers
- Manage customer database
- Search customers by name, email, or phone
- Create, edit, and delete customer records

### Sales
- View all sales with search and status filter
- Create new sales with multiple items
- View detailed sale information with itemized lists
- Automatic stock deduction on sale creation

## Technologies Used

- ASP.NET Core MVC 8.0
- Entity Framework Core 8.0
- SQL Server
- Bootstrap 5
- Bootstrap Icons
- jQuery

## Default Data

The application includes seed data with:
- 4 sample categories (Electronics, Clothing, Food & Beverages, Home & Garden)
- 3 sample products

## Notes

- Stock quantities are automatically updated when sales are created
- Sales are automatically assigned a unique sale number
- Products can be marked as active/inactive
- Low stock threshold is set at 10 units (shown on dashboard)

## License

This project is provided as-is for educational and commercial use.

