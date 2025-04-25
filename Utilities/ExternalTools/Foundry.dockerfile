FROM ubuntu:22.04

ENV FOUNDRY_HOME=/root/.foundry
WORKDIR /app

RUN apt-get update && \
    apt-get install -y curl git && \
    curl -L https://foundry.paradigm.xyz | bash && \
    $FOUNDRY_HOME/bin/foundryup && \
    ln -s $FOUNDRY_HOME/bin/forge /usr/local/bin/forge

ENTRYPOINT ["forge"]
