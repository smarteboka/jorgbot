#!/usr/bin/env bash
# Builds the docker image

docker build -f Smartbot.Dockerfile -t smartbot:latest --rm .
