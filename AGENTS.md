# AGENTS.md

## Project Overview

This is a Unity project using **YooAsset** as the asset packaging, content delivery, and hot-update system.

The project needs a production-ready asset pipeline with the following goals:

1. The game can load remote bundles from a configurable URL for hot update.
2. The project has a unified resource management module that wraps YooAsset usage.
3. The resource management module supports loading assets by name, and supports tag-based download / preload workflows where appropriate.
4. The project includes an editor tool to package builtin content into `StreamingAssets`.
5. The project includes an editor tool to rebuild full bundles.
6. The project includes an editor tool to upload generated bundle output to a provided S3 target.

This is a practical Unity production project.
Prefer maintainable, directly usable implementations over abstract demos.

---

## Important Implementation Principles

- Prefer minimal and focused changes.
- Do not rewrite the whole project unless explicitly asked.
- Reuse existing naming style, folder structure, namespace style, and module patterns.
- Keep editor-only code separate from runtime code.
- Keep YooAsset initialization centralized.
- Avoid scattering raw YooAsset API calls across gameplay/UI/business code.
- Add clear logs around initialization, build, copy, download, and upload.
- When uncertain, make reasonable assumptions and continue with implementation instead of stopping at analysis.

---

## High Priority Architecture Goals

### 1. Runtime hot update via configurable remote URL

The game must support remote asset loading via configurable URL.

Requirements:
- Do not hardcode host URLs in gameplay code.
- Prefer one central config source for package name, host URL, fallback URL, and build target related data.
- Host play mode must resolve URL from config.
- The design should support environment switching later if needed.

Example concept:
- package name
- main host server
- fallback host server
- static version URL if needed
- manifest URL rules if needed

---

### 2. Unified resource management module

Create or improve a project-level resource management module that wraps YooAsset.

Important rule:
**Gameplay code should not directly depend on YooAsset package operations if it can be avoided.**
Use a project-level module or service instead.

The module should support:
- initialization
- load asset by name
- load asset async
- instantiate prefab async if suitable
- load scene async if needed
- get downloader for tag
- get download size for tag
- trigger tag download workflow
- expose clean and stable APIs for upper layers

Preferred design direction:
- `AssetModule`, `ResourceModule`, or project-equivalent naming
- one central entry point
- clearly separated init / update / download / load responsibilities

---

### 3. Builtin content packaging into StreamingAssets

Create an editor tool that can package builtin content into `StreamingAssets`.

This is extremely important:

#### Correct understanding
Builtin content means:
- content that should exist in the app package at install time
- content copied into `StreamingAssets`
- content available for offline/local bootstrap before remote download

#### Expected implementation direction
The tool should:
- build or prepare YooAsset player data correctly
- identify content intended as builtin
- copy the correct output into `StreamingAssets`
- keep result deterministic
- log exactly what was copied

#### Important warning
Do not implement fake builtin logic by randomly searching arbitrary file names.
Builtin handling must match the actual YooAsset build output and project packaging rules.

If the project uses tag-based conventions such as `buildin`, use them consistently and trace how that tag maps to actual build result and copied files.

---

### 4. Full bundle rebuild editor tool

Create an editor tool that:
- rebuilds the full bundle set
- uses the correct package name and build target
- clears or isolates old output safely
- generates a fresh output
- logs build summary and output location

The build tool must be callable from Unity Editor menu and should be repeatable.

---

### 5. S3 upload automation tool

Create an editor automation tool that:
- uploads bundle output to S3
- uses configurable bucket / key prefix / region / endpoint settings
- logs upload progress and summary
- clearly reports failures

Never hardcode credentials in source code.

Preferred credential sources:
1. environment variables
2. ignored local config file
3. editor input that is not committed

Keep upload logic isolated from build logic.
Do not tightly couple build and upload in one untestable method.

---

## YooAsset-Specific Rules

These rules are critical. Do not violate them.

### A. Distinguish the play modes clearly

The runtime system must clearly distinguish:
- Editor Simulate Mode
- Offline Play Mode
- Host Play Mode

Do not mix their initialization logic carelessly.

### B. Centralize package initialization

There should be one clear runtime initialization flow for YooAsset package setup.

Avoid:
- multiple unrelated init paths
- duplicated init logic in launcher/gameplay/UI
- package name scattered across files

### C. Understand tag semantics correctly

This project may use tags in two related but different meanings:

1. **Build-time tagging / collection tagging**
   - used to organize assets into bundle groups or build rules
   - may affect what gets built, grouped, or copied

2. **Runtime tag-based download**
   - used with YooAsset downloader APIs to download bundles associated with a tag

Do not assume these are interchangeable without checking the actual YooAsset workflow used by the project.

Before implementing tag-related logic:
- verify how tags are assigned in the project
- verify whether the tag is attached to assets, collectors, or bundle build output
- verify whether runtime download-by-tag maps to the intended built bundles

