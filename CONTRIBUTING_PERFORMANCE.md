# Contributing — Performance & Engineering Standards

This document is ReactiveUI's hot-path engineering rule book: the
allocation, async, type-design, and API-shape standards that production
code under `src/ReactiveUI/` (and the platform extension libraries) is
held to. It is the performance-focused companion to
[`agent.md`](agent.md) and the website contribution guide linked from
[`CONTRIBUTING.md`](CONTRIBUTING.md).

When guidance overlaps:

- **`agent.md` wins** for build/test commands, repository layout, the
  SLNX/MTP/TUnit toolchain, and the AOT guidance (do not duplicate the
  AOT section here — see `agent.md` § *AOT Guidance*).
- **This document wins** for hot-path detail: allocation discipline,
  the System.Reactive boundary, async rules, type design, and the
  perf-driven API-shape rules.
- The website guide (`reactiveui.net/contribute`) remains the narrative
  onboarding doc.

We are tightening performance standards going forward. New code is
expected to meet every rule below. Existing code is migrated
opportunistically — when you touch a file, bring the lines you change up
to standard.

---

## The two tiers: internal vs. public API

ReactiveUI is a widely consumed framework with a large, long-lived
public surface. The rules below apply at **two different strictness
levels**:

- **Internal / private code (strict).** Anything not part of the
  published API — `private` / `internal` members, sink and operator
  implementations, helpers, the expression-rewriting machinery. Apply
  every rule in full. **Internal contracts are not sacred:** if a
  perf-driven change breaks an internal contract, change the contract.
  Internal `[InternalsVisibleTo]` consumers (tests, sibling platform
  assemblies) move with it.
- **Public / user-facing API (careful).** Anything a consumer can
  compile against — public types, public/protected members, public
  return and parameter types, default parameter values already shipped.
  New public API follows the rules. Existing public signatures change
  only through proper deprecation and versioning (`[Obsolete]` →
  removal across a major version), never silently. When a rule below
  would break a shipped public signature, treat the rule as a guide for
  *new* surface and a *migration target* for old surface — not a license
  to break consumers.

When a rule has a different public-vs-internal answer, it says so
explicitly.

---

## The System.Reactive boundary

ReactiveUI is built on the Rx contract and that does not change. What is
changing is **who implements the operators**: we are moving toward our
own low-allocation operator and observer *sinks* rather than routing hot
paths through `System.Reactive.Linq.Observable`.

- **Keep the BCL/Rx contract types — never reinvent them.**
  `IObservable<T>`, `IObserver<T>`, `IScheduler` and `Unit` are the
  ecosystem interop contract every consumer relies on. Depend on them
  directly and pass them through. Authoring parallel substitutes would
  fragment the contract for no gain. This is the deliberate exception to
  the "build our own" rule. (`IObservable<T>` / `IObserver<T>` live in
  the BCL `System` namespace, not `System.Reactive`, so they stay
  regardless; `IScheduler` and `Unit` are the only `System.Reactive`
  types kept.)
- **Disposables are our own internal implementations — not
  `System.Reactive.Disposables`.** We do *not* depend on
  `System.Reactive.Disposables`. Use the owned internal primitives,
  named to avoid colliding with the `System.Reactive` types a consumer
  might also have in scope:
    - `DisposableBag` — composite of child disposables
      (replaces `CompositeDisposable`).
    - `MutableDisposable` — single reassignable slot that does *not*
      dispose the previous value on swap
      (replaces `MultipleAssignmentDisposable`).
    - `SwapDisposable` — single reassignable slot that disposes the
      previous value on swap (replaces `SerialDisposable`).
    - `OnceDisposable` — write-once slot
      (replaces `SingleAssignmentDisposable`).
    - `EmptyDisposable` — shared no-op singleton
      (replaces `Disposable.Empty`).
    - `ActionDisposable` — runs an action on dispose
      (replaces `Disposable.Create(Action)`).
  These are tailored, low-allocation, and only as thread-aware as the
  call site needs — not 1:1 rebadges of the `System.Reactive` shapes.
