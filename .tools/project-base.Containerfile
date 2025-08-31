FROM ubuntu:24.04

# Prevents interactive prompts during package installation
ENV DEBIAN_FRONTEND=noninteractive

# Install core dependencies
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        curl \
        ca-certificates \
        gnupg \
        apt-transport-https \
        software-properties-common \
        unzip \
        git \
        build-essential \
        libssl-dev \
        pkg-config \
        neovim \
    && rm -rf /var/lib/apt/lists/*

# Install dotnet 9 SDK
RUN add-apt-repository ppa:dotnet/backports && \
    apt-get update && \
    apt-get install -y --no-install-recommends \
        dotnet-sdk-8.0 \
        dotnet-sdk-9.0 \
    && rm -rf /var/lib/apt/lists/*

# Add dotnet to PATH
ENV PATH="${PATH}:/root/.dotnet:/root/.dotnet/tools"
ENV DOTNET_ROOT="/root/.dotnet"

# some more settings for dotnet
ENV DOTNET_NOLOGO='true' \
    DOTNET_GENERATE_ASPNET_CERTIFICATE='false' \
    DOTNET_CLI_TELEMETRY_OPTOUT='true'

# Install just command runner
RUN curl -sSL https://just.systems/install.sh | bash -s -- --to /usr/local/bin

# mirror host user
ARG USER_NAME
ARG USER_UID
ARG USER_GID

# Delete conflicting user/group if they exist, then add the correct ones
RUN set -eux; \
    # Remove any existing user with the same UID
    existing_user=$(getent passwd ${USER_UID} | cut -d: -f1 || true); \
    if [ -n "$existing_user" ]; then \
        deluser --remove-home "$existing_user"; \
    fi; \
    # Remove any existing group with the same GID
    existing_group=$(getent group ${USER_GID} | cut -d: -f1 || true); \
    if [ -n "$existing_group" ]; then \
        delgroup "$existing_group"; \
    fi; \
    # Create group and user
    addgroup --gid "$USER_GID" "$USER_NAME"; \
    adduser --uid "$USER_UID" --gid "$USER_GID" --disabled-password --gecos "" --shell /bin/sh "$USER_NAME"

USER $USER_NAME

# Verify installs
RUN dotnet --version && just --version

# Install any tools
WORKDIR /.setup

COPY dotnet-tools.json ./

RUN dotnet tool restore

COPY roslynator-global.json ./global.json

RUN dotnet --version && dotnet tool restore

WORKDIR /work

USER root

RUN rm -rf /.setup

USER $USER_NAME
