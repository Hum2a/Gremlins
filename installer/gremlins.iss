; Build publish first: dotnet publish -c Release -o publish
; Then: iscc installer/gremlins.iss (from repository root)

#define MyAppName "Gremlins"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Squarebrackets"
#define MyAppExeName "Gremlins.exe"

[Setup]
AppId={{8F2A1B3C-4D5E-6F70-8A9B-0C1D2E3F4A5B}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
; Relative to the folder containing this .iss file
OutputDir=output
OutputBaseFilename=Gremlins-Setup-1.0.0
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "..\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"
Name: "startupentry"; Description: "Start Gremlins when Windows starts"; GroupDescription: "Startup:"

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: """{app}\{#MyAppExeName}"""; Flags: uninsdeletevalue; Tasks: startupentry

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
