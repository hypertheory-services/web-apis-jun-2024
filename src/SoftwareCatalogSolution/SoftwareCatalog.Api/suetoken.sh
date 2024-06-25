#!/bin/bash

echo 'Generating SUE Token'

dotnet user-jwts create -n sue@company.com --role SoftwareCenter |  grep -oP '(?<=Token: ).*' | clip

echo 'Token for sue is in your clipboard.'