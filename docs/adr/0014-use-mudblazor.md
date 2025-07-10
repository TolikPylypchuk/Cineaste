# Use MudBlazor as the Front-End UI Framework

**Status:** In progress

**Date:** 2025-07-10

## Context

Unfortunately, Microsoft's Fluent UI library is not easily customizable, and quite incomplete. Another choice for a
component library is required.

## Decision

I've desided to move to [MudBlazor](https://www.mudblazor.com) as it appears to be the most mature UI component library
for Blazor. While it looks like Material Design by default, it appears to be customizable.

## Consequences

All the consequences from the previous ADR apply here as well. But again, I haven't implemented much yet, so it should
be relatively easy to switch.

## Alternatives

All the alternatives from the previous ADR apply here as well. I could've stayed with Fluent UI, or I could've used a
different UI library, but MudBlazor seems like the best choice at the moment.
