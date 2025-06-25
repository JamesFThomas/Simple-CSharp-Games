#!/bin/bash

export DOTNET_VERSION=8.0.100
export DOTNET_ROOT=$HOME/.dotnet

wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version $DOTNET_VERSION --install-dir $DOTNET_ROOT

export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

echo "Installed .NET SDK version:"
dotnet --version

dotnet publish Simple_CSharp_Games/Simple_CSharp_Games.csproj -c Release
