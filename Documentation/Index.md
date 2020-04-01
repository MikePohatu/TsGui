Documentation is currently a work in progress. For usage information, also see the [How to videos](https://www.youtube.com/playlist?list=PLbymiOxRQJvIS6BGPJ6ggKaU90QheXgV8)



## Concepts

* [Layout](/Documentation/Layout.md)
* [Prebuilt configuration](/Documentation/PrebuiltConfiguration.md)
* [Authentication](/Documentation/Authentication/Authentication.md)


## Command line options
tsgui.exe -config [filepath] -webconfig [url] -hash [password]

 **-config [filepath]** where [filepath] is the path to a config XML file on the file system<br>
 **-webconfig [url]** where [url] is the URL to a config XML file on a web server. Takes precedence over **-config**. Note that this only downloads the config, not any dependencies e.g. images<br>
 **-hash [password]** where [password] is a password to be hashed for use in a [Local Config Authentication](/Documentation/Authentication/LocalConfigAuthentication.md) block<br>
 **-key [keystring]** where [keystring] is the key to be used during the hash operation. Must be used with **-hash**

 \* If both -config and -webconfig are ommitted, TsGui will look for Config.xml in the same folder as tsgui.exe