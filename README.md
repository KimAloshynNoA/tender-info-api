
# Tender Info API

Tender Info API is a RESTful service that allows users to interact with tenders data. It supports filtering, sorting, and pagination of tenders, as well as retrieval of tenders by ID or by supplier.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Running the Application](#running-the-application)
- [Manual Testing](#manual-testing)
- [Running Unit Tests](#running-unit-tests)

## Prerequisites

Before you begin, ensure you have the following installed:

- **Docker**: Docker is required to build and run the application container.
- **Git**: Git is needed to clone the repository.

## Getting Started

1. **Clone the Repository**

   Clone the repository to your local machine using Git:

   ```bash
   git clone [clone url]
   cd tender-info-api
   ```

2. **Build the Docker Image**

   Build the Docker image for the application:

   ```bash
   docker build -t tenderinfoapi .
   ```

3. **Run the Docker Container**

   Start the application container, mapping port `8080` from the container to port `8080` on your local machine:

   ```bash
   docker run -d -p 8080:8080 --name tenderinfoapi-container tenderinfoapi
   ```

## Running the Application

Once the container is running, the application will be available at `http://localhost:8080`.

### Access Swagger Documentation

You can view the Swagger API documentation and interact with the API using the Swagger UI:

- **Swagger UI**: [http://localhost:8080/swagger](http://localhost:8080/swagger)

Swagger UI provides a web interface for interacting with the API endpoints directly from your browser.

## Manual Testing

You can manually test the API using the following tools:

**Browser** or **Postman**: You can perform simple GET requests by navigating to the following URLs:
  - **Get All Tenders**: `http://localhost:8080/api/tenders`
  - **Get Tender by ID**: `http://localhost:8080/api/tenders/{id}`
  - **Get Tenders with Filters**: Create a GET request in Postman with the URL `http://localhost:8080/api/tenders`, and add query parameters such as `MinPrice`, `MaxPrice`, `StartDate`, `EndDate`, `OrderBy`, `OrderDirection`, `Page`, and `PageSize`.


## Running Unit Tests

Unit tests are included in the project and can be run to ensure that the application behaves as expected. Follow these steps to run the tests:

1. **Run Tests in Docker Container**

   The unit tests are not included in the production Docker image, but you can run them in a local environment where the .NET SDK is available.

2. **Run Tests Locally**

   To run the unit tests locally, you must have the .NET SDK installed:

   ```bash
   dotnet test
   ```

   This command will discover and run all the unit tests in the solution. It will output the results to the console.

3. **Viewing Test Results**

   After running the tests, you’ll see a summary in the console indicating how many tests passed, failed, or were skipped.
