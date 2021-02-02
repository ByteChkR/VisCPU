
# VASM High Level Language (HL)
HL is a language that is meant to provide similar syntax to unsafe C# code or C++.
HL Scripts are compiled into VASM, which is then compiled into Binary.
The Language has implementations for most of the common operators and a very simple (and unstable) Type System that allows for constant data structures.

## Language Features

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

## Expression Parser
Example Logic Expressions:

`(a & b) | (c & d)`

`a ^ 1`

`a ^= 1`

Example Math Expressions:

`a %= 50`

`(a + 100) * 15 * b`

## Functions
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

## Global Level Code
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

Note: It is possible to invoke the Global scope with a const value pointer

```cs
//Import the JREF Instruction to be able to run the entry without returning
//We need to use JREF instead of JMP because of the inner workings of the Importer.
//The arguments that are passed into the I0_JREF function are copied to another variable.
//Using JMP here would result in a segfault as we are directly jumping to the address of the argument variable.
#import "vasm-bridge JREF"

private const uint ENTRY_PTR = 0;


private void BackToMainSubroutine()
{
	ENTRY_PTR(); //Jumps to Global Entry. Important: The Invocation will happen as subroutine. 
	//Once the Entry Point Finished, the program flow will return to here.
}

private void BackToMainDirect()
{
	I1_JREF(0); //Jump to Entry Pointer.
	//This does not return. It will not write any processor state to the stack.
}
```

___

## Data Structures
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

## Array Accessors
```cs

private uint MyArray[1000]; //Array with 1000 Entries

MyArray[0] = 123; //Set First Index
MyArray[999] = 123; //Set Last Index

// MyArray = 123; //Also sets the First Index

uint arrSize = size_of(MyArray); //Using the size_of compiletime function.



```

## Variable Runtime Types
HL Supports 3 Types of Variable Declarations

- Constant Variables(`const` keyword)
- Static Variables(`static` keyword)
- Dynamic Variables(no keyword)

### Constant Variables
Constant Variables can be used to define Never Changing Values inside the Scripts.
They are primarily used to define static functions and address pins.
```cs

private const uint CONST_VAL = 0xFFFF1001;

CONST_VAL = 123; //Writes 123 to address 0xFFFF1001

private uint val = &CONST_VAL; //Writes 0xFFFF1001 to variable val
// the '&' operator is a short hand for the "addr_of()" compiletime function
// the '*' operator is the dereference operator(implicitly used when variable is defined as const)
// also available as "val_of()" compile time function

```

### Static Variables
Static Variables are currently only used for defining strings.

Strings have to be defined as `static`.

Their length can be obtained with the `size_of(MyString)` Compiletime Function
```cs
private static string MyString = "ABC";
private uint str_length = size_of(MyString);
```


## Condition Blocks

HL Implements the most common condition blocks.

- If/Else/Else If
- While/For 

### If conditions 
If conditions are used to conditionally change the execution flow through the binary. 

#### `if` keyword
If blocks start with the keyword `if` followed by the condition in brackets, followed by the code block to execute if the condition evaluates to "true"

```cs 
uint a = 1;
if ( a == 1 )
{
	//a is equal to 1.
	//do stuff
}
```

#### `else` keyword 
The keyword `else` is always preceded by an `if` or `else if`. If none of the previous conditions were met, the else code block after the keyword is executing. 

#### `else if` keyword 
The combined keyword `else if` allows for extending if branches with multiple different conditions

### While Loops 
When using a `while` loop, the CPU enters a code block that will execute until the specified condition will evaluate to false. 

``` cs
private uint a = 0;
while (a < 1000 )
{
	a++; //Increment A
}
```

### For Loops 
For loops are a variation of while loops that aim to make it easier to write repeating code. 
A for loop enables writing simpler code with less likeliness to create an endless loop by introducing a counter variable. 
It takes 3 expressions separated by semicolons. 
The first expression is executed when entering the block, it typically declares the counter variable. 
The second expression is evaluated before the code block is executed. If the second expression is true, the program flow will enter the code block. 
If the expression is false, the execution continues after the for loop. 
The third expression is executed after the code block. It is typically used to change the counter value. 

```cs 
for(uint i = 0; i < 100; i++)
{
	//Do stuff 100 times. 
}
//if i >= 100 the execution continues here. 
```

For loops can be directly converted to while loops. 
The following example exhibits the same behaviour as a for loop. 

```cs 
uint i = 0;
while(i < 100)
{
	//Do Stuff 100 times 
i++;
}
```

## Compiletime Functions

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

## Custom Compile Importer

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

## Optimizations

All the Optimizations are applied during compilation from HL to VASM. 
All VASM examples are approximate handwritten code that omits details for clarity reasons. The actual compiled output could look different. 

### Constant Expression Elimination

The HL Compiler will detect if an expression is entirely static and can be computed at compile time.
```cs
uint a = 5 + 5
```
Unoptimized:
```asm
:data __a 0x1
:data temp0 0x1
:data temp1 0x1

LOAD 5 temp0
LOAD 5 temp1
ADD temp0 temp1 __a
```
Optimized:
```asm
:data __a 0x1
LOAD 10 __a ; The 5+5 got computed at compiletime
```

### Reusing Temp Variables

The HL Compiler will reuse temp variables that went out of scope.

```cs
uint a = 5 + 5
```
Unoptimized:
```asm
:data __a 0x1
:data temp0 0x1
:data temp1 0x1

LOAD 5 temp0
LOAD 5 temp1
ADD temp0 temp1 __a
```
Optimized:
```asm
:data __a 0x1
:data temp0 0x1

LOAD 5 temp0;
COPY temp0 __a
LOAD 5 temp0
ADD __a temp0 __a
```




### Expression Strength Reduction

The HL Compiler will Replace expensive instructions with simpler ones if the outcome is the same.

```cs
uint a = 1;
uint b = a * 2;
```
Unoptimized:
```asm
:data __a 0x1
:data __b 0x1
:data temp0 0x1

LOAD 1 __a
LOAD 2 temp0
MUL __a temp0 __b ; Multiply by 2
```
Optimized:
```asm
:data __a 0x1
:data __b 0x1
:data temp0 0x1

LOAD 1 __a
LOAD 1 temp0
SHL __a temp0 __b ; Replace Multiply by 2 with a Shift Left by 1
```

### Constant `if` and `while` Evaluation

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
```asm
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
```asm
:data __a 0x1

LOAD 1 __a
.if_start
; The Jump is Ommitted as it was evaluated to "true" during compile time
LOAD 2 __a ; Load 2 (inside the if block)
.if_end
```
