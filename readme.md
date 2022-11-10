# File System Analyzer

*An app that analyses the disk usage of folders in your file system. Based on Avalonia and RavenDB.*

![Light Libraries Logo](/Design/file-system-analyzer-icon-256x256.png)

## How to Build and Run

You need to have the .NET 6 SDK installed and an IDE of your choice that can handle `.sln` files. 

Simply check out the repository and open `./Code/FileSystemAnalyzer.sln` in your favorite IDE. Build and run the `FileSystemAnalyzer.AvaloniaApp` project. You do not need to configure anything - the app will start up an embedded RavenDB server with the default settings.

Once the app is running, you are presented with a list view, showing all analyses that you already conducted. Click "Create new analysis..." on the top left and select a folder in your file system that you want to analyze.

![Light Libraries Logo](/Design/analyses-list-view.png)

File System Analyzer will create a snapshot of the file system entries in the selected folder and store it in the underlying RavenDB database. While the snapshot is created, you can already view the processed results. Depending on the folder you choose, the analysis can take several minutes (many small files and folders decrease performance - I'm looking at you, `node_modules`).

If you inspect an analysis, you have three views that you can access via the tabs at the top: use the explorer to drill down into the target folder. Use the files and folders tabs to get flat access to all file system entries. The files tab also allows grouping over file extensions.

![Light Libraries Logo](/Design/explorer.png)

![Light Libraries Logo](/Design/files-view.png)

![Light Libraries Logo](/Design/folders-view.png)

If you want to inspect the database, you can open your browser and go to [http://localhost:10002](http://localhost:10002) by default to inspect the database (the app must be running while accessing the web frontend of the database server).

![Light Libraries Logo](/Design/ravendb.png)

You can use RavenDB for seven days without a license. Afterwards, just simply head over to [ravendb.net](https://ravendb.net/buy) and get a free license.

You can customize the settings by creating a file called `appsettings.Development.json` next to the existing [appsettings.json](https://github.com/feO2x/FileSystemAnalyzer/blob/main/Code/FileSystemAnalyzer.AvaloniaApp/appsettings.json) file. This file is automatically ignored by git. Simply assign new values in this file with the same keys, they will override the contents of appsettings.json.

## Acknowledgments

File System Analyzer uses the following Open Source packages:

- [Avalonia](https://avaloniaui.net/), [Material.Avalonia](https://github.com/AvaloniaCommunity/Material.Avalonia), and [Material.Icons.Avalonia](https://github.com/SKProCH/Material.Icons)
- [Bogus](https://github.com/bchavez/Bogus)
- [FluentAssertions](https://fluentassertions.com/)
- [FuzzySharp](https://github.com/JakeBayer/FuzzySharp)
- [Humanizer](https://humanizr.net/)
- [LightInject](https://www.lightinject.net/)
- [Light.GuardClauses](https://github.com/feO2x/Light.GuardClauses)
- [Light.ViewModels](https://github.com/feO2x/Light.ViewModels)
- [Microsoft.Extensions.Configuration](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [RavenDB](https://ravendb.net/)
- [Serilog](https://serilog.net/)
- [Synnotech.Core](https://github.com/Synnotech-AG/Synnotech.Core), [Synnotech.DatabaseAbstractions.Mocks](https://github.com/Synnotech-AG/Synnotech.DatabaseAbstractions.Mocks) [Synnotech.FluentProcesses](https://github.com/Synnotech-AG/Synnotech.FluentProcesses), [Synnotech.RavenDB](https://github.com/Synnotech-AG/Synnotech.RavenDB), and [Synnotech.Time](https://github.com/Synnotech-AG/Synnotech.Time)
- [Verify](https://github.com/VerifyTests/Verify)
- [xUnit.net](https://github.com/xunit/xunit)
