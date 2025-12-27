# Patterns Map

| Pattern | Where | Why | Trade-offs |
|--------|-------|-----|-----------|
| Facade | Application/Checkout/CheckoutFacade.cs | Simple API for checkout flows | Can become a "god object" if not kept thin |
| Command + Handler | Application/Commands | Separate intention from execution | More files/types |
| Strategy | Application/Payments/PaymentSelectionStrategy.cs | Select gateway based on rules | Extra indirection |
| Adapter | Infrastructure/Payments/*Adapter.cs | Wrap external gateways behind internal interface | Mapping overhead |
| Decorator | Infrastructure/Decorators | Add logging/retry without changing gateways | Composition complexity |
| Chain of Responsibility | Application/Discounts | Discounts are ordered and extensible | Debugging order matters |
| Domain Events (Observer) | Domain/Orders/Events + Infrastructure/Events | Side effects after domain changes | Event ordering/consistency |
| State | Domain/Orders/OrderState.cs | Enforce valid transitions | More classes for states |
| Repository | Application/Abstractions + Infrastructure/Persistence | Persistence abstraction | Risk of anemic domain if misused |
| Unit of Work | Infrastructure/Persistence/InMemoryUnitOfWork.cs | Transaction boundary | Needs discipline to use correctly |
