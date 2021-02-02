# VisCPU Integration

To integrate the CPU into C# Code there are multiple interfaces that can be utilized to accomplish Interopabilty.


## Integration Helper Library
Straight forward, allows for HL to C# interopabilty and limited C# to HL interopabilty.

Setup:

1. Set the App Root with the `AppRootHelper` class.
2. Prepare Temp Paths.
3. (Optional) Enable Event Logging and Crashing with the `EventManager` class.
4. (Optional) Enable Logging by subscribing to the `Logger.OnLogReceive` event.
5. Build the CPU Instance:
```cs
CpuInstanceBuilder builder = new CpuInstanceBuilder()
								.WithPeripherals(
										new ConsoleOutInterface(),
										...
									)
								.WithExposedApi(SleepTime, "Sleep", 1);
```
The Instance Builder follows the Factory Pattern.
A new CPU Instance can be built by calling `builder.Build()` after all peripherals were set up.
C# Code can be Exposed with the `WithExposedApi` function, taking the function, exposed name and argument count.
The Exposed Function has to be imported with `#import "integration <Funcname>` to be able to use it.

6. Compile a Script with the `CompilerHelper.Compile` method.
```cs
string resultFile = CompilerHelper.Compile(
                TempFile, //Input File
                TempBuild, //Build Output Directory
                TempInternalBuild, //Temporary Build Directory, Used for intermediate results.
                false,                     //Does not clean temp files, useful for debugging
                new[] { "HL-expr", "bin" } //Build Steps, last step has to be "bin" to generate .vbin binary
            );
```
7. Load and Run the Compiled Binary:
```cs
Cpu instance = builder.Build(); //Create CPU Instance

//Load Compiled Binary from File
uint[] binary = File.ReadAllBytes( file ).ToUInt();

//Write to CPU Memory Bus
instance.LoadBinary( binary );

//Run the Compiled Binary
instance.Run();
```