- **`CompositeDisposable` survives only at the public API edge.** Where
  ReactiveUI's *shipped public surface* exposes `CompositeDisposable`
  (parameter or return type a consumer compiles against), keep it — that
  signature is the contract and changing it breaks consumers. Internally,
  and in all new non-public code, use `DisposableBag`. Never introduce
  `System.Reactive.Disposables` types into internal code paths.
- **Prefer our own operator/observer sinks on hot paths.** Where a
  binding, `WhenAny*`, or command pipeline emits per-value, prefer a
  purpose-built sink with the allocation profile we want over chaining
  `System.Reactive.Linq` operators (`Select`, `Where`, `CombineLatest`,
  `Merge`, `Throttle`, …). A hand-written sink can be `sealed`, avoid
  the closure-per-operator overhead, and fuse adjacent stages.
- **Audit new dependencies on `System.Reactive.Linq` before adding
  them.** If a hot path "needs" an `Observable.Foo`, the preferred
  answer is usually our own sink with the right allocation profile.
  Reach into `System.Reactive.Linq` freely in cold paths (one-time
  setup, configuration, sample/doc code) and at the public API edge
  where we deliberately return composable `IObservable<T>`.
- **`IScheduler` is the scheduling abstraction — pass it through.**
  Prefer `RxSchedulers` (AOT-friendly) over `RxApp` per `agent.md`;
  never bake a scheduler default into a hot path that should accept one.

---

## Allocation discipline

The core of the tightened standard. These apply to production code; test
projects relax the allocation rules (see *Tests & benchmarks*).

- **Zero-LINQ in production code.** No `System.Linq` in production hot
  paths. LINQ pulls in lambdas, iterators, and boxed enumerators on
  every call. Use plain indexed `for` loops. (LINQ *over `IObservable<T>`*
  — the Rx operators — is governed by the System.Reactive boundary
  above, not by this rule; this rule is about `System.Linq` over
  `IEnumerable<T>`.)
- **`for` over `foreach`.** Indexed `for` over arrays / `Span<T>` /
  `ReadOnlySpan<T>` / `List<T>` / `IReadOnlyList<T>`. `foreach` only when
  the type genuinely lacks an indexer (`HashSet<T>`,
  `IAsyncEnumerable<T>`, dictionary enumeration).
- **`static` lambdas everywhere there is no capture.**
  `static x => …` / `static (state, x) => …` lets the JIT skip the
  closure allocation. This matters most in the `WhenAnyValue` / binding
  selectors that allocate per subscription — pass captured state through
  a tuple/state argument rather than closing over locals.
- **Arrays over `List<T>`** when the final length is known up front;
  pre-size and write by index. When `List<T>` is unavoidable, **always
  pass a `capacity`** (`new List<T>(expectedCount)`) — never
  capacity-less.
- **Pre-size `Dictionary` / `HashSet`** with a capacity hint reflecting
  the expected size.
- **Avoid `ImmutableArray<T>` / `ImmutableList<T>` on hot paths.** The
  wrapping struct adds an indirection per read and the builder churns
  intermediate arrays. Reach for an immutable collection only when the
  API is genuinely public and consumers must not mutate. Otherwise
  expose `IReadOnlyList<T>` / `T[]` and treat it as immutable by
  convention.
- **Collection expressions `[..]` first.** `[a, b, ..tail]`, `[]`,
  `[..source]` for final materialization. Never `.ToArray()` when a
  collection expression does the job.
- **Pool transient buffers.** `ArrayPool<T>.Shared.Rent` paired with a
  `try` / `finally` `Return` for transient buffers in pipelines that
  allocate per emission.
- **`Interlocked.Increment` / `Interlocked.Decrement`** for simple
  counters under contention. Reserve `lock` for genuine multi-field
  invariants.
