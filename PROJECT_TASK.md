# PROJECT_TASK.md

## Goal

Implement a production-ready YooAsset-based asset pipeline for this Unity project.

## Required Features

### 1. Runtime remote hot update
The game must be able to load bundles from a configurable remote URL.

Requirements:
- host URL comes from config
- supports host play mode
- future-friendly for fallback URL or multiple environments

### 2. Resource management module
Create or improve a unified project-level resource manager.

Requirements:
- wrap YooAsset usage
- expose load-by-name APIs
- expose tag-based download / preload APIs
- upper-layer code should use this wrapper instead of raw YooAsset calls

### 3. Builtin packaging tool
Create an editor tool that copies builtin package data into `StreamingAssets`.

Requirements:
- intended builtin content is identified correctly
- output is deterministic
- logs are clear
- tool is available from Unity menu

### 4. Full bundle rebuild tool
Create an editor tool that rebuilds the full bundle output.

Requirements:
- clean build output
- correct package name and target handling
- clear logs
- Unity menu entry

### 5. S3 upload tool
Create an editor tool that uploads built bundles to a provided S3 location.

Requirements:
- configurable bucket / region / path
- no hardcoded credentials
- clear progress and result logs

---

## Important Clarification About Tags

This project uses the word "tag" in requirements, but implementation must distinguish carefully between:

- runtime asset loading by asset name / location
- runtime download grouping by tag
- build-time tag usage for bundle grouping / builtin selection

Do not assume these are the same thing.
If direct "load by tag" is not a natural YooAsset runtime behavior, implement a correct wrapper strategy and explain it clearly.

---

## Preferred Result

The implementation should produce:

- one centralized YooAsset init flow
- one centralized resource module
- one editor tool for builtin copy into `StreamingAssets`
- one editor tool for full rebuild
- one editor tool for S3 upload
- clear usage instructions

---

## Constraints

- keep changes focused
- preserve current project naming style
- avoid overengineering
- prefer practical Unity production code