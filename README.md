# SetupTool

A configurable Windows setup tool written in C#. It installs software packages (online or offline), sets up required tools (like PM2, nginx, MariaDB, PHP, Composer), and registers a PM2 startup service for use in production systems.

## Features

- Installs packages via Chocolatey (`choco`), NPM (`npm`), MSI, or ZIP
- Offline and online package support
- PM2 startup script generation
- Windows service registration (`sc.exe`)
- JSON-based configuration

## Usage

1. Create a configuration file: `include/packages.json`  
   Example:

```json
   {
     "offlinePackages": [
       {
         "Name": "nodejs",
         "Type": "msi",
         "Source": "offline/node-v20.11.0-x64.msi"
       }
     ],
     "onlinePackages": [
       {
         "Name": "php",
         "Type": "choco"
       },
       {
         "Name": "composer",
         "Type": "msi",
         "Source": "offline/composer-setup.exe"
       },
       {
         "Name": "pm2",
         "Type": "npm"
       }
     ]
   }
```
---
2. Place offline installers in the `offline/` folder relative to the executable, if needed.

3. Run the tool:

   * To install:

     ```bash
     SetupTool.exe setup
     ```

   * To uninstall:

     ```bash
     SetupTool.exe remove
     ```

## Requirements

* .NET Framework
* Admin privileges (for package installation and service registration)

## Notes

* The tool detects internet access and automatically chooses between offline and online packages.
* Offline ZIP and MSI packages require the `Source` and (for ZIP) `TargetDir` fields in `packages.json`.

## TODO
  Refactor Code
  Implement more arguments

## License
This project is licensed under the MIT License.
