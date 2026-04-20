namespace FSharp.DI

open System
open Microsoft.Extensions.Logging

type Log<'t>(_log: ILogger<'t>) =
    let _info (msg: string) = _log.LogInformation(msg)
    let _warn (msg: string) = _log.LogWarning(msg)
    let _error (msg: string) = _log.LogError(msg)
    let _exn (exn: exn, msg: string) = _log.LogError(exn, msg)
    let _trace (msg: string) = _log.LogTrace(msg)
    member _.info = _info
    member _.warn = _warn
    member _.error = _error
    member _.exn = _exn
    member _.trace = _trace
    member _.logger = _log

[<RequireQualifiedAccess>]
module DI =
    type Log = class end
    open Microsoft.Extensions.DependencyInjection

    let mutable private _sp : IServiceProvider =
        ServiceCollection().BuildServiceProvider() :> IServiceProvider

    let private defaultLogger<'t> () = 
        LoggerFactory.Create(fun x -> x.AddConsole() |> ignore).CreateLogger<'t>()

    let logger<'t>() = 
        match _sp.GetService(typeof<ILoggerFactory>) with
        | :? ILoggerFactory as l ->            
            Log(l.CreateLogger<'t>())            
        | _ ->             
            printfn "Logging factory not configured using default console logger"
            Log(defaultLogger<'t>())

    let loggerLazy<'t>() = lazy(logger<'t>())

    let private _log = loggerLazy<Log>()
    
    let service<'t>() =
        match _sp.GetService(typeof<'t>) with
        | null ->
            let msg = $"Unable to find service of type {typeof<'t>}"
            _log.Value.error(msg)
            failwith msg
        | s -> s :?> 't

    let serviceLazy<'t>() : Lazy<'t> =
        lazy (service<'t>())

    let serviceProvider() = _sp
            
    ///Call this in Program.fs to configure DI for functional code
    let init (sp: IServiceProvider) =
        _sp <- sp
