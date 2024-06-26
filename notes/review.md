# Review / Key Points / Take-Aways

## Designing APIs
- What is an API?
- What is  an HTTP API? (e.g., "Web API", "REST API", "RESTFUL")
    - Why create an HTTP API?
        - It is pretty "universal" - it is ubiquitous.

## Distributed Applications
- Why?
- What is a "service"? What is the "boundary" of a service?
    - Ownership of state and state process.

## Tech Stuff
- What is OpenAPI?
    - A standard way to document HTTP-based APIs. 
- What is SwaggerUI?
    - An HTML/JavaScript application you can run in a browser that takes an OpenAPI specification and puts a pretty interface on it.
- What is our contribution here?
    - depends.
        - sometimes you don't need it.
- AuthN vs. AuthZ
    - Authentication is verifying the identity of a user or service.
    - Authorization is is determining what that verified user or service is allowed to do.
- Services in .NET
    - Lifetimes (Transient, Scoped, Singleton)
        - Transient - new instance for every use. Never reused. 
        - Scoped - in an API "Scoped" means the single HTTP request. So a new instance will be created for each request, and not shared across other requests (even other requests from the same user)
        - Singleton - shared across all scopes. One instance. Must be either stateless (no data stored in memory) or it has to be written to be thread safe. Be careful with this.
- Controllers or Minimal APIs?
    - How do you choose?
    - What are the gains/losses for each approach?
- What is "branch by feature" and why is that important?

## Classroom Disclaimers
- Source Code Control
- Tests (lack thereof)
- General "quality" (more an issue of quantity)
- Data / Databases / Etc.