#!/bin/sh

CURRENT_IP="$(curl -s http://whatismyip.akamai.com)"
clear
echo "Current IP: $CURRENT_IP"
