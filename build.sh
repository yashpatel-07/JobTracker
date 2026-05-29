#!/bin/bash

# Install .NET 10
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"

# Create appsettings.json from example with real keys
sed "s|your-supabase-url|$SUPABASE_URL|g; s|your-supabase-key|$SUPABASE_ANON_KEY|g" \
  wwwroot/appsettings.example.json > wwwroot/appsettings.json

echo "✓ appsettings.json created"

dotnet publish -c Release -o publish