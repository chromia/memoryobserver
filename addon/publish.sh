#!/usr/bin/sh

#For Firefox
rm -rf pub_firefox
cp -r base pub_firefox
cp manifests/firefox/manifest.json pub_firefox/manifest.json

#For Edge
#Do nothing here. use /native/windows/edge/memoryobserverForEdge.sln
#It is all-in-one solution for publishing

#For Chrome(not supported yet)
rm -rf pub_chrome
cp -r base pub_chrome
cp manifests/chrome/manifest.json pub_chrome/manifest.json
