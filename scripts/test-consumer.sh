#!/bin/sh
# Usage: scripts/test-consumer.sh <directory-with-nupkg> [target-framework]
# Restores BarisCemant.Verimor only from that directory into a fresh project and calls all products.
set -eu

package_dir=$(cd "$1" && pwd)
framework=${2:-net10.0}
root=$(cd "$(dirname "$0")/.." && pwd)
version=$(tr -d '[:space:]' < "$root/VERSION")
work=$(mktemp -d)
trap 'rm -rf "$work"' EXIT

cp "$root/tests/consumer/Consumer.csproj" "$root/tests/consumer/Program.cs" "$work/"
cat > "$work/nuget.config" <<CONFIG
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$package_dir" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local">
      <package pattern="BarisCemant.Verimor" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
CONFIG

export NUGET_PACKAGES="$work/packages"
cd "$work"
dotnet run -c Release -f "$framework" -p:VerimorVersion="$version"
