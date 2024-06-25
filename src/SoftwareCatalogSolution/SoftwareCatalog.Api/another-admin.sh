#!/bin/bash

echo 'Generating Boss (jesse) Token'

dotnet user-jwts create -n jesse@company.com --role SoftwareCenter --role Admin |  grep -oP '(?<=Token: ).*' | clip

echo 'Token for Boss (Jesse) is in your clipboard.'