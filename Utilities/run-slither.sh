#!/bin/bash

INSTANCE_PATH="$1"

ABS_PATH="$(readlink -f "$INSTANCE_PATH")"

echo "Running Slither analysis..."
docker run --rm -v "$ABS_PATH:/workspace" -w /app slither-image .
