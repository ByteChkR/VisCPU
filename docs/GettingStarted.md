# VisCPU First Steps

How to get started with the Vis Console SDK Tools

1. Build `VisCPU.Console` from Git Repository
2. (Optional) add vis executable to PATH
3. Navigate into an empty folder and execute `vis project new` to create a new project with all default values.
4. Write Code with the Project Folder as Root Directory.
5. Compile Project with `vis project make . Debug` or `vis project make . Release`
6. Run Compiled Program with `vis project make . DebugRun` or `vis project make . ReleaseRun`
7. Publish Project to Local Package Repository with `vis project publish` or using the Build System `vis project make . Publish`