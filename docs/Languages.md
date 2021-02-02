# Supported Languages

List and Description of supported Languages

## Variable Assembly Language (VASM)

VASM is an Assembly Language that can (mostly) be translated 1:1 into machine instructions

The Language compiler has 2 main steps

- Linker
- Assembler

### The VASM Linker
The Linker is responsible for processing included files and fixing the address layout of included files. So that labels that are defined in other files can be used.
By Default all Defined Labels are usable in all scripts.
To hide labels from other files, the labels can be decorated with `linker:hide`.

### The VASM Assembler
Generates the Binary Data from the Intermediate Result of the Linker.
The Assembler ajusts the Addresses so that the Linked assembly data can be merged into a single executable binary.

### Language Features
The VASM Language is meant to be a close representation of the actual binary code.
Instructions are selected by Specifying the Mnemonic followed by the arguments for the Instruction.
```arm
:data MyData 0x01 ; MyData is a label that has a size of 1.

LOAD 1 MyData ; Loads the Value 1 into the MyData Label
ADD MyData MyData ; Adds MyData onto itself. => MyData = MyData + MyData
ADD MyData MyData MyData ; Same as above. But specifies the Result Address.
INC MyData ; Increments the MyData label by 1
```

### `:data` Labels
`:data` labels are used to define a section of memory that can be used by specifying the name of the data section.

The Actual Memory Layout is handled by the Compiler. The Actual Addess in memory can be loaded by treating the label as "literal value"
```arm
:data MyLabel 0x1 ; No Initialization

:data MyData 0x1 0xFFFF ; MyData is Initialized with the Value 0xFFFF
:data MyString "VASM Supports Strings!" ; MyString is initialized with the string data. (size is equal to string length)

:data MyLabelPointer 0x1

LOAD MyLabel MyLabelPointer ; Loads the Address of MyLabel into MyLabelPointer
```

Because of the abilty to load pointers and perform operations on them, the COPY/JUMP and JSR instructions have "reference" versions named: `CREF`/`JREF`/`JSREF` versions that work the same way, but work with pointers

```arm
:data MyLabel 0x1
:data MyLabelPointer 0x1
:data MyDestination 0x1
:data MyDestinationPointer 0x1

LOAD MyLabel MyLabelPointer ; Loads the Address of MyLabel into MyLabelPointer
LOAD MyDestination MyDestinationPointer ; Loads the Address of MyDestination into MyDestinationPointer

COPY MyLabel MyDestination ; Default Copy
CREF MyLabelPointer MyDestinationPointer ; Copy Value in MyLabel to MyDestination by Reference(using pointers)
```

The Same operation can be performed with actual jump labels.
This can be used to get the addresses of functions.
```arm
:data MyFunctionPointer 0x1
:data MyVariable 0x1

LOAD 100 MyVariable ; MyVariable = 100;
LOAD my_function MyFunctionPointer ; Load Address of my_function into the pointer variable


JSR my_function ; Jump to my_function
JSREF MyFunctionPointer ; Jump to subroutine my_function using pointer
; JMP my_function ; Jump Directly without pushing processor state
; JREF my_function ; Jump Directly without pushing processor state

HLT ; Halt Execution 
	; The Processor is falling through the label and starts executing my_function again if we dont Halt.

.my_function linker:hide ; Hides the my_function label from other scripts that might reference this script.
	DEC MyVariable ; MyVariable -= 1;
	RET ; Return from Subroutine
```

### `:const` Labels
Const labels are used to define constant values.
The values are resolved during compilation and are not stored inside the binary as `:data` labels are.
`:const` labels can be used to define address pins of devices or they can be used to define a fixed location in memory that can be written to(outside of the actual binary, as seen in the below example COPY instruction)
```arm
:const MY_CONST 5000 ; Constant Value 5000
:data MyVar 0x1 ; Variable with size 1

LOAD MY_CONST MyVar ; Loads 5000 into MyVar
COPY MY_CONST MyVar ; Copies the value at Address 5000 into MyVar
```

