# Introduction

A small console app for managing docker compose instances. 

It stores location of yaml files in the config.json file

Usage examples

`umbar add`

Adds the current folder or you can change the folder, it will identify '~' as your user home folder. It will detect if any yaml or yml files exist. If none are located the command will fail, otherwise it will be stored in the config.json for later usage.

`umbar remove`

Removes the entry from the config.json

`umbar restart`
Attempts to run `docker compose restart` and `docker compose log -f` at the location specified in config.json

`umbar pull` 

Attempts to run `docker compose pull` at the location specified in config.json

# Compiling

`dotnet publish Src/Umbar -r linux-x64 -o pub`

-o determines the output folder, change it to whatever suits your needs

The '-r' specifies the runtime identifier
an example for Windows or Mac

`osx-arm64`

`win-x64`

[Or find others here](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)


# Spectre console

The primary library used for this application is [Spectre.Console](https://github.com/spectreconsole/spectre.console/)

It is a wonderful library that makes it easy to create cool CLI's, I can highly recommend starring their repository or supporting them!