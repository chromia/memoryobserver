# Difference manifest.json between browsers

On 22/01/2018

## Reference

Firefox: <https://developer.mozilla.org/ja/Add-ons/WebExtensions/manifest.json>  
Edge:  <https://docs.microsoft.com/en-us/microsoft-edge/extensions/api-support/supported-manifest-keys>  
Chrome: <https://developer.chrome.com/extensions/manifest>

## List

*any : optional  
*YES : mandatory  
*NO : not supported  
*? : not researched yet

|Key|Firefox|Edge|Chrome|
|-|-|-|-|  
|applications|any| **NO** | **NO** |
|author| **NO** | **YES** |any|
|automation| **NO** | **NO** |any *5|
|background|any|any *1|any|
|browser_action|any|any *4|any|
|chrome_settings_overrides|any| **NO** |any|
|chrome_url_overrides|any|any *2|any|
|commands|any| **NO** |any|
|content_scripts|any|any|any|
|content_security_policy|any|any|any|
|default_locale|any|any|any|
|description|any|any|any|
|developer|any| **NO** | **NO** |
|devtools_page|any| **NO** |any|
|event_rules| **NO** | **NO** |any|
|externally_connectable| **NO** | **NO** |any|
|homepage_url|any| **NO** |any|
|icons|any|any|any|
|import| **NO** | **NO** |any|
|incognito|any| **NO** |any|
|key| **NO** | **NO** |any|
|manifest_version| **YES** |any *3| **YES** |
|minimum_chrome_version| **NO** | **NO** |any|
|nacl_modules| **NO** | **NO** |any|
|name| **YES** | **YES** | **YES** |
|offline_enabled| **NO** | **NO** |any|
|omnibox|any| **NO** |any|
|optional_permissions|any| **NO** |any|
|options_page| **NO** |any|any|
|options_ui|any| **NO** |any|
|page_action|any|any *4|any|
|permissions|any|any|any|
|protocol_handlers|any| **NO** | **NO** |
|requirements| **NO** | **NO** |any|
|sandbox| **NO** | **NO** |any|
|short_name|any|any|any|
|sidebar_action|any| **NO** | **NO** |
|storage| **NO** | **NO** |any|
|theme|any| **NO** |any|
|update_url| **NO** | **NO** |any|
|version| **YES** | **YES** | **YES** |
|version_name| **NO** | **NO** |any|
|web_accessible_resources|any|any|any|

*1 persistent property is required  
*2 no document found on microsoft website  
*3 ignored  
*4 "default_icon" key isn't supported
*5 experimental

## Mandatory Key List

|Browser|Keys|
|-|-|
|Firefox|name, manifest_version, version|
|Edge|author, name, version|
|Chrome|name, manifest_version, version|
