# Project Name: E-Commerce Web Application

## Description
This project is a simple e-commerce web application built using .NET Core and MVC architecture. It allows users to add categories of products and individual products. This application is designed with an N-tier architecture to ensure separation of concerns and implements the Repository pattern along with Unit of Work to manage database operations efficiently.

## Features
- Product and category management: Add and manage categories and products.
- User management: Implemented using .NET Core Identity for handling user authentication and authorization.
- Shopping cart functionality: Supports adding and managing orders.
- Order management: Track and update the status of orders as processing, delivered, or canceled.
- Payment integration: Integrated with Stripe for handling transactions.

## Technologies
- .NET Core
- MVC Architecture
- Entity Framework Core
- Stripe API for payments
- SQL Server for database management

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
What things you need to install the software and how to install them:
- .NET Core SDK
- Visual Studio or similar IDE
- SQL Server

### Installing
A step by step series of examples that tell you how to get a development environment running:
1. Clone the repository to your local machine.
2. Open the solution in Visual Studio.
3. Restore NuGet packages.
4. Update the database connection string in `appsettings.json` to point to your SQL Server.
5. Run the application.

## Database Design
The database is structured into several key tables reflecting different aspects of the e-commerce platform:

### Categories
- **Id** (int): Primary key, auto-incremented.
- **Name** (string): Name of the category, required field, max length 25.
- **DisplayOrder** (int): Numerical order for displaying categories, constrained between 1 and 100.

### Products
- **Id** (int): Primary key, auto-incremented.
- **Title** (string): Product title, required.
- **Description** (string): Description of the product.
- **ISBN** (string): International Standard Book Number, required for books.
- **Author** (string): Author of the book or creator of the product, required.
- **ListPrice** (double): Suggested retail price.
- **Price** (double): Price for quantities between 1-50.
- **Price50** (double): Price for quantities between 51-100.
- **Price100** (double): Price for quantities above 100.
- **CategoryId** (int): Foreign key linked to Categories.
- **ImageUrl** (string): URL to an image of the product.

### ApplicationUser
- Inherits from IdentityUser, adding additional fields:
  - **Name** (string): Full name of the user, required.
  - **StreetAddress**, **City**, **State**, **PostalCode** (string): Address fields.
  - **CompanyId** (int, nullable): Optional foreign key that links to the Company table.

### Orders
- **OrderHeader** and **OrderDetail** tables manage the details and line items of orders respectively.
  - **OrderHeader**: Includes fields for total amount, date of order, status, and user details.
  - **OrderDetail**: Includes fields for product, quantity, and price per unit.

This schema ensures that all aspects of product management, user management, and order processing are handled efficiently within the application.