### `:file` Labels
`:data` labels but the content of the label is specified by the referenced file.
Using `:file` will introduce a "hidden" variable that can be used to get the size of the `:file` label at runtime.
```arm
:file MY_DATA "my_data.bin" ; Contains the Content of the File

:data MyDataLength 0x1 ; Helper Variable

COPY MY_DATA_LEN MyDataLength ; uses the Hidden Variable <VAR_NAME>_LEN variable that stores the size of the MY_DATA label.
```
This feature is mainly unused.


## VASM High Level Language (HL)
HL is a language that is meant to provide similar syntax to unsafe C# code or C++.
HL Scripts are compiled into VASM, which is then compiled into Binary.
The Language has implementations for most of the common operators and a very simple (and unstable) Type System that allows for constant data structures.

### Language Features

- Expression Parser
	+ Logic
	+ Math
- Functions with Arguments and Return Values
- Global Level Code
- Data Structures
- Array Accessors
- Public/Private Scopes
- Const and Dynamic Variables(and Strings)
- `if`/`else if`/`else`/`while`/`for` Blocks
- Compiletime Functions
- Custom Importer
- Optimizations
	+ Constant Expression Elimination
	+ Temp Variable Reusing
	+ Expression Strength Reduction
	+ Constant `if` and `while` evaluation
- (In Development) Floating Point Support

### Expression Parser
Example Logic Expressions:

`(a & b) | (c & d)`

`a ^ 1`

`a ^= 1`

Example Math Expressions:

`a %= 50`

`(a + 100) * 15 * b`

