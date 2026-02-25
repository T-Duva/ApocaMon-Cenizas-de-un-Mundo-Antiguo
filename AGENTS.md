# AGENTS.md

## Project overview

**ApocaMon: Cenizas de un Mundo Antiguo** — a 2D Tower Defense / "Mazing TD" game built in **Unity 6** (`6000.0.66f1`). Post-apocalyptic creature-collection theme with 19 elemental clans, D6 stat generation, wave spawning on NavMesh, and defender placement.

Four scenes: `01_MenuPrincipal` → `02_MapaSeleccion` (Desfile) → `03_Batalla` → `04_Tienda` (stub).

All game code lives in `Assets/_Scripts/` (13 custom C# scripts). No backend services, databases, or external APIs.

## Cursor Cloud specific instructions

### Prerequisites already installed in the VM snapshot

- **Unity Hub** (apt package `unityhub`) with CLI at `/usr/bin/unityhub`
- **Unity Editor 6000.0.66f1** at `/home/ubuntu/Unity/Hub/Editor/6000.0.66f1/Editor/Unity`
- **.NET SDK 8.0** (`dotnet`) for out-of-editor C# compilation and linting

### Unity license (REQUIRED before any editor operation)

Unity Personal licenses **cannot** be activated via CLI/batch mode. You must log in through Unity Hub's GUI:

1. Run `unityhub` (it will open a GUI window on the DISPLAY).
2. Click **Sign in** and authenticate with a Unity ID in the browser.
3. After sign-in, Unity Hub will automatically activate a Personal license.

Without an active license, `Unity -batchmode` exits with code 198 ("No valid Unity Editor license found").

For **Pro/Plus/Enterprise** serial-based activation (headless):
```bash
/home/ubuntu/Unity/Hub/Editor/6000.0.66f1/Editor/Unity \
  -quit -batchmode -serial <SERIAL> -username '<EMAIL>' -password '<PASSWORD>'
```

### Common commands

| Task | Command |
|------|---------|
| Open project (batch, resolve packages) | `Unity -batchmode -quit -nographics -projectPath /workspace -logFile -` |
| Run Edit Mode tests | `Unity -batchmode -nographics -projectPath /workspace -runTests -testPlatform EditMode -logFile -` |
| Run Play Mode tests | `Unity -batchmode -nographics -projectPath /workspace -runTests -testPlatform PlayMode -logFile -` |
| Build Linux Standalone | `Unity -batchmode -nographics -projectPath /workspace -buildLinux64Player /workspace/Build/ApocaMon -logFile -` |
| Open editor GUI | `Unity -projectPath /workspace` |

Where `Unity` = `/home/ubuntu/Unity/Hub/Editor/6000.0.66f1/Editor/Unity`.

### C# compilation outside Unity

A temporary `.csproj` can compile the custom scripts against Unity DLLs at `/home/ubuntu/Unity/Hub/Editor/6000.0.66f1/Editor/Data/Managed/`. This is useful for quick syntax/type checking without a Unity license. Key DLL paths:
- `UnityEngine/UnityEngine.CoreModule.dll`, `UnityEngine.AIModule.dll`, `UnityEngine.PhysicsModule.dll`, `UnityEngine.UIModule.dll`
- Template assemblies for UGUI and TMPro under `Data/Resources/PackageManager/ProjectTemplates/libcache/`

### Gotchas

- The `.alf` manual-activation file is generated in the project root — do **not** commit it.
- The project targets 2D mode (`m_DefaultBehaviorMode: 1`). URP 2D renderer is configured in `Assets/Settings/`.
- No custom automated tests exist yet (`com.unity.test-framework` is included but unused).
- Scripts use Spanish naming conventions (e.g., `ManejadorDesfile`, `IAEnemigo`, `ControlCamara`).
