Also see the [How to videos](https://www.youtube.com/playlist?list=PLbymiOxRQJvIS6BGPJ6ggKaU90QheXgV8)

## Concepts

* [Testing](/documentation/TestMode.md)
* [Layout](/documentation/Layout.md)
* [TsGui Options](/documentation/options/README.md) (UI controls)
* [Prebuilt configuration](/documentation/PrebuiltConfiguration.md)
* [Configuring the main window](/documentation/MainWindow.md)
* [Configuration guidelines](/documentation/ConfigGuidelines.md)
* [Configuration imports](/documentation/ConfigImports.md) (import from external config files)
* [Authentication](/documentation/Authentication/README.md)

## Features

TsGui includes a number of features to help you customise your UI to fit your needs.

| Feature    | Description/usage |
| -------- | ------- |
| [Actions](/documentation/features/Actions.md)  | Initiate actions using a button in TsGui  |
| [Authentication](/documentation/Authentication/README.md)  | Login fields in TsGui  |
| [Compliance](/documentation/features/Compliance.md)  | Pre-flight checks |
| [Groups and Toggles](/documentation/features/GroupsAndToggles.md) | Enable/disable UI elements |
| [Option Linking](/documentation/features/OptionLinking.md)    | Use values of one UI element in another |
| [Queries](/documentation/features/Queries.md) | Gather data for use in TsGui |
| [Scripts](/documentation/features/Scripts.md) | Integration of separate scripts with TsGui |
| [Sets](/documentation/features/Sets.md) | Create a set or dynamic list of variables |
| [Styles (previously Formatting)](/documentation/features/Styles.md) | Layout, look and feel |
| [TsGui Output](/documentation/features/TsGuiOutput.md) | Saving values to Task Sequence variables or Registry |
| [Validation](/documentation/features/Validation.md) | Check user input |

---

## Command line options
tsgui.exe -config [filepath] -webconfig [url] -hash [password] -test

 **-config [filepath]** where [filepath] is the path to a config XML file on the file system<br>
 **-webconfig [url]** where [url] is the URL to a config XML file on a web server. Takes precedence over **-config**. Note that this only downloads the config, not any dependencies e.g. images<br>
 **-hash [password]** where [password] is a password to be hashed for use in a [Local Config Authentication](/documentation/Authentication/LocalConfigAuthentication.md) block<br>
 **-key [keystring]** where [keystring] is the key to be used during the hash operation. Must be used with **-hash**<br>
 **-test** start TsGui in [test mode](/documentation/TestMode.md)

 \* If both -config and -webconfig are ommitted, TsGui will look for Config.xml in the same folder as tsgui.exe


 ## Code

 Source documentation can be found in the [code](/documentation/code/README.md) documentation.