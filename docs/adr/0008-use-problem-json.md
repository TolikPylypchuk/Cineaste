# Use application/problem+json to Describe Problems in the API

**Status:** Implemented

**Date:** 2022-07-05

## Context

Proper error handling on the API level is crucial for the app to be maintainable. One of the more difficult decisions is
the format of the errors which the API will return when a problem occurs.

## Decision

`application/problem+json` is a well-defined format for describing problems in the API. Both ASP.NET Core and Refit
know how to work with it.

Exceptions on the application layer need to specify which error has happened (using the type of the exception) and the
infrastructure will transform them into appropriate error responses

The front-end will handle the problems by showing an alert with a localized description of the problem, optionally with
the ability to retry the request again.

## Consequences

Using a well-defined standard of representing problems will make maintaining exception handling easier.