- **`System.Threading.Lock` (net9+) is the default monitor primitive**
  for new code that needs a private gate around shared mutable state:
  `private readonly Lock _gate = new();` and `lock (_gate)`. On
  `net8.0` / `net462` / `net472` / `net481` fall back to
  `private readonly object _gate = new();`; hide the multi-TFM split
  behind a helper where call-site readability matters.
- **No locks on arbitrary objects** (`this`, `typeof(X)`, public
  fields). Always a dedicated `_gate`-style field.

---

## Strings

ReactiveUI is legitimately string-shaped: property names drive
`RaiseAndSetIfChanged` / `nameof`, binding paths and expression chains
resolve to member names, and the public API exchanges `string` freely.
**We are not adopting a no-`string` / UTF-8-bytes policy.** `string`
stays a first-class type, public and internal.

What still applies:

- **Don't allocate strings needlessly on hot paths.** No string
  interpolation or `string.Format` inside a per-emission path to build a
  value that is then discarded or only used on a failure branch — build
  the message lazily, on the throw path.
- **`nameof(...)` over string literals** for member references (already
  required by `agent.md`).
- **`StringComparer.Ordinal` / `StringComparison.Ordinal`** for
  identifier, type-name, and property-name comparisons and for the
  dictionaries/sets keyed on them. Culture-aware comparison is both
  wrong here and several times slower.
- **Spans for cheap parsing/slicing.** Prefer `ReadOnlySpan<char>` +
  range expressions (`path[..i]`, `name[^1]`) over `Substring` when
  picking apart a member path, rather than allocating temporaries.

---

## Async

Where ReactiveUI is async — `ReactiveCommand.CreateFromTask`,
interaction handlers, async bindings — the pipeline is async end to end.

- **No sync-over-async.** Never `.GetAwaiter().GetResult()`, `.Result`,
  or `.Wait()` inside an async path. If the contract hands you a
  `CancellationToken`, you `await`.
- **`ConfigureAwait(false)` on every library `await`** in production
  code. Tests don't need it.
- **Cancellation flows through.** Async operators accept a
  `CancellationToken` where the contract supports it and pass it down —
  never swallow it, never default to `CancellationToken.None` when a real
  token is in scope. Create a
  `CancellationTokenSource.CreateLinkedTokenSource` once at subscribe
  time, not per emission.
- **`ValueTask` first when zero-alloc is proven; `Task` otherwise.** Use
  `ValueTask` when most implementations complete synchronously and the
  call site multiplies (per-emission paths). Use `Task` when the path is
  genuinely async-dominant (I/O) or cold (one call per setup). Obey the
  consume-once rule for `ValueTask`: never `await` the same instance
  twice, never store it in a field.
- **Sync impls return cached completed tasks** — `=> ValueTask.CompletedTask`
  / `Task.CompletedTask`; no state machine, no allocation.

---

## Pattern matching & flow control

- **Invert `if`s to flatten the happy path.** Guard clauses + early
  `return` / `continue` first; main logic stays unindented. No `else` on
  a guarded branch.
- **Switch expressions over `if` / `else` chains** — property patterns
  (`{ HasValue: true }`), positional patterns, list patterns. Order of
  preference: switch expression → switch statement → `if` / `else if`
  chain. Reach for the next form only when the prior cannot express the
  dispatch (mutating `ref` / `out`, side-effects, fall-through).
- **List patterns for emptiness / cardinality.** `is [_, ..]` over
  `.Count > 0`; `is []` for empty; `is [var single]` to bind a
  single-element collection.
- **`is` / `is not` over `==` / `!=`** for null and type checks; combine
  type test + property check in one line where it reads well.
- **Avoid `while (true)`.** Express the termination condition in the loop
  header. The only exception is a genuinely unbounded pump exiting via a
  token, which should be `while (!cancellationToken.IsCancellationRequested)`.

---

## API shape

