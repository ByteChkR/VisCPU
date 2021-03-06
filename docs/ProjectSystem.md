# VisCPU Projects
Set up custom Project Build Targets and view default values.

## The Default `project.json` File

The `project.json` file is generated by the SDK when a new project gets created.
It will reference (common) files from the SDK installation directory.
Aside from Name/Version and Dependencies, the `project.json` contains 5 Different Default Build Targets.

- Debug
	+ Builds Project with no optimizations
	+ Common Path: $(VISDIR)/common/jobs/debug_build.json
- Release
	+ Builds Project with all optimizations
	+ Common Path: $(VISDIR)/common/jobs/release_build.json
- DebugRun
	+ Runs `Debug` target before running the resulting binary
	+ Common Path: $(VISDIR)/common/jobs/debug_run.json
- ReleaseRun
	+ Runs `Release` target before running the resulting binary
	+ Common Path: $(VISDIR)/common/jobs/release_run.json
- Publish
	+ Publishes the Project to the local package store.
	+ Common Path: $(VISDIR)/common/jobs/newVersion.json
	+ Common Path: $(VISDIR)/common/jobs/publish.json
	+ Common Path: $(VISDIR)/common/jobs/restore.json


### Project Build Targets
Build Targets have a Name and Dependencies that are required to run before the current target can be run.
Targets consist out of one or more jobs that will be carried out in order.

#### Build Target Jobs
Jobs that make up Build Targets have a Name and a list of Depending Targets that have to be completed beforehand.
Each job is referencing a BuildRunner by name. The Build runner then gets executed with the arguments specified in the Job Settings.

____

Note: In the Default `project.json` the Jobs are merged with the common build jobs located at the SDK installation directory to keep the `project.json` content as small as possible. Values can be overridden by specifying them in the argument of the Merge Job.

____

### Publishing Projects
With `vis project publish` or `vis project make . Publish` the target project is published to the default project repository.
This is meant to be used to better structure the different projects and reference them without having duplicate code.
All currently available packages from all origins can be listed with the command `vis origin packages`.

### Referencing Projects
To Reference other Projects the project reference has to be added to the `project.json` file.
Do this by hand or with the command `vis project add . -name <ProjectName> -version <Version>`. -name and -version flags are optional and can be changed directly in the `project.json` file.

____

Note: if no version is specified the newest version will be taken.

____

Once the reference has been added, the dependency can be resolved with the command `vis project restore`, the Referenced project will now be available in the project root and can be included by the code.