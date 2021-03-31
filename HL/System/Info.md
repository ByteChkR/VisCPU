# HL System Library

## Base Classes

### Exception

#### abstract uint GetMessage()
> returns the pointer to the first character of the exception message.
#### abstract uint GetMessageLength()
> returns the length of the exception message.

### ExceptionHandler

#### static void Throw(Exception ex)

> Prints the Exception Message and Interrupts Execution(Including Stacktrace).

_____

### Console

#### static void Clear()
> Clears the Console Output Window.
#### static void Write(uint p, uint l)
> Writes a String located at `p` with length `l` to the console Output.
#### static void WriteLine(uint p, uint l)
> Writes a String located at `p` with length `l` to the console Output followed by a new-line character.
#### static void WriteChar(uint c)
> Writes a Character to the Console Output.
#### static void WriteNumber(uint n)
> Writes a Number to the Console Output.
#### static void WriteNumberLine(uint n)
> Writes a Number to the Console Output followed by a new-line character.
#### static void SetWindowWidth(uint w)
> Sets the Width (in characters) of the output window
#### static uint GetWindowWidth()
> Returns the Width (in characters) of the output window
#### static void SetWindowHeight(uint h)
> Sets the Height (in characters) of the output window
#### static uint GetWindowHeight()
> Returns the Height (in characters) of the output window
#### static void SetBufferWidth(uint w)
> Sets the Width (in characters) of the output Buffer
#### static uint GetBufferWidth()
> Returns the Width (in characters) of the output Buffer
#### static void SetBufferHeight(uint h)
> Sets the Height (in characters) of the output Buffer
#### static uint GetBufferHeight()
> Returns the Height (in characters) of the output Buffer
#### static void SetCursorLeft(uint l)
> Sets the Cursor Position relative to the left side of the window.
#### static uint GetCursorLeft()
> Returns the Cursor Position relative to the left side of the window.
#### static void SetCursorTop(uint t)
> Sets the Cursor Position relative to the top side of the window.
#### static uint GetCursorTop()
> Returns the Cursor Position relative to the top side of the window.
#### static void SetBackgroundColor(uint c)
> Sets the Background Color of the Console Window
#### static uint GetBackgroundColor()
> Returns the Background Color of the Console Window
#### static void SetForegroundColor(uint c)
> Sets the Foreground Color of the Console Window
#### static uint GetForegroundColor()
> Returns the Foreground Color of the Console Window
#### static void ResetColors()
> Resets the Color Code of the Console window to its default values.
#### static uint Read()
> Reads the Next Character from the Console Input, blocking execution if no character is sent.

### Colors
Provides all available Console Colors
#### static uint Black() 

#### static uint DarkBlue() 

#### static uint DarkGreen() 

#### static uint DarkCyan() 

#### static uint DarkRed() 

#### static uint DarkMagenta() 

#### static uint DarkYellow() 

#### static uint Gray() 

#### static uint DarkGray() 

#### static uint Blue() 

#### static uint Green() 

#### static uint Cyan() 

#### static uint Red() 

#### static uint Magenta() 

#### static uint Yellow() 

#### static uint White() 

_____

### Allocator

#### abstract uint Allocate(uint size)
> Allocates a Memory Block with the specified size
#### abstract void Free(uint ptr)
> Releases/Frees the Previously Allocated Pointer.

_____


### Time
Helper Class that provides time Conversions.
All Input Parameters should be in Unix-Seconds Format.

#### uint GetMinutes(uint t)
> Returns the Minutes since 01/01/1970 00:00

#### uint GetHours(uint t)
> Returns the Hours since 01/01/1970 00:00

#### uint GetMinuteInHour(uint t)
> Returns the Minutes of the Current Hour

#### uint GetHourInDay(uint t)
> Returns the Hour of the Day.

#### uint GetSecondInMinute(uint t)
> Returns the Current Seconds in the Current Minute

____

