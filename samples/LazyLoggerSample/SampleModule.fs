module LazyLoggerSample.SampleModule

open FSharp.DI

type SampleModuleLog = class end

// The lazy value is bound when the module loads, but the logger is not resolved
// until log.Value is accessed after DI.init has configured the service provider.
let private log = DI.loggerLazy<SampleModuleLog>()

let run orderId =
    log.Value.info $"Starting work for order {orderId}"
    log.Value.warn "Logger created on first use"
