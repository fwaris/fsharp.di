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
