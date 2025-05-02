FROM node:20.14.0-slim

# ---- Warming up the compiler cache ----
# dummy for three op ver
RUN for VER in 0.8.17 0.8.20 0.8.28; do \
  mkdir -p /dummy && cd /dummy && \
  npm init -y && \
  npm install hardhat && \
  echo "require('hardhat/config');" > hardhat.config.js && \
  echo "pragma solidity ^$VER; contract Dummy {}" > Dummy.sol && \
  npx hardhat compile && \
  rm -rf /dummy; \
done

# --- Runtime ---
WORKDIR /workspace
CMD ["bash"]
