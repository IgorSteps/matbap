namespace Engine
    module Logger =

        open Serilog

        // Creates new instance of Serilog logger with minimum Debug level and writes to console.
        let New() =
           LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger()