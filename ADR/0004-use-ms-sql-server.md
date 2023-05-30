# Use Microsoft SQL Server as the Database

**Status:** Implemented

**Date:** 2022-07-05

## Context

Data for the project has to be stored somewhere, and the relational model is the go-to solution for simple projects.

## Decision

Since [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server) (or MS SQL) is commonly used with .NET, that's
the database I'm going to use for the project. The Express edition is free and more than enough for my needs.

## Consequences

The most immediate consequence is that I'll have to learn how to use MS SQL Server. Other than that, smooth sailing is
expected.

## Alterntives

The simplest alternative for me is to use PostgreSQL as this is the RDBMS which I've used the most. I've decided against
it and in favor of learning something new.

Another alternative is not to use the relational model at all, and use a NoSQL database. I've contemplated doing event
sourcing with a NoSQL DB, but ultimately decided against it for now since it will needlessly complicate things in the
beginning of the project.

I've also looked at EdgeDB - a 'next-generation' graph-relational database. It's still too bleeding-edge to use, and its
support for .NET is almost non-existent.
