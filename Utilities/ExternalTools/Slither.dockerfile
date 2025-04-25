FROM python:3.13-slim

WORKDIR /app

RUN pip install --upgrade pip && \
    pip install slither-analyzer

ENTRYPOINT ["slither"]
