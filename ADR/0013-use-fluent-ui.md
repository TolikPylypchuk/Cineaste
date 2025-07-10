# Use Fluent UI as the Front-End UI Framework

**Status:** Superseded by [ADR-0014](0014-use-mudblazor.md)

**Date:** 2023-05-30

## Context

After studying CSS I've become much more proficient in writing UIs effectively. As such, I've decided not to use CSS
frameworks like Bootstrap, and instead write CSS on my own. Unfortunately, Radzen comes with Bootstrap and offers little
in terms of customization. Also, I've come to see that it's tailored more to line-of-business applications.

## Decision

I've desided to use [FAST](https://www.fast.design) – a design framework developed by Microsoft, and its integration
with Blazor – [Blazor Fluent UI](https://www.fluentui-blazor.net). It offers customizability which I think other UI
component frameworks do not.

I've decided not to use FAST design tokens, at least for now, opting instead to customize the UI using pure CSS.

## Consequences

Changing a UI framework is not easy. Fortunately, not much has been implemented in terms of functionality.

The Blazor Fluent UI library is rough around the edges, especially when it comes to customizability with CSS (but it
can still be customized fortunately).

Also, since I've become more proficient with CSS, writing layouts and composing the UI should be much easier and the
code should be much more maintainable.

## Alternatives

The most obvious alternative would be not to change the UI framework at all. Even if I had chose this option, I would've
still rewritten a lot of the UI code since it needed refactoring. And I don't think it would be much easier to
customize Radzen.

Another alternative would be to use another component library for Blazor, but after looking into them, I don't think I
like any of them all that much.

And lastly, I've thought about using FAST components directly, but that would be a lot more needless work. Blazor Fluent
UI provides the integration out-of-the-box. My app doesn't need to look and feel unique or special – after all, I'm
not planning to sell it and make it into a brand.
