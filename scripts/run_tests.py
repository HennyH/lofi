#!/bin/python

import subprocess
import sys

subprocess.run(
    ["dotnet", "test", "Lofi.API.Tests", "/p:CollectCoverage=true", "/p:CoverletOutputFormat=cobertura", "/p:Exclude=\"[xunit*]\*\"", "/p:CoverletOutput=\"../.coverage/\"" ],
    check=True
)

subprocess.run(
    ["dotnet", "reportgenerator", "-reports:./.coverage/coverage.cobertura.xml", "-targetdir:.coverage", "-reporttypes:html"],
    check=True
)
