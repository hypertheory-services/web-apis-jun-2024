#!/bin/bash

echo 'Generating Boss Token'

dotnet user-jwts create -n boss@company.com --role SoftwareCenter --role Admin |  grep -oP '(?<=Token: ).*' | clip

echo 'Token for Boss is in your clipboard.'