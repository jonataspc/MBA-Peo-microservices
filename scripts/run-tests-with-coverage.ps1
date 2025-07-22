dotnet build ..\Peo.sln

dotnet tool install --global JetBrains.dotCover.CommandLineTools

dotCover cover-dotnet --output report.html --ReportType HTML --Filters="-:class=*Migrations*;+:module=Peo.*;" -- test  ..\Peo.sln

.\report.html