### Thread
Helper Class that provides a way of having delays in the code.

#### void Sleep(uint seconds)
> Spin-Waits for the Amount of seconds to be passed.

____

### SIMD
A Utility Class that allows for Hardware and Software Accelerated IO and Math Operations

#### void Initialize(uint mode)
> Initializes the SIMD Class with the specified BackendMode

#### void AddConstant(uint a, uint c, uint r, uint l)
> Adds a Constant Value `c` to the values of `a` and writing the results into 'r'
> The Length of the "Vectors" `a` and `r` is determined by `l`
#### void Add(uint a, uint b, uint r, uint l)
> Adds the Values of `b` to the values of `a` and writing the results into 'r'
> The Length of the "Vectors" `a`, `c` and `r` is determined by `l`
#### void SubConstant(uint a, uint c, uint r, uint l)
> Subtracts a Constant Value `c` to the values of `a` and writing the results into 'r'
> The Length of the "Vectors" `a` and `r` is determined by `l`
#### void Sub(uint a, uint b, uint r, uint l)
> Subtracts the Values of `b` to the values of `a` and writing the results into 'r'
> The Length of the "Vectors" `a`, `c` and `r` is determined by `l`
#### void MulConstant(uint a, uint c, uint r, uint l)
> Multiplys a Constant Value `c` with the values of `a` and writing the results into 'r'
> The Length of the "Vectors" `a` and `r` is determined by `l`
#### void Mul(uint a, uint b, uint r, uint l)
> Multiplys the Values of `b` with the values of `a` and writing the results into 'r'
> The Length of the "Vectors" `a`, `b` and `r` is determined by `l`
#### void DivConstant(uint a, uint c, uint r, uint l)
> Divides the values of `a` by a Constant Value `c` and writing the results into 'r'
> The Length of the "Vectors" `a` and `r` is determined by `l`
#### void Div(uint a, uint b, uint r, uint l)
> Divides the Values of `a` by the values of `b` and writing the results into 'r'
> The Length of the "Vectors" `a`, `b` and `r` is determined by `l`
#### void DivConstant(uint a, uint c, uint r, uint l)
> Modulos the values of `a` by a Constant Value `c` and writing the results into 'r'
> The Length of the "Vectors" `a` and `r` is determined by `l`
#### void Div(uint a, uint b, uint r, uint l)
> Modulos the Values of `a` by the values of `b` and writing the results into 'r'
> The Length of the "Vectors" `a`, `b` and `r` is determined by `l`

#### void Set(uint a, uint v, uint l)
> Sets the Values of `a` to the constant value of `v` 
> The Length of `a` is determined by `l`
#### void Copy(uint a, uint b, uint l)
> Copies the Values of `a` into `b` 
> The Length of `a` and `b` is determined by `l`
#### void Move(uint a, uint b, uint l)
> Moves the Values of `a` into `b` 
> The Length of `a` and `b` is determined by `l`
#### void Swap(uint a, uint b, uint l)
> Swaps the Values of `a` and `b` 
> The Length of `a` and `b` is determined by `l`


### SIMDBackend
Defines the backend modes of the SIMD Utility

#### uint Software()
#### uint Hardware()



### MemoryRegion
Helper Class that uses SIMD to accelerate IO operations

#### MemoryRegion(uint p, uint l)
> Constructor.

#### void Set(uint addr, uint value)
> Sets a Value at a specific address.

#### uint Get(uint addr)
> Returns the Value at the specified address.

#### uint Start()
> Returns the Start Pointer of the region

#### uint Length()
> Returns the Length of the Region.

#### void CopyTo(MemoryRegion other)
> Copies all Values in this Region to the `other` region.

#### void MoveTo(MemoryRegion other)
> Copies all Values in this Region to the `other` region.

#### void Swap(MemoryRegion other)
> Swaps all values between this Region and the `other` Region.

#### void EnableRangeCheck(uint toggle)
> Enables or Disables Range Checking.