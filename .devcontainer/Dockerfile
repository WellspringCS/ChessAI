ARG VARIANT="8.0"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:${VARIANT}-jammy
USER root
RUN apt-get update && apt-get upgrade -y

USER vscode