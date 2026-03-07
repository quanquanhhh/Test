# FIRST_TASK.md

## First Implementation Task

Implement the base runtime architecture first.

### Step 1. Create runtime config structure
Create a runtime config structure for YooAsset related settings.

It should include things like:
- package name
- play mode selection if needed
- host server URL
- fallback host server URL if needed

Do not hardcode host URLs directly in unrelated game logic.

---

### Step 2. Create centralized YooAsset initialization flow
Implement a centralized initialization flow for YooAsset.

Requirements:
- support Editor Simulate Mode
- support Offline Play Mode
- support Host Play Mode
- use config-driven host URL
- add logs for selected mode and resolved URL

---

### Step 3. Create resource management module
Create a project-level resource manager / asset module.

The module should expose at least:
- `LoadAsync<T>(string location)`
- `InstantiateAsync(string location)` if appropriate
- `GetDownloadSizeByTagAsync(string tag)` or equivalent
- `DownloadByTagAsync(string tag)` or equivalent

Important:
If direct load-by-tag is not correct for YooAsset runtime semantics, do not fake it.
Use tags for download/preload grouping and use location/name for actual asset load unless the project already defines another mapping.

---

### Step 4. Add usage instructions
After implementation, explain:
- created files
- why each file exists
- how to initialize the system
- how gameplay code should call the wrapper

---

## Deliverable Style

Respond with:
1. design summary
2. file list
3. complete code
4. usage steps
5. risks / assumptions
6. manual verification checklist