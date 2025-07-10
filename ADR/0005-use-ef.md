# Use Entity Framework as the Persistence Engine

**Status:** Implemented

**Date:** 2022-07-05

## Context

In order to persist data in MS SQL Server some technology should be used: either raw ADO.NET, Dapper, or Entity
Framework (EF).

## Decision

[Entity Framework](https://learn.microsoft.com/en-us/ef) will be used, as it's well-optimized and well-supported. Also,
it's an opportunity for me to learn.

Fluent API will be used to configure the database as persistence concerns will live in a project which is separate from
the one which contains the domain.

## Consequences

Entity Framework requires some setup and configuration. Also, the domain model needs to be modified a little in order to
be supported by EF, so it won't be free of persistence concerns unfortunately.

## Alternatvies

Raw ADO.NET is out of the question. Dapper can be used, and in fact has been used in the desktop version of the app. But
I feel that EF will bring more for me in the long run, and – as always – it's a great way to learn about it.
