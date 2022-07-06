# Use MVVM and ReactiveUI for the Front-End

**Status:** Superseded by [ADR-0009](0009-use-fluxor.md)

**Date:** 2022-07-05

## Context

Since most of the code will initially be on the front-end, the code absolutely must be organized in some way. There are
several ways to do that, some more suitable for the web than others

## Decision

I will use the MVVM pattern for front-end code organization, and ReactiveUI in particular as the MVVM framework. This
will allow the code to be written in a reactive style which is a nice fit for UI.

I've already used ReactiveUI quite a lot in the desktop version of the app, so I'm proficient with it. Its support for
Blazor is a little different than for desktop frameworks, but seems to be OK.

## Consequences

The UI code will be written in a reactive style which I really like, but will require a havey-weight view model for
every view.

## Alternatives

There are other MVVM frameworks out there, but I like ReactiveUI the most. Also, state can be managed directly by
components, but I've decided against it.