- **No default parameter values on new APIs.** Default values bake the
  constant into every caller's IL — bumping it later needs a recompile of
  every consumer. Provide explicit overloads instead, each delegating to
  the most-specific overload that takes everything explicitly.
  *(Public two-tier note: ReactiveUI's shipped public API already uses
  defaults in places — `WhenAnyValue`, `ToProperty`, scheduler args.
  Leave those as-is; do not break consumers. Apply this rule to new
  public surface and to all internal surface.)*
- **Concrete collection types in new production APIs** where practical —
  `IReadOnlyList<T>` / `T[]` / `Dictionary<K,V>` / `HashSet<T>` over
  `IEnumerable<T>` for new parameters and return values. `IEnumerable<T>`
  is fine only when a streaming yield genuinely avoids materializing the
  sequence. *(The Rx observer contract is already streaming one value at
  a time; this rule is about `IEnumerable<T>`-style collection params, not
  `IObservable<T>`.)* Existing public signatures that return interface
  types stay.
- **Pin the latest non-beta version** when adding to
  `src/Directory.Packages.props`. Check
  `https://api.nuget.org/v3-flatcontainer/<lower-cased-id>/index.json`
  for the highest stable release; never `-preview` / `-rc` / `-alpha` /
  `-beta`. Same rule for bumps.

---

## Type design

- **`sealed` every class** that isn't designed for inheritance — the
  default for new internal types. Helps inlining and removes accidental
  override surface. *(Public base types intended for derivation —
  `ReactiveObject`, view base classes — stay open by design.)*
- **`readonly record struct`** for immutable value-shaped data: small
  (≤ 4–5 fields) or holding only references. Free equality/hashing, no GC
  pressure.
- **`sealed record` (class)** when the record participates in an
  inheritance hierarchy or holds many fields.
- **Most methods static.** A method that doesn't touch `this` should be
  `static` — fewer hidden allocations, clearer call sites, free
  devirtualization. If a class ends up with only static methods, mark the
  class `static` too.
- **`internal static` helpers** for stateless cross-type utilities.
  Group by responsibility; keep the public surface narrow.
- **Singleton comparers** (`private sealed class XComparer : IComparer<T>`
  with `public static readonly XComparer Instance`) instead of
  allocating a fresh comparer/lambda per `Array.Sort` / dictionary.
- **Bundle long parameter lists into a `readonly record struct` or
  `ref struct`** rather than splitting the method. The state type
  documents the relationship between values and lets the JIT keep them in
  registers.

---

## Properties

- **C# `field` keyword by default.** When a property needs backing logic
  (lazy init, validation, change-tracking), use `field` inside the
  accessors rather than a separate `_name` field. Keep an explicit
  backing field only for:
    - **`ref`-passing APIs** — `Interlocked.Increment(ref _counter)`,
      `Volatile.Read(ref _state)`, `Unsafe.As<T>(ref _slot)`. Document
      with a one-line comment.
    - **Constructor assignment that must bypass setter logic.**
    - **Storage referenced from a method outside the property** (rare).
- **`RaiseAndSetIfChanged` stays the canonical reactive setter.** The
  `field`-keyword guidance is for non-reactive backing logic; reactive
  properties continue to use `this.RaiseAndSetIfChanged(ref field, value)`
  with an explicit backing field (it is a `ref`-passing API — the first
  exception above).

---

## Exception helpers, spans, and read-mostly lookups

- **Exception helpers compose their own messages.** Prefer
  `ArgumentNullException.ThrowIfNull(x)` and
  `[CallerArgumentExpression]`-based helpers over hand-written
  `if (x is null) throw …`; call sites pass only the value, never
  `nameof(x)`. (Matches the `agent.md` zero-pragma `ThrowIfNull` example.)
- **`SearchValues<T>` for repeated multi-character searches.** Cache as
  `private static readonly SearchValues<char>` and pass to
  `IndexOfAny` / `IndexOfAnyExcept` — faster than `IndexOfAny([...])`
  anywhere hit more than once.
