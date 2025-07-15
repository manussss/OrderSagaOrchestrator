# Project

Complete runnable example of an orchestrated SAGA in .NET 8 using Minimal APIs.

The scenario is a classic Create Order workflow that spans three autonomous services:

Order Service – persists the order.
Inventory Service – reserves items.
Payment Service – captures (or refunds) the payment.
An OrderSaga Orchestrator drives the whole flow, decides the next step, and triggers the compensating actions if anything goes wrong.

# Solution Layout

Each folder is an independent project that you can run on a different port (or Docker container).

Only the orchestrator keeps state about the saga; every other service is completely unaware it is part of a larger transaction.