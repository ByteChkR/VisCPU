# OS Notes

## BootImage Flow
	1. Start VM
	2. Detect Drive
	3. Try Read Magic Values
	4. Load Image from Disk
		4.1. Apply possible PIC Algorithms
	5. Hand Over Control

## OS Flow
	1. Get Control
	2. Find Devices
	3. Find Drivers
	4. Setup Driver/Device Pairs
	5. Detect Disk Format
	6. Read/Load File System Data
		6.1. File System Formats are initialized here.
		6.2. Do File System Check for errors
	7. Initialize IO API with the detected devices/formats

## OS Start Flow
	1. Initialize Event System
	2. Read Config Data
	3. Process Auto Start
	4. Start Console APP
	5. Fire "OSStartEvent"
		5.1. Use this event to signal apps that the OS is now fully operational
	6. Start Update Loop
		6.1. Pump the Event Queue(e.g. Process the OSStartEvent and others)

## OS Structures
### OS
Static Class that is used as "Root" object.

#### Update()
Process Events that were Enqueued. 
Call OnEvent on all Apps with proper EventMask
#### Enqueue(uint* event)
Enqueues a Event

### App
Class that implements following Members.
This Class is used by the operating system as container for custom "non-OS" code such as apps.
#### Main(uint* args, uint argl)
Entry Point of the App. Passing the Startup Arguments as in C++
#### OnEvent(uint* event)
Gets Invoked when a Event that got fired fits the Event Mask of the App
#### GetEventMask()
Returns the Event Mask of the App.
This masks allows to selectively ignore events.
### Event
Event Class that is the abstract base of all OS Events.
They do not have functionality in them, they are used as data objects to invoke functionality in one or more Apps
#### GetEventType()
Returns the Event Type that is compared against the App.GetEventMask value
#### Disarm()
Disables the Event.
(gets called once an app has picked up on the event and is processing it.)