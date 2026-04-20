# FSharp.DI

Lightweight functional helpers for Microsoft dependency injection and logging in F#.

## Quick Start

- Target framework: .NET 10.0
- Install the package (when published):

```bash
dotnet add package FSharp.DI
```

## Configure Logging in `Program.fs`

This is a minimal example of an F# app configuring `Microsoft.Extensions.Logging`,
initializing `FSharp.DI`, and then calling a module that has a lazy logger bound with `let`:

```fsharp
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open FSharp.DI
open LazyLoggerSample.SampleModule

[<EntryPoint>]
let main _ =
    let services =
        ServiceCollection()
            .AddLogging(fun logging ->
                logging.ClearProviders() |> ignore
                logging.SetMinimumLevel(LogLevel.Information) |> ignore
                logging.AddSimpleConsole(fun options ->
                    options.TimestampFormat <- "HH:mm:ss "
                    options.SingleLine <- true
                ) |> ignore
            )
            .BuildServiceProvider()

    DI.init services
    run 42
    0
```

## Module-Level Lazy Logger Example

The sample module keeps a lazy logger in a normal module-level `let` binding:

```fsharp
module LazyLoggerSample.SampleModule

open FSharp.DI

type SampleModuleLog = class end

let private log = DI.loggerLazy<SampleModuleLog>()

let run orderId =
    log.Value.info $"Starting work for order {orderId}"
    log.Value.warn "Logger created on first use"
```

The `let` binding is created when the module loads, but the actual logger is not resolved until
the first `log.Value` access. That means the pattern is safe as long as `DI.init` runs before
`run` is called.

A runnable version of this example is included in `samples/LazyLoggerSample/`.

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

## Author

Faisal Waris — © 2026
