#  Microservices Shop API

A microservices-based backend system built with **.NET 8** and **Docker**.
The project demonstrates secure communication between isolated services, JWT authentication, and container orchestration.

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Docker](https://img.shields.io/badge/Docker-Enabled-blue)
![Architecture](https://img.shields.io/badge/Architecture-Microservices-green)

##  Architecture

The solution consists of two independent microservices running in isolated Docker containers, communicating via a custom bridge network:

1.  **Auth Service (`auth-container`)**:
    * Handles User Registration & Login.
    * Generates and signs **JWT Tokens** (HMACSHA256).
    * Acts as a gateway to proxy requests to the Books Service.
2.  **Books Service (`books-api`)**:
    * Manages the catalog of books/products.
    * Protected API: requires a valid JWT token from the Auth Service.
    * Accessible internally via hostname `http://books-api:8080`.

##  Tech Stack

* **Core:** C# / ASP.NET Core Web API (.NET 8)
* **Containerization:** Docker, Docker Network
* **Security:** JWT Bearer Authentication, BCrypt (Password Hashing)
* **Data:** Entity Framework Core (SQLite support for containers)
* **Testing:** Swagger UI

##  How to Run (Docker)

You can run the entire system locally using Docker commands.

### 1. Create a Network
Create a shared network so containers can communicate by name:
```bash
docker network create shop-network
```

2. Build & Run Auth Service
Navigate to the Auth Service directory:
```bash
docker build -t auth-service .
docker run -p 5000:8080 --name auth-container --network shop-network -e ASPNETCORE_ENVIRONMENT=Development auth-service
```
Accessible at: http://localhost:5000/swagger

3. Build & Run Books Service
Navigate to the Books/Product Service directory:
```bash
docker build -t product-service .
docker run -p 5001:8080 --name books-api --network shop-network -e ASPNETCORE_ENVIRONMENT=Development product-service
```
Accessible at: http://localhost:5001/swagger

Note: The Books Service is available internally to the Auth Service via the hostname http://books-api:8080.
