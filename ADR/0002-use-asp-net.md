# Use ASP.NET Core and Blazor

**Status:** Implemented

**Date:** 2022-07-05

## Context

Previously Cineaste was a desktop application. This is convenient as I use my laptop a lot, but it has a couple
drawbacks:
- I can access the list only on my laptop, not on my phone
- Web applications are much better for career growth than desktop ones

As such, I've decided to completely rewrite the application for it to be a web app. Thus I need to decide which
technology will be used for the back-end and the front-end.

## Decision

Using ASP.NET Core for the back-end is a no-brainer. It's the most popular web framework for .NET, it's extensive and
well supported.

For the front-end there are multiple choices. I've decided to use Blazor since it also runs on .NET so will need minimal
JavaScript, if at all. Also, it's a nice opportunity to learn. There are two variants of Blazor - I've chosen Blazor
WebAssembly since I think it's a better workflow, and will allow the app to be semi-offline in the future.

## Consequences

Blazor WebAssembly is slow. This is not a problem for me though, and I hope that in the future it will be much more
optimized.

Also, I truly hope that Microsoft doesn't abandon Blazor, but bad stuff can still happen to it and if it does then me
learning it will not bring much to career growth.

## Alternatives

For the back-end there are no real alternatives for me - ASP.NET Core is just really good. For the front-end I could've
used Angular as I have experience with it, but I haven't worked with it in a long time, so I would've needed to learn it
almost from scratch. Also, Blazor Server could have been used, but I don't think it's a good idea for public-facing apps
which are not backed by powerful hardware.