### Functions
Functions are declared at the same position as their implementation (C# Like)
```cs
private void MyPrivateFunction()
{
	// DO STUFF

	return; //Can be omitted => Will return undefined value if not specified.
}

private void MyPrivateFunctionArg(uint myArgument)
{
	// DO STUFF WITH ARGUMENT
	return;
}

private uint MyPrivateFunctionArgRet(uint myArgument)
{
	uint ret = myArgument * 2;
	return ret; //Return the Result
}

public void MyPublicFunction() //This Function is visible when exporting the Linker Info
{
	//DO STUFF
}
```

### Global Level Code
Global Level code is written at the root level of the file.
```cs

private void MyFunc()
{
	//Code in here is not global and will be only executed if this function gets invoked
}

//Code outside of functions is compiled into the binary, starting at address 0.
MyFunc(); //This is Global and will get executed if you start execution of the binary at address 0

```

___

Note: Variable Declarations in Global Level are not assigned the correct value until the Global Level Code ran or the variables got assigned from somewhere else.
___

### Data Structures
HL Enables the usage of Constant Data Structures that can be used to make the code more readible.
```cs
private class MyData //Define the Data Type
{
	uint A;
	uint B;
}

private MyData data; //Define Constant Instance of this Data Type

data.A = 123; //Assign Values
data.B = 123;

if(data.A == data.B) //Use Assigned Values
{
	//Do Stuff
}
```

### Array Accessors
```cs

private uint MyArray[1000]; //Array with 1000 Entries

MyArray[0] = 123; //Set First Index
MyArray[999] = 123; //Set Last Index

// MyArray = 123; //Also sets the First Index

uint arrSize = size_of(MyArray); //Using the size_of compiletime function.



```

### Variable Runtime Types
HL Supports 3 Types of Variable Declarations

- Constant Variables(`const` keyword)
- Static Variables(`static` keyword)
- Dynamic Variables(no keyword)

#### Constant Variables
Constant Variables can be used to define Never Changing Values inside the Scripts.
They are primarily used to define static functions and address pins.
```cs

private const uint CONST_VAL = 0xFFFF1001;

CONST_VAL = 123; //Writes 123 to address 0xFFFF1001

private uint val = &CONST_VAL; //Writes 123 to variable val
// the '&' operator is a short hand for the "addr_of()" compiletime function
// the '*' operator is the dereference operator(implicitly used when variable is defined as const)

```

#### Static Variables
Static Variables are currently only used for defining strings.

Strings have to be defined as `static`.

Their length can be obtained with the `size_of(MyString)` Compiletime Function
```cs
private static string MyString = "ABC";
```


### Condition Blocks

HL Implements the most common condition blocks.

- If/Else/Else If
- While/For

### Compiletime Functions

Compiletime Functions can be used like any other Function in the Language. Those functions however will get computed and resolved during the compilation of the Program.

Common Compiletime Functions are:

- addr_of(variable_name)
	+ Returns the Address of the specified variable
- val_of(addr_name)
	+ Returns the Value of the specified Address
- size_of(variable_name)
	+ Returns the Size of the specified Address/Variable
- static_cast(variable_name, TargetType)
	+ Internally Changes the Type of the Variable(not permanent)
- offset_of(variable_name, TargetType)
	+ Returns the Offset from start of the Data Structure to the specified variable
- string(variable_name, "string content")
	+ Defines a string. same as `private static string variable_name = "string content"`
- interrupt(error_code)
	+ Fires an Interrupt with an error code that can be used to change the behaviour of the interupt handler

### Custom Compile Importer

HL implements an Importer Framework that can be used to inject compiler info and files into the Compiler Pipeline.
An example using the `InstructionDataImporter` to dynamically generate functions that perform specific instructions
```cs
//Imports the VASM Instruction HLT
#import "vasm-bridge HLT" 
//The "vasm-bridge" key is used to select the correct importer.
//The Instruction is Exposed through a Label that follows the naming convention:
I0_HLT();
//I{ARG_COUNT}_{INSTRUCTION_KEY}


```

### Optimizations

All the Optimizations are applied during compilation from HL to VASM

#### Constant Expression Elimination

The HL Compiler will detect if an expression is entirely static and can be computed at compile time.
```cs
uint a = 5 + 5
```
Unoptimized:
```arm
:data __a 0x1
:data temp0 0x1
:data temp1 0x1

LOAD 5 temp0
LOAD 5 temp1
ADD temp0 temp1 __a
```
Optimized:
```arm
:data __a 0x1
LOAD 10 __a ; The 5+5 got computed at compiletime
```

#### Reusing Temp Variables

The HL Compiler will reuse temp variables that went out of scope.

```cs
uint a = 5 + 5
```
Unoptimized:
```arm
:data __a 0x1
:data temp0 0x1
:data temp1 0x1

LOAD 5 temp0
LOAD 5 temp1
ADD temp0 temp1 __a
```
Optimized:
```arm
:data __a 0x1
:data temp0 0x1

LOAD 5 temp0;
COPY temp0 __a
LOAD 5 temp0
ADD __a temp0 __a
```




#### Expression Strength Reduction

The HL Compiler will Replace expensive instructions with simpler ones if the outcome is the same.

```cs
uint a = 1;
uint b = a * 2;
```
Unoptimized:
```arm
:data __a 0x1
:data __b 0x1
:data temp0 0x1

LOAD 1 __a
LOAD 2 temp0
MUL __a temp0 __b ; Multiply by 2
```
Optimized:
```arm
:data __a 0x1
:data __b 0x1
:data temp0 0x1

LOAD 1 __a
LOAD 1 temp0
SHL __a temp0 __b ; Replace Multiply by 2 with a Shift Left by 1
```

#### Constant `if` and `while` Evaluation

`while` and `if` branches are evaluated statically if the condition is static.
This removes the need to check the never changing condition at every execution.
All `else` and `else if` blocks after a constant condition evaluated to true are omitted as they never can get entered because of the previous condition beeing true.

```cs
uint a = 1
if(1)
{
	a = 2;
}
```
Unoptimized:
```arm
:data __a 0x1
:data temp0 0x1

LOAD 1 __a
LOAD 1 temp0
.if_start
BNZ temp0 if_end ; Branch to end if temp0 is not 0
LOAD 2 __a ; Load 2 if we didnt jump
.if_end
```
Optimized:
```arm
:data __a 0x1

LOAD 1 __a
.if_start
; The Jump is Ommitted as it was evaluated to "true" during compile time
LOAD 2 __a ; Load 2 (inside the if block)
.if_end
```