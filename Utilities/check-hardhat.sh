#!/usr/bin/env bash
set -e

INSTANCE_ID="$1"

# this is now the host path:
HOST_INSTANCES="${HOST_INSTANCES:?not set}"

docker run --rm \
  -v "${HOST_INSTANCES}/${INSTANCE_ID}:/workspace" \
  hardhat-image \
  bash -c '
    cd /workspace
    echo "Inside container: /workspace contents:"
    echo "---------------------------------------------------"
    ls -la /workspace
    echo "---------------------------------------------------"
    echo "Checking for important files:"
    for f in package.json hardhat.config.ts tsconfig.json contracts test; do
      if [ -e /workspace/$f ]; then
        echo "Found $f"
      else
        echo "Missing $f"
      fi
    done
    echo "Done checking."
  '

docker run --rm \
  -v "${HOST_INSTANCES}/${INSTANCE_ID}:/workspace" \
  hardhat-image \
  bash -c '
    cd /workspace
    if [ ! -d node_modules ]; then
      echo "Installing dependencies with npm install..."
      npm install || { echo "npm install failed"; exit 1; }
    else
      echo "node_modules already present."
    fi
  '
