# Product Review API

The Product Review API is a RESTful web service designed to manage and retrieve product information, reviews, and related data. It provides endpoints to interact with products, reviews, and other entities in a convenient and efficient manner.

## Features

- Retrieve a list of products along with their details, prices, categories, images, and reviews.
- Get product details by ID, including associated data.
- Add, update, and delete products.
- Manage reviews for products, including adding, updating, and deleting reviews.
- Secure endpoints using JWT-based authentication.
- Error handling for better user experience.

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation

1. Clone the repository:

   ```sh
   git clone https://github.com/your-username/product-review-api.git
   cd product-review-api

2. Update the database connection string in appsettings.json to point to your SQL Server database.
3. Project has EF Migration so run migration
   - [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
4. Call SignUp API to create User now need to change its Role to Admin open the "ReviewDB" database goto "dbo.User" and change Role to "0" (As creating an Admin user is not implemented)

### Authentication

The API uses JSON Web Tokens (JWT) for authentication. To access protected endpoints, include the JWT token in the Authorization header of your HTTP request.
- [MS Authentication.JwtBearer](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer?view=aspnetcore-6.0)

### Error Handling

The API provides detailed error messages with ILogger for easy troubleshooting.
