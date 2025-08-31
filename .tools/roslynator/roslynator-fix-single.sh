#!/usr/bin/env bash

set -euo pipefail

if [ "$#" -lt 1 ]; then
    echo "Usage: $0 <path-to-file>"
    exit 1
fi

FILE_PATH="$(realpath "$1")"

if [ ! -f "$FILE_PATH" ]; then
    echo "Error: File '$FILE_PATH' does not exist."
    exit 1
fi

DIR="$(dirname "$FILE_PATH")"
PROJECT_FILE=""

# Walk up the directory tree to find the first .csproj file
while [ "$DIR" != "/" ]; do
    MATCHES=($(find "$DIR" -maxdepth 1 -type f -name "*.csproj"))
    if [ "${#MATCHES[@]}" -gt 0 ]; then
        PROJECT_FILE="${MATCHES[0]}"
        break
    fi
    DIR="$(dirname "$DIR")"
done

if [ -z "$PROJECT_FILE" ]; then
    echo "Error: No .csproj file found in the directory hierarchy."
    exit 1
fi

echo "Running roslynator fix on project: $PROJECT_FILE with file: $FILE_PATH"
dotnet tool run roslynator fix "$PROJECT_FILE" --include "$FILE_PATH" "${@:2}"
