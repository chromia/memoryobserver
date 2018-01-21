#!/usr/bin/sh

rm -rf pub_firefox
#rm -rf pub_chrome
#rm -rf pub_edge

cp -r base pub_firefox
#cp -r base pub_chrome
#cp -r base pub_edge

cp manifests/manifest.firefox pub_firefox/manifest.json
#cp manifests/manifest.chrome pub_chrome/manifest.json
#cp manifests/manifest.edge pub_edge/manifest.json
