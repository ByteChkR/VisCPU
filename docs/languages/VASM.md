# Variable Assembly Language (VASM)

VASM is an Assembly Language that can (mostly) be translated 1:1 into machine instructions

The Language compiler has 2 main steps

- Linker
- Assembler

## The VASM Linker
The Linker is responsible for processing included files and fixing the address layout of included files. So that labels that are defined in other files can be used.
By Default all Defined Labels are usable in all scripts.
To hide labels from other files, the labels can be decorated with `linker:hide`.

## The VASM Assembler
Generates the Binary Data from the Intermediate Result of the Linker.
The Assembler ajusts the Addresses so that the Linked assembly data can be merged into a single executable binary.

## Language Features
The VASM Language is meant to be a close representation of the actual binary code.
Instructions are selected by Specifying the Mnemonic followed by the arguments for the Instruction.
```asm
:data MyData 0x01 ; MyData is a label that has a size of 1.

LOAD 1 MyData ; Loads the Value 1 into the MyData Label
ADD MyData MyData ; Adds MyData onto itself. => MyData = MyData + MyData
ADD MyData MyData MyData ; Same as above. But specifies the Result Address.
INC MyData ; Increments the MyData label by 1
```

## `:data` Labels
`:data` labels are used to define a section of memory that can be used by specifying the name of the data section.

The Actual Memory Layout is handled by the Compiler. The Actual Addess in memory can be loaded by treating the label as "literal value"
```asm
:data MyLabel 0x1 ; No Initialization

:data MyData 0x1 0xFFFF ; MyData is Initialized with the Value 0xFFFF
:data MyString "VASM Supports Strings!" ; MyString is initialized with the string data. (size is equal to string length)

:data MyLabelPointer 0x1

LOAD MyLabel MyLabelPointer ; Loads the Address of MyLabel into MyLabelPointer
```

Because of the abilty to load pointers and perform operations on them, the COPY/JUMP and JSR instructions have "reference" versions named: `CREF`/`JREF`/`JSREF` versions that work the same way, but work with pointers

```asm
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
```asm
:data MyFunctionPointer 0x1
:data MyVariable 0x1

LOAD 100 MyVariable ; MyVariable = 100;
LOAD my_function MyFunctionPointer ; Load Address of my_function into the pointer variable


JSR my_function ; Jump to my_function
JSREF MyFunctionPointer ; Jump to subroutine my_function using pointer
; JMP my_function ; Jump Directly without pushing processor state
; JREF MyFunctionPointer ; Jump Directly without pushing processor state

HLT ; Halt Execution 
	; The Processor is falling through the label and starts executing my_function again if we dont Halt.

.my_function linker:hide ; Hides the my_function label from other scripts that might reference this script.
	DEC MyVariable ; MyVariable -= 1;
	RET ; Return from Subroutine
```

## `:const` Labels
Const labels are used to define constant values.
The values are resolved during compilation and are not stored inside the binary as `:data` labels are.
`:const` labels can be used to define address pins of devices or they can be used to define a fixed location in memory that can be written to(outside of the actual binary, as seen in the below example COPY instruction)
```asm
:const MY_CONST 5000 ; Constant Value 5000
:data MyVar 0x1 ; Variable with size 1

LOAD MY_CONST MyVar ; Loads 5000 into MyVar
COPY MY_CONST MyVar ; Copies the value at Address 5000 into MyVar
```

## `:file` Labels
`:data` labels but the content of the label is specified by the referenced file.
Using `:file` will introduce a "hidden" variable that can be used to get the size of the `:file` label at runtime.
```asm
:file MY_DATA "my_data.bin" ; Contains the Content of the File

:data MyDataLength 0x1 ; Helper Variable

COPY MY_DATA_LEN MyDataLength ; uses the Hidden Variable <VAR_NAME>_LEN variable that stores the size of the MY_DATA label.
```
This feature is mainly unused.

