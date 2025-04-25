#!/bin/bash

INSTANCE_PATH="$1"

ABS_PATH="$(readlink -f "$INSTANCE_PATH")"

echo "Running Foundry gas report..."
docker run --rm -v "$ABS_PATH:/workspace" -w /app foundry-image forge test --gas-report
