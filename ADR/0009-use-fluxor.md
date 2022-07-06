# Use Fluxor and the Redux Pattern for the Front-End

**Status:** Implemented

**Date:** 2022-07-06

## Context

Global state management with ReactiveUI (and MVVM) can turn ugly really quickly. This is actually one of its pain points
in my opinion.

Also, Blazor doesn't play nicely with MVVM (it wasn't created with this pattern in mind), so ReactiveUI feels weird to
use with Blazor compared to WPF or Avalonia.

As such, alternatives to ReactiveUI should be considered in greater detail while the app is still very new.

## Decision

When it comes to state management, I've decided to give the Redux pattern a try, and Fluxor - its implementation for
.NET.

The advantages that each component's state is immutable, and data flows in a single direction. I really like this
approach - this makes hydrating components with pre-existing state trivial. It also reminds me of MVU, albeit the UI is
not immutable here.

Also, I've decided not to use Fluxor to manage the entire state of components. If a component is a form, or has inputs,
then those inputs will be bound to the component's properties directly. Fluxor will be used only for the
behind-the-scenes concerns.

## Consequences

ReactiveUI and MVVM will be removed, and the components will be rewritten to use Fluxor.

I'll need to get used to the way Fluxor manages state.

I think the library is powerful enough and will serve my needs without me needing to write additional state-managing
stuff.

## Alternatives

I could've continued using ReactiveUI, but I see that it's unwieldy. Also, I don't want to use MVVM at all here.

There are other state-managing libraries which implement the Redux pattern, but Fluxor seems the most mature one.
