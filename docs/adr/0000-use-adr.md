# Use Architecture Decision Records (ADRs)

**Status:** Implemented

**Date:** 2022-07-05

## Context

Architecture decisions (AD) should be documented so that people reading code understand _why_ they were made – they can
already see _what_ they are (ideally) just by looking at the code. Maintaining documentation is tedious and error-prone.
Moreover, if documentation is not supported, it turns basically useless, or worse, harms understanding of the decisions.

This is obviously not needed in a simple single-person project such as Cineaste, but I want to experiment what it would
look like in a real project.

One of the possible solutions of the problem is to make docs more or less immutable – these docs should describe ADs,
their context and consequences, and only be updated to amend information, not change it.

## Decision

Architecture decision records (ADR) will be used to keep track of decisions and why they were made. ADRs help structure
docs about decisions in such a way that is easy to maintain.

The specific structure of ADRs may be amended later, but initially they will contain the following:

-   A short title formatted like a git commit message
-   Status (in progress, implemented, superseded by ADR-NNNN)
-   Date
-   Context which describes the issue
-   Decision on how to handle the issue
-   Consequences
-   Alternatives (optional)

ADRs will reside in the 'adr' folder under the 'docs' folder. The file name structure is a four-digit number followed by
the simplified title of the ADR in lowercase with dashes instead of spaces.

## Consequences

Even though ADRs should be easy to maintain, they still require some time and efforts. I believe they will force me to
think about architecture decisions and the docs about them in a more structured way.

## Alternatives

The easiest alternative is not to have any docs about decisions at all. Not viable since the whole point of this is for
me to learn how to document decisions.

Another alternative is to have a more traditional documentation structure with articles that I will have to update. I
believe it's an inferior approach since it won't show the history of decisions in an easy to consume manner.
