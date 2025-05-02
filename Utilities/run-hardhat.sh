#!/usr/bin/env bash
set -e

CMD="$1"
INSTANCE_ID="$2"

# this is now the host path:
HOST_INSTANCES="${HOST_INSTANCES:?not set}"

echo "HOST_INSTANCES = $HOST_INSTANCES"

docker run --rm \
  -v "${HOST_INSTANCES}/${INSTANCE_ID}:/workspace" \
  hardhat-image \
  bash -c '
    cd /workspace
    echo "Checking node_modules..."
    if [ ! -d node_modules ]; then
      echo "Running npm install..."
      npm install || { echo "npm install failed"; exit 1; }
    fi
    echo "Running: npx hardhat '"$CMD"'"
    npx hardhat '"$CMD"'
  '
