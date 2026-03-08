# SECOND_TASK.md

## Second Implementation Task

Implement the **editor tooling layer** for the YooAsset pipeline.

The first task already created the runtime foundation:
- runtime config
- YooAsset initialization flow
- project-level resource module / asset module
- tag-based download or preload wrapper where appropriate

This second task should focus on **editor-side production tooling**.

---

## Main Goals

### Goal 1. Builtin package content into StreamingAssets

Create a Unity editor tool that prepares builtin package content for app installation.

Requirements:
- identify builtin content according to the project's actual YooAsset build/tag rules
- build or collect the correct package output
- copy the correct generated files into `StreamingAssets`
- make the result deterministic and safe to run multiple times
- log copied file count, source path, destination path, and package name

Important:
- do not fake builtin handling
- do not copy random source assets by filename
- builtin selection must match actual YooAsset build output and the project's tag conventions
- if the project uses a tag such as `buildin`, verify how that tag maps to real build output before implementing the copy logic

Deliverable direction:
- one Unity menu entry for builtin preparation
- clear logs
- reusable helper methods if appropriate

---

### Goal 2. Full bundle rebuild tool

Create a Unity editor tool that rebuilds the full bundle output.

Requirements:
- use the correct package name
- use the correct build target
- generate a fresh output
- clear or isolate old output safely
- log build summary and output location
- keep the tool safe to run multiple times

Deliverable direction:
- one Unity menu entry for full rebuild
- build output path should be clear in logs
- separate build logic from copy logic when practical

---

### Goal 3. Clean editor/runtime separation

Requirements:
- keep editor-only code in editor-safe folders or assemblies
- do not mix editor utilities into runtime modules
- reuse runtime/package config from the first task when appropriate
- keep APIs and naming aligned with the current project style

---

## Optional Extension Point Only

Do not fully implement S3 upload in this step unless it is needed only as a clean extension point.

Allowed in this step:
- define upload config structure
- define interface or placeholder service boundary if useful

Not required in this step:
- complete S3 upload implementation
- credential wiring
- full deployment pipeline

The focus of step 2 is:
1. builtin copy to `StreamingAssets`
2. full rebuild tool

---

## Important YooAsset Clarifications

Be careful with the meaning of "tag".

This project may use tags in different ways:
- build-time tag or collector tag
- runtime downloader tag
- builtin selection convention

Do not assume these are the same thing.

If direct "load by tag" is not a natural YooAsset runtime behavior, do not force that concept into editor tooling.
For this step, use tags only where they correctly map to actual YooAsset packaging/build output.

Builtin handling must be based on actual built package data, not guessed source files.

---

## Deliverables

Respond with:

1. design summary
2. files created or modified
3. complete code
4. how to use the tools from Unity Editor
5. assumptions / risks
6. manual verification checklist

---

## Verification Expectations

The result should make it possible to verify:

- builtin package content can be prepared into `StreamingAssets`
- the full bundle output can be rebuilt from Unity Editor
- logs clearly show package name, build target, output path, copied file count, and failure reasons
- the tools are re-runnable without producing confusing side effects

---

## Constraints

- keep changes focused
- avoid overengineering
- preserve existing naming conventions
- do not modify `.meta` files unless absolutely necessary
- do not replace YooAsset with another loading system
- do not stop at analysis only; provide concrete implementation