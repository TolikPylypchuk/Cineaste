# Have a Rich Domain Model

**Status:** Implemented

**Date:** 2022-07-05

## Context

It's really important for a project to model the domain correctly. There are multiple formats of domain models: anemic
domain, rich domain, active records etc.

Practicing domain-driven design (DDD) should be a nice experience, but obviously full DDD is impossible in a
single-person project.

The domain of Cineaste is quite simple - it's mostly just a CRUD app, but I think it will be extended to be more than
that.

## Decision

The domain model will be rich - the entities should be at least minimally self-validating, and contain behavior
associated with them right inside. Also, they should be as oblivious to the persistence mechanism, as possible.

The only reason I've chosen to have a rich domain model is to practice creating a rich domain model.

## Consequences

Having a rich domain model will make reasoning about it easier. On the other hand, such a simple project may not benefit
from a rich domain model. Thus some parts of the app may be a little more complex than they should be.

Having the domain model be oblivious to the persistence technology may be unachievable, and at the very least the
persistence concerns will have to adapt significantly.

## Alternatives

An anemic domain model could also very well be used here. It may actually simplify stuff yet I still want to try to have
the domain model be rich.
