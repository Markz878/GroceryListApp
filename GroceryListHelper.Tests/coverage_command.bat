rmdir /s /q TestResults
dotnet test -- --coverage --coverage-output-format cobertura --coverage-output coverage.xml --coverage-settings .runsettings
reportgenerator -reports:"./TestResults/coverage.xml" -targetdir:"coverageresults" -reporttypes:HtmlInline
.\coverageresults\index.html
pause