### D. Do not fake "load by tag"

Usually, **assets are loaded by location / asset name / address-like identifier**, while **tags are commonly used for download grouping**, not necessarily as direct asset load keys.

Therefore:
- do not pretend that "load asset by tag" means "find an asset with tag and load it directly" unless the project explicitly defines such a mapping
- if the requirement says "load bundle by tag", prefer implementing tag-based download / prepare / preload workflows
- if actual direct asset loading by tag is needed, define a clear mapping strategy instead of guessing

### E. Builtin tag handling must match build output

If the project uses a tag like `buildin`, do not just filter source files by string match and copy random files.
Instead:
- align builtin selection with YooAsset packaging/build output
- copy the correct generated files required for runtime package initialization
- include manifests and dependent files as needed by the actual chosen YooAsset workflow

### F. Preserve YooAsset workflow realism

When writing code:
- prefer actual YooAsset operations and build pipeline flow
- do not replace YooAsset with Resources loading or AssetDatabase loading in runtime systems
- do not introduce placeholder logic that only works in editor unless clearly marked

---

## Editor Tool Rules

Editor tools must follow these rules:

- Put editor-only code into editor-safe folders/assemblies.
- Expose tools via clear Unity menu items.
- Log:
  - selected package
  - build target
  - output path
  - copied file count
  - upload target
  - failure reason
- Fail loudly with actionable error messages.
- Be safe to run multiple times.
- Avoid hidden destructive behavior.

Preferred UI forms:
- menu items for quick actions
- editor window for configurable operations
- ScriptableObject config asset for reusable settings if appropriate

---

## S3 Upload Rules

- Do not hardcode AWS access key, secret key, session token, bucket, or private endpoint secrets into source code.
- Prefer:
  - `AWS_ACCESS_KEY_ID`
  - `AWS_SECRET_ACCESS_KEY`
  - `AWS_SESSION_TOKEN` if needed
  - region from environment or config
  - bucket/path from config or editor input
- Log target bucket and prefix before upload.
- Log file count and result summary after upload.
- If partial upload fails, report which files failed if possible.
- Keep upload code replaceable and isolated.

---

## Coding Style Expectations

- Follow the project's existing style.
- Keep methods readable and explicit.
- Prefer small clear helper methods.
- Avoid clever abstractions.
- Do not introduce giant generic frameworks.
- Prefer production-grade C# over pseudo-code.
- Use async patterns where appropriate, but do not overcomplicate the call chain.
- Add null checks and defensive logs in risky init/build/upload paths.

---

## Folder / Naming Preference

Reuse existing project conventions if present.

If the project already uses names like:
- Foundation
- Core
- Modules
- Launcher
- Editor
- Config
- Manager

then follow those patterns instead of inventing a new architecture.

Possible acceptable class names:
- `YooAssetSettings`
- `YooAssetRuntimeConfig`
- `AssetModule`
- `ResourceModule`
- `YooAssetInitializer`
- `BuiltinBundleBuilder`
- `YooAssetBuildTool`
- `BundleUploadService`
- `S3UploadConfig`

These are examples only. Prefer actual project naming style.

---

## Expected Deliverable Format

When implementing a task, respond in this order:

1. brief design summary
2. list of files created or modified
3. complete code
4. how to use from Unity Editor or runtime
5. assumptions / risks
6. manual verification steps

When changing existing files, always explain:
- why the file changed
- whether public API changed
- whether manual Unity verification is required

---

## Preferred Task Order

For this repository, prefer implementing work in this order:

1. runtime config structure for YooAsset settings
2. YooAsset initialization flow
3. resource management module
4. builtin packaging tool for StreamingAssets
5. full rebuild tool
6. S3 upload integration
7. validation logs and usage docs

Avoid one giant refactor if a staged implementation is more reliable.

---

## What To Avoid

- Do not modify `.meta` files unless absolutely necessary.
- Do not casually rename namespaces, folders, or major classes.
- Do not replace YooAsset with another loading system.
- Do not scatter host URL logic across unrelated files.
- Do not hardcode secrets.
- Do not create fake tag logic that does not match actual YooAsset behavior.
- Do not mix runtime code and editor code unsafely.
- Do not stop after analysis only; provide concrete implementation.

---

## Validation Checklist

A valid implementation should make it possible to verify:

- YooAsset package initializes successfully
- remote host URL comes from config
- asset can be loaded by name through project wrapper
- tag-based download/preload can be triggered through project wrapper
- builtin content is copied into `StreamingAssets` through editor tool
- full bundle rebuild completes through editor tool
- generated output can be uploaded to S3 through editor tool
- logs clearly show what happened

---

## User Preference

The user prefers:
- ready-to-use C# code
- practical Unity editor tools
- minimal but complete architecture
- solutions that fit directly into a production Unity project
- concrete integration guidance rather than abstract discussion