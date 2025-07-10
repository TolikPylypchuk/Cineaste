# Use Testcontainers for Back-End Integration Tests

**Status:** Implemented

**Date:** 2023-03-19

## Context

Integration tests are much better than unit tests for testing the application layer and database access. Unit tests make
testing the services difficult because mocking the database in a compatible way to the real one is difficult.

## Decision

I've decided not to write unit tests for the application logic. but only for self-contained nuggets of logic such as
validation and domain. The rest will be tested using integration tests.

xUnit will still be used as the testing engine, and [Testcontainers](https://dotnet.testcontainers.org) will be used to
spin up Docker containers with the MS SQL server so that the database is real.

## Consequences

As with unit tests, writing integration is tedious – probably more so since they take longer to run. Still, writing them
is better than not writing them.

## Alternatives

Unit tests can be used instead of integration tests, but at some point, DB integration will have to be tested as well.
I haven't considered any alternatives to Testcontainers as this library is really easy to use and doesn't stand in my
way at all.
