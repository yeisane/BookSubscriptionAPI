# BookSubscriptionAPI

Solution consists of 2 projects. One contains business logic, models, data transfer objects (DTO) and repository classes.

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

