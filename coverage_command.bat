rmdir /s /q coverage
dotnet test GroceryListHelper.Tests -c Release --collect:"XPlat Code Coverage" --results-directory ./coverage /p:CollectCoverage=true /p:ExcludeByAttribute="GeneratedCodeAttribute"
reportgenerator -reports:".\coverage\*\coverage.cobertura.xml" -targetdir:"coverageresults" -reporttypes:HtmlInline
.\coverageresults\index.html
pause