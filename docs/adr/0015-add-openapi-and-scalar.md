# Add OpenAPI and Scalar for Documenting and Testing the API

**Status:** Implemented

**Date:** 2025-07-07

## Context

API documentation is a nice-to-have feature, but it enables easier manual testing of the API. OpenAPI is an industry
standard for API documentation so it makes sense to add it. I currently use a Postman collection to test the API
manually, but a better approach should be considered.

## Decision

I'll add [OpenAPI](https://www.openapis.org) documentation to the project using Microsoft's OpenAPI extensions for
ASP.NET Core. I'll also add [Scalar](https://scalar.com) to make API testing easier.

## Consequences

Having Scalar will help me test the API.

OpenAPI and Scalar add very little in terms of maintenance (the only side effect is having to describe controllers and
actions more in-depth).

## Alternatives

[Swagger UI](https://swagger.io/tools/swagger-ui) could have been used instead of Scalar. This requires using
[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) or [NSwag](https://github.com/RicoSuter/NSwag)
instead of Microsoft's OpenAPI package. Since I already have experience with Swagger UI, I've decided to try out
something else. If I don't like Scalar, it will be quite easy to replace it with another tool.
