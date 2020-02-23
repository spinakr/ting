#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

dotnet publish "$DIR/../Ting.sln"

PUBDIR="$DIR/../src/ting/bin/Debug/netcoreapp3.1/publish/"
DATE=`date +"%Y-%m-%d-%H%M"`
DEPLOYDIR="rpi-white:/home/sp1nakr/applications/ting"

echo "Deploying application from $PUBDIR to $DEPLOYDIR"

ssh rpi-white "rm -rf /home/sp1nakr/applications/ting"
scp -r $PUBDIR $DEPLOYDIR

ssh -t rpi-white "sudo systemctl enable kestrel-ting.service;sudo systemctl start kestrel-ting.service;sudo systemctl restart kestrel-ting.service;sudo systemctl status kestrel-ting.service"