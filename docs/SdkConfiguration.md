# VisSDK Configuration
The VisSDK has a common configuration directory that can be used by the core implementation as well as extensions.
The config directory is structured by `sdk` and `cpu` configs.

- sdk
	+ compiler
		* Vasm and HL Compiler Settings
	+ module
		* Resolver Settings/Origins
	+ projects
		* Local Repository Data
	+ cli.json
		* Commandline Settings
- cpu
	+ cpu.json
		* General CPU Settings
	+ memory-bus.json
		* Memory Bus Settings/Memory Device List
	+ peripherals
		* Configuration directory for peripheral settings
	+ extensions
		* Directory containing additional instructions and peripherals that can be used.