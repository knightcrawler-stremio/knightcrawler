#!/bin/bash

rm -rf ../src/python
mkdir -p ../src/python
python3 -m pip install -r ../src/requirements.txt -t ../src/python/