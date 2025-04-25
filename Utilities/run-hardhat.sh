#!/usr/bin/env bash
set -e

CMD="$1"
INSTANCE_PATH="$2"

ABS_PATH="$(readlink -f "$INSTANCE_PATH")"

docker run --rm \
  -v "$ABS_PATH:/workspace" \
  hardhat-image "$CMD"
