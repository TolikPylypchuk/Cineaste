# Use Radzen Blazor as the Library for UI Components

**Status:** Superseded by [ADR-0013](0013-use-fluent-ui.md)

**Date:** 2022-07-05

## Context

I'm not proficient with writing good UI and I'm not planning to be. So, I need to use existing components and design.
There are several good UI component libraries, but most of them are not free.

## Decision

Radzen Blazor will be used for UI components as it's free, easy to use, reasonably good-looking, and integrates with
Bootstrap.

Radzen also supports form validation, but I will not use it, and instead I will roll my own validation infrastructure.

## Consequences

I will have to adapt the UI to the things that Radzen supports. This is OK for me as I don't need a fancy custom design.

## Alternatives

I could've used another free UI component library for Blazor, but I currently like Radzen the most. Also, I could've
used CSS libraries directly (e.g. Bootstrap, Bulma, or Tailwind CSS), but it's too much work for me.
