# VisCPU
A simple Virtual Machine/CPU Emulator that operates on 32-bit unsigned numbers with an instruction width of 4.
Utilities and Example scripts can be found in the [VisOS Repository](https://github.com/ByteChkR/visos)

## Contents
List of Projects in this Repository

### VisCPU
Core Library Implementing the CPU/MemoryBus and Peripheral base implementations

### VisCPU.Compiler
Compiler/Linker for VASM Assembly Language

### VisCPU.Utility
Utility Library that contains Shared Types and Systems

### VisCPU.HL
High Level C-style language crosscompiler to VASM

### VisCPU.Instructions
Default Instruction Set Implementation

### VisCPU.Peripherals
Optional Peripheral Implementations
- Benchmark Device
- Console Input Device
- Console Output Device
- Console Management Device
- Host File System Device

### VisCPU.ProjectSystem
Simple Build/Project/Repository System used to work with VASM and HL Projects

### VisCPU.HL.Integration
Helper Library that aims to make it easier to create interopabilty to C#

### VisCPU.HL.Integration.Unity
Work in progress Integration Library for the Unity Game Engine

### VisCPU.Extensions
Extension System that dynamically loads 3rd party libraries for extending the instruction set or peripherals

### VisCPU.Console.Core
CLI Implementation that implements all Commands in a syntax similar to the 'dotnet' command.

### VisCPU.Console
.NET 5 Console Application that invokes the Console Core.
