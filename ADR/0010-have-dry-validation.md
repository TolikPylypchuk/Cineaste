# Have Hand-Rolled DRY Validation Infrastructure

**Status:** Implemented

**Date:** 2022-07-06

## Context

Validation is extremely important for having consistent data. It is also important to write validation rules in as few
places as possible. Ideally, the domain should be self-validating so that creating invalid domain objects is impossible,
but it's hard to achive in practice. Also, the API requests should be validated, and this validation can be shared
between the front-end and the back-end since they both use C#.

## Decision

I've decided to use [FluentValidation](https://docs.fluentvalidation.net) to create validators as its API is simple and
expressive. I've also decided that application services should accept already-validated requests, so that it's
impossible not to run validation before calling these services.

The domain will be minimally self-validating for the time being since the application services are guaranteed to accept
validated requests. I don't like it very much, but for now it'll do. Maybe it will changed in the future.

## Consequences

Hand-rolled infrastructure means writing and maintaining more technical code, but I don't mind that. Also, this approach
will limit the features of FluentValidation a little, but I don't think it will be a show-stopper.

## Alternatives

Validation rules provided by Radzen Blazor could be used for UI. Also, a more traditional approach for integrating
FluentValidation with ASP.NET Core could be used. I don't think these approaches are going to work better since it will
mean that validation rules will have to be duplicated.
