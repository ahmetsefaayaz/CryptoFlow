# CryptoFlow API 🚀

CryptoFlow is a comprehensive backend API for cryptocurrency portfolio management. It is built adhering to Clean Architecture principles, ensuring a scalable, testable, and maintainable codebase. The entire infrastructure is fully dockerized for seamless deployment and development.

## 🛠 Tech Stack

* **Framework:** .NET 8 / C#
* **Architecture:** Clean Architecture (Domain, Application, Infrastructure, Persistence, API)
* **Relational Database:** PostgreSQL (via Entity Framework Core)
* **NoSQL Database:** MongoDB (for high-speed transaction logs/dashboard data)
* **Caching:** Redis (for live cryptocurrency prices)
* **Authentication:** ASP.NET Core Identity & JWT
* **Testing:** xUnit & Moq
* **Containerization:** Docker & Docker Compose

---

## 🚀 Getting Started

You don't need to install any databases locally to run this project. Everything is containerized!

## Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

## Installation & Run

1. Clone the repository:
   ```bash
   git clone https://github.com/ahmetsefaayaz/CryptoFlow.git
   ```
   '''bash
   cd CryptoFlow
   '''
## Spin up the infrastructure and the API:
```bash
docker-compose up -d --build
```

* The API will automatically apply EF Core migrations and seed the initial data.

### API Usage & Swagger
Once the containers are up and running, you can access the Swagger UI to test the endpoints:
* **Swagger URL**: http://localhost:8080/swagger

### Database Connections (For Local Inspection)
If you want to inspect the databases using tools like DBeaver, MongoDB Compass, or Redis Insight, use the following connection strings mapped to your local machine:

## PostgreSQL (Main Data & Identity)
* **Host:** localhost
* **Port:** 5433
* **Database:** CryptoFlowDb
* **Username:** postgres
* **Password:** mysecretpassword

## MongoDB (Transactions & Logs)
* **URI:** mongodb://localhost:27018
* **Database:** CryptoNoSqlDb

## Redis (Cache)
* **Host:** localhost:6379

## Running Tests
The application includes comprehensive unit tests covering the core business logic (Order and Deposit services). To run the tests, use the .NET CLI:
```bash
dotnet test
```
### Admin Information
* **Email:** admin@gmail.com
* **Password:** _AdminPassword0
