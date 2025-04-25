FROM node:20.14.0-slim

# ---------- build layer with template ----------
WORKDIR /template
COPY hardhat.config.ts tsconfig.json package*.json ./
RUN npm ci

COPY entry.sh /template/entry.sh
RUN chmod +x /template/entry.sh

# --- runtime ---
WORKDIR /workspace
ENTRYPOINT ["/template/entry.sh"]
