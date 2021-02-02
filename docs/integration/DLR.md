# The Dynamic Language Runtime Helper Class
To make use of the DLR Support, reference `VisCPU.Dynamic`.
Follow the Integration Helper Library Setup until step 7 but without calling `instance.Run();`.
After those steps, wrap the Cpu Instance into the `DynamicCpuWrapper` class.
The Dynamic Helper Library allows for simple and effortless access to functions and variables in compiled binaries.

___

Note: Make sure that the flag `-linker:export` is set. The Debug Symbols are needed in order to find the correct data in the binary.

Note: All functions have a return value by default. If a function is not returning explicitly, the returned value is undefined

___

```cs
Cpu instance = builder.Build(); //Create CPU Instance

//Load Compiled Binary from File
uint[] binary = File.ReadAllBytes( file ).ToUInt();

//Write to CPU Memory Bus
instance.LoadBinary( binary );

//Instance Setup Finished

dynamic wrapper = new DynamicCpuWrapper( instance ); //Create Dynamic Wrapper

//Call Entry Point of Loaded Binary
wrapper(); //Invoke directly (Start at address 0)

//Call Function inside the Binary(has to be public)
uint funcReturn = wrapper.MyFunction();

//Call Function with argument
uint funcArgReturn = wrapper.MyArgFunction(1337);

//Access and set Variable inside the Binary
uint myVariable = wrapper.MyVariable;
wrapper.MyVariable = 100;

```