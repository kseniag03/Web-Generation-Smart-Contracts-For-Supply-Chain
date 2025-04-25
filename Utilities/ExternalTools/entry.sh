#!/usr/bin/env bash
set -e

# if in /workspace (bind-mount) there is no config-файла — copy template
if [[ ! -f /workspace/hardhat.config.ts && ! -f /workspace/hardhat.config.js ]]; then
  cp -a /template/. /workspace
fi

cd /workspace

exec npx hardhat "$@"
