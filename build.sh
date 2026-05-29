#!/bin/bash

# Create appsettings.json from example with real keys
sed "s|your-supabase-url|$SUPABASE_URL|g; s|your-supabase-key|$SUPABASE_ANON_KEY|g" \
  wwwroot/appsettings.example.json > wwwroot/appsettings.json

echo "✓ appsettings.json created"

dotnet publish -c Release -o publish