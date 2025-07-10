# Use xUnit and FsCheck for Back-End Unit Tests

**Status:** Implemented

**Date:** 2022-11-26

## Context

Unit tests are a must. The logic of the project must be rigorously tested to ensure that it doesn't break when changes
are made. Other tests may be added in the future, but unit tests are the cornerstone of testing and should be added
first.

Property-based testing is an approach in which tests show certain properties of code and use randomly-generated data to
semi-prove those properties.

## Decision

I've decided to use [xUnit](https://xunit.net) as the testing framework and the engine for unit-tests on the back-end
and [FsCheck](https://fscheck.github.io/FsCheck) to enable property-based testing. As for mocking,
[NSubstitute](https://nsubstitute.github.io/) will be used if it's required.

## Consequences

Writing tests is tedious, but required for a healthy project. Having unit tests will make me more disciplined about
structuring my code and thinking about desgn consequences. I will also gain experience in writing tests.

## Alternatives

The most obvious alternative is not to write tests at all, but that's out of the question.

NUnit and MSTest are alternative unit-testing frameworks, but I'm most accustomed to xUnit. Property-based testing is
not required, but I like this approach a lot.
