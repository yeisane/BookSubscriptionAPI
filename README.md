# BookSubscriptionAPI

Solution consists of 2 projects. One contains business logic, models, data transfer objects (DTO) and repository classes.

1. [https://github.com/yeisane/BookSubscriptionUI]()
1. [https://github.com/yeisane/BookSubscriptionAPI]()

## Architecture

### App logic

The application implements the Repository Pattern together with Unit Of Work. I created a generic repository that is inheritted by all other repositories. The individual repos add their own extra methods on top of the CRUD ones exposed by the generic repo.

Unit of work consolidates the repos. That way, the rest of the solution is totally unaware of Entity Framework. Unit of work also allows us to Inject on that class and not all repos classes.

### Database

The database of choice was Sqlite for its portability while providing the same power as a standard SQL server db. The database is store in the PROVIDER service, which is the startup project and is named __booksubscription.db__ 

### Authentication

The API implements and Application Token architecture. This allows the api to be consumed externally by subscribers. When they register, the application generates an APIKEY and this should be passed with every API request. The APIKEY is linked to a DNS (e.g. www.sun.com) which is verified when the API passes the APIKEY. So even if someone was to use the key, the call will be rejected because the call will be coming from a different domain.

### Authorization

Some endpoints require a user to be logged in to consume the API. These will be APIs that manage subscribing to books and managing them.


# DEMO

## Login

> Method: POST

> http://localhost:5001/api/auth

> Params:

```json
{
	"email":"admin@gmail.com",
	"password":"1234"
}
```
> Payload:

```json
{
 "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJKb2huZG9lQGhvdG1haWwuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoic3Vic2NyaWJlciIsImV4cCI6MTYwNTAwNDQyOCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAxIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAxIn0.nxcVhWky3wb8UMGwDQ_Jhqz1CBCvIiqOgO0OYhvUWak"
}
```




