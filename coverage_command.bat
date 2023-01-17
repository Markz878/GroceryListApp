rmdir /s coverage
dotnet test -c Release --collect:"XPlat Code Coverage" --results-directory ./coverage
reportgenerator -reports:".\coverage\*\coverage.cobertura.xml" -targetdir:"coverageresults" -reporttypes:HtmlInline
.\coverageresults\index.html