- **`TryFormat` / `TryParse` over `ToString` / `Parse`** when writing
  into a span buffer.
- **`FrozenDictionary<K,V>` / `FrozenSet<T>` only when all four hold:**
  built once → queried many times → read-only after construction →
  genuinely hot or broadly shared. The freeze pass is expensive; do not
  use `Frozen*` for per-instance, short-lived, or per-subscription
  tables — a plain `Dictionary` / `HashSet` with the right comparer wins
  there.

---

## Analyzers & suppressions

ReactiveUI runs StyleCop, Roslynator, the .NET CA analyzers, and now
`SonarAnalyzer.CSharp` and `Blazor.Common.Analyzers`. These catch real
perf and correctness issues.

- **Fix the code, don't silence the rule.** Almost every analyzer hit has
  a structural fix — pull out a helper, invert a guard, change a return
  type, restructure a throw. That is preferable to suppression.
- **Zero-pragma policy (see `agent.md`).** No `#pragma warning disable`
  in production code. StyleCop (`SA****`) warnings must be *fixed*, never
  suppressed.
- **`[SuppressMessage]` is a last resort and requires justification.**
  Use a per-symbol `[SuppressMessage("Category", "RuleId", Justification = "…")]`
  naming a concrete reason. CA-rule suppression is allowed only when a
  fix would genuinely harm the design; a second hit on the same rule
  usually means the design is wrong — fix that instead.
- **Zero `<NoWarn>` policy in production projects.** Project-wide
  `<NoWarn>` in `.csproj` / `.props` / `.targets` needs explicit
  consultation and is unlikely to be approved.

---

## Commit style

We follow [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/)
so `git log` is mechanically scannable and release-notes tooling can
group by intent.

```
<type>(<optional scope>): <subject>

<body>

<footers>
```

**Types:** `feat`, `fix`, `perf`, `refactor`, `docs`, `test`, `build`,
`ci`, `chore`, `revert` — standard meanings.

**Scope** is the affected subsystem, lowercase, no `ReactiveUI.` prefix.
Examples: `binding`, `whenany`, `command`, `activation`, `routing`,
`interactions`, `builder`, `scheduler`, plus platform scopes (`wpf`,
`winui`, `maui`, `androidx`, `blazor`, `winforms`). Omit the scope when a
change spans many areas evenly.

**Subject:** ~70 chars, imperative mood, lowercase initial, no trailing
period.

**Body:** explains the *why*. **For `perf` commits, include benchmark
numbers** (before/after, scenario, allocation delta) so the win is
verifiable.

**Footers:** `BREAKING CHANGE: <text>` (or `!` after the type, e.g.
`feat(binding)!:`) for any public-API change; reference issues
(`Closes #123`, `Refs #456`).

Example:

```
perf(whenany): cut per-subscription alloc by hoisting the selector to a static lambda

Replace the captured-closure selector in the two-property WhenAnyValue
overload with a static lambda that threads the source through a value
tuple. The closure object is gone from the subscription path.
Verified on WhenAnyValueBenchmarks: 92 ns -> 61 ns, alloc 64 B -> 0 B.

Refs #1234
```

---

## Tests & benchmarks

- **TUnit + Microsoft Testing Platform** under `src/tests/` (see
  `agent.md` for commands). Treat tests as documentation — names and
  asserts communicate the contract. Prefer real implementations over
  mocks in integration tests.
- **Test allocation rules are relaxed.** `foreach`, `System.Linq`, and
  capacity-less `List<T>` are fine in tests where readability beats
  micro-optimization. The style, pattern-matching, and suppression rules
  still apply.
- **BenchmarkDotNet** under `Benchmarks/` for hot-path work. Always
  include `[MemoryDiagnoser]` and track allocations alongside throughput.
  Add a benchmark when you add or change a hot-path feature; cite its
  numbers in the `perf` commit body.
