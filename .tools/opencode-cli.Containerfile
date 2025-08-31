FROM ai-knowledge-exchange-agent-base:latest

ARG USER_NAME

USER $USER_NAME

ENV NODE_VERSION=24.4.1
ENV NVM_DIR="/home/$USER_NAME/.nvm"

# Install nvm and node
RUN mkdir -p "$NVM_DIR" && \
    curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh | bash && \
    . "$NVM_DIR/nvm.sh" && \
    nvm install $NODE_VERSION && \
    nvm use $NODE_VERSION && \
    nvm alias default $NODE_VERSION

# Add node to PATH
ENV PATH="${PATH}:$NVM_DIR/versions/node/v$NODE_VERSION/bin"

# install the opencode CLI
ARG TOOL_VERSION

RUN npm install -g npm --no-fund && \
    npm install -g opencode-ai@$TOOL_VERSION --no-fund

# Verify installs
RUN node --version && npm --version && opencode --version

ENTRYPOINT ["opencode"]
