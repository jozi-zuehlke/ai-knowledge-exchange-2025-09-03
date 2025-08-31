build *args:
  dotnet build src/AiKnowledgeExchange.sln {{ args }}
  
build-release *args:
  dotnet build src/AiKnowledgeExchange.sln {{ args }} -c Release

format:
  cd .tools/roslynator && dotnet tool run roslynator format ../../src/AiKnowledgeExchange.sln
  dotnet tool run csharpier format src

analyze:
  # some diagnostics cause the command to fail or we simple don't want them to be
  # flagged on the CLI since they are meant as hints for humans in the IDE
  cd .tools/roslynator && \
  dotnet tool run roslynator analyze ../../src/AiKnowledgeExchange.sln \
  --ignored-diagnostics IDE0058 RCS1181 RCS1208 CA1848 S103 S107 S138 S1541 S3776

fix:
  dotnet tool run csharpier format src

  # some diagnostics cause the command to fail or we simple don't want them to be
  # automatically applied (e.g. because they are only suggestions) so we disable those
  cd .tools/roslynator && \
  dotnet tool run roslynator fix ../../src/AiKnowledgeExchange.sln \
  --ignored-diagnostics IDE0058 RCS1181 RCS1208 CA1848 S103 S107 S138 S1541 S3776

fix-single file:
  dotnet tool run csharpier format {{ file }}
  cd .tools/roslynator && ./roslynator-fix-single.sh {{ file }} \
  --ignored-diagnostics IDE0058 RCS1181 RCS1208 CA1848 S103 S107 S138 S1541 S3776
  dotnet tool run csharpier format {{ file }}

run *args:
  dotnet run --project src/AiKnowledgeExchange -- {{ args }}

test *args:
  dotnet test src/AiKnowledgeExchange.sln {{ args }} --logger trx --collect:"XPlat Code Coverage" -- NUnit.ConsoleOut=0

test-filter test_case *args:
  dotnet test src/AiKnowledgeExchange.sln {{ args }} --filter "{{ test_case }}" -- NUnit.ConsoleOut=0

publish:
  dotnet publish src/AiKnowledgeExchange -c Release -r win-x64 -o ./.publish/win
  dotnet publish src/AiKnowledgeExchange -c Release -r linux-x64 -o ./.publish/linux

build-base-image:
  #!/usr/bin/env bash
  set -euo pipefail
  CURRENT_USER=$(whoami)
  CURRENT_UID=$(id -u)
  CURRENT_GID=$(id -g)
  rm -rf .image-build
  mkdir .image-build
  cp .config/dotnet-tools.json .image-build/
  cp .tools/roslynator/global.json .image-build/roslynator-global.json
  podman build \
  -f .tools/project-base.Containerfile \
  -t ai-knowledge-exchange-agent-base:latest \
  --build-arg USER_NAME=${CURRENT_USER} \
  --build-arg USER_UID=${CURRENT_UID} \
  --build-arg USER_GID=${CURRENT_GID} \
  .image-build
  rm -rf .image-build

# https://www.npmjs.com/package/@anthropic-ai/claude-code?activeTab=versions
build-claude-code: build-base-image
  #!/usr/bin/env bash
  set -euo pipefail
  CURRENT_USER=$(whoami)
  rm -rf .image-build
  mkdir .image-build
  podman build \
  -f .tools/claude-code.Containerfile \
  -t ai-knowledge-exchange-agent-claude-code:latest \
  --build-arg USER_NAME=${CURRENT_USER} \
  --build-arg TOOL_VERSION=1.0.72 \
  .image-build
  rm -rf .image-build

# https://www.npmjs.com/package/@google/gemini-cli?activeTab=versions
build-gemini-cli: build-base-image
  #!/usr/bin/env bash
  set -euo pipefail
  CURRENT_USER=$(whoami)
  rm -rf .image-build
  mkdir .image-build
  podman build \
  -f .tools/gemini-cli.Containerfile \
  -t ai-knowledge-exchange-agent-gemini-cli:latest \
  --build-arg USER_NAME=${CURRENT_USER} \
  --build-arg TOOL_VERSION=0.1.16 \
  .image-build
  rm -rf .image-build

# https://www.npmjs.com/package/opencode-ai?activeTab=versions
build-opencode-cli: build-base-image
  #!/usr/bin/env bash
  set -euo pipefail
  CURRENT_USER=$(whoami)
  rm -rf .image-build
  mkdir .image-build
  podman build \
  -f .tools/opencode-cli.Containerfile \
  -t ai-knowledge-exchange-agent-opencode-cli:latest \
  --build-arg USER_NAME=${CURRENT_USER} \
  --build-arg TOOL_VERSION=0.3.112 \
  .image-build
  rm -rf .image-build

build-ai-cli: build-claude-code build-gemini-cli build-opencode-cli
