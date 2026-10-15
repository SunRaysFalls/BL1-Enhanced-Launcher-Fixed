# Borderlands GOTY Enhanced launcher — fixed source reconstruction

This project is a full C# decompilation of the installed patched `Launcher.exe`, with the fixes annotated in the relevant source. It is not Gearbox's original authored source repository.

## Source reference

- Executable: `Binaries/Win64/Launcher.exe`
- SHA-256: `2DD16C9763DE8EB91243CE90262714C7B371E9643A4420B557328EFCD653AC5E`
- Decompiled C# files: `Launcher/`
- Embedded launcher settings resource: `Launcher/ConfigData/Settings.w2l`
- The `Dependencies/` folder contains copies of the launcher's support assemblies from the local game installation.

## Included fixes

- Normal launcher startup no longer unconditionally saves and rewrites the user's configuration.
- The launcher's VSync choice is saved to both `SystemSettings.UseVSync` and `[VSync] Enabled`.
- Selecting a graphics preset copies that preset's `MaxShadowResolution` into the user engine config.
- Preset application updates anisotropic filtering with the requested mapping:

  | Preset | Launcher selection | Saved anisotropy |
  | --- | ---: | ---: |
  | Low | 1 | 2× |
  | Medium | 2 | 4× |
  | High | 3 | 8× |
  | Ultra | 4 | 16× |

- The embedded launcher settings resource classifies `MaxAnisotropy` as `ESettingType.MaxAnisotropy`.

## Build notes

The project targets the original x86 .NET Framework 4.0 WPF launcher. Rebuilding requires Windows with the .NET Framework 4.0 targeting/developer pack, WPF build tools, XNA Framework 4.0, and Visual Studio/MSBuild. A .NET SDK was not available in the environment used to prepare this source, so compilation was not verified. Decompiled source may also need project or resource adjustments to reproduce the executable byte-for-byte.

This package contains source and the launcher's support assemblies; it does not include the game itself.
