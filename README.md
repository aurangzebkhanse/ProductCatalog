# Razor Pages Project with API and WebApp

This project consists of three main components:
1. **API**: A RESTful API for managing products.
2. **WebApp**: A Razor Pages web application that interacts with the API.
3. **Unit Tests**: A set of unit tests for the API.

## Prerequisites

Before running the project, ensure you have the following installed and docker is running locally on your machine:
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

## Running the Project in Docker

Follow these steps to build and run the project in Docker:

### 1. Clone the Repository
Clone the project repository to your local machine:

git clone https://github.com/aurangzebkhanse/ProductCatalog

### 2. Build and Run with Docker Compose

Go to the main solution folder containing docker-compose.yml file.
Use Docker Compose to build and run the containers:

docker-compose up --build

This command will:
- Build the `api` service from `Api/Dockerfile`.
- Build the `webapp` service from `WebApp/Dockerfile`.
- Start both services and connect them via a shared network.
- Initial data will be seeded into the database.

### 3. Access the Application
Once the containers are running:
- **API**: Accessible at `http://localhost:5000/swagger`
- **WebApp**: Accessible at `http://localhost:8080`

For API healthcheck:
-`http://localhost:5000/health`
- It will tell you if the API and db are up and running.

Note: If you can access the application, this means your app is running fine and all the actions
can be performed.

### 4. Login as Admin
To make changes to the product catalog, you must log in as an admin:
- **Username**: `admin`
- **Password**: `password`

### 5. Stopping the Containers
To stop the running containers, use:

docker-compose down

### 6. Rebuilding the Containers
If you make changes to the code and need to rebuild the containers, use:

docker-compose up --build



## API Endpoints

Base URL
The API is accessible at:
http://localhost:5000/api/v1

Note: The API uses versioning, and the current version is '1'.

### Authentication

POST /auth/login
- Description: Logs in as an admin and returns a JWT token.
- Request Body
{
  "username": "admin",
  "password": "password"
}

-Response: JWT token for authentication.

Note: Use the returned JWT token to authenticate admin-only endpoints by including it in the Authorization header:

Authorization: Bearer <JWT_TOKEN>

### Products

****************************************************************************

GET /products

- Description: Retrieves a list of all products.
- Response: List of products.
- Example Response:
[
  {
	"id": 1,
	"name": "Product 1",
	"description": "Description of Product 1",
	"price": 10.0,
	"stock": 100
  },
  {
	"id": 2,
	"name": "Product 2",
	"description": "Description of Product 2",
	"price": 20.0,
    "stock": 100
  }
]

****************************************************************************

GET /products/{id}

- Description: Retrieves a product by its ID.
- Response: Product details.
- Example Response:
{
  "id": 1,
  "name": "Product 1",
  "description": "Description of Product 1",
  "price": 10.0,
  "stock": 100
}

****************************************************************************

Post /products

- Description: Creates a new product.
- Request Body
{
  "id": 111,
  "name": "sample",
  "description": "sample",
  "price": 11,
  "stock": 111
}
- Response: 201 Created
- Example Response:
{
  "id": 111,
  "name": "sample",
  "description": "sample",
  "price": 11,
  "stock": 111
}

- Error Responses:
•	401 Unauthorized: If the user is not authenticated as an admin.

- Note: Requires admin authentication.

****************************************************************************

PUT /products/{id}
- Description: Updates a product by its ID.
- Request Body
{
  "id": 111,
  "name": "Updated Product",
  "description": "Updated description",
  "price": 15.0,
  "stock": 111
}
- Response: 204 No Content

- Error Responses:
•	400 Bad Request: If the id in the URL does not match the id in the request body.
•	404 Not Found: If the product with the specified ID does not exist.
•	401 Unauthorized: If the user is not authenticated as an admin.

- Note: Requires admin authentication.

****************************************************************************

DELETE /products/{id}

- Description: Deletes a product by its ID.
- Response: 204 No Content

- Error Responses:
•	404 Not Found: If the product with the specified ID does not exist.
•	401 Unauthorized: If the user is not authenticated as an admin.

- Note: Requires admin authentication.

****************************************************************************



## What I Wanted to Do but Couldn't Due to Time Constraints

- Adding SQL Server - I used SQLite, but SQL Server would be better.
- API rate limiting
- Better handling exceptions
- Implementing a more robust logging mechanism
- Better handling of versioning
- Redis
- Better user management


I hope you find this project useful and informative. If you have any questions or suggestions, 
feel free to reach out.

