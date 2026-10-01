#!/bin/sh
# Usage: scripts/check-version.sh <tag>   Fails unless the tag is v<VERSION>.
set -eu
root=$(cd "$(dirname "$0")/.." && pwd)
expected="v$(tr -d '[:space:]' < "$root/VERSION")"
if [ "$1" != "$expected" ]; then
  echo "Tag $1 does not match VERSION ($expected)" >&2
  exit 1
fi
