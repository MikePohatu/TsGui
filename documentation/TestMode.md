# Test Mode

By default, TsGui will attempt to load the COM objects from a ConfigMgr task sequence. This is normal 'production' mode.

If [outputting to registry](/documentation/features/TsGuiOutput.md#registry-output), TsGui will also run in production mode unless the '-test' command line option is used.

If TsGui is not outputting to registry, can't attach to the ConfigMgr COM objects, or the '-test' command line option has been used, it will enter 'test mode'.

Test mode allows TsGui configuration to be quickly tested.

## Reloading in Test Mode

When running in test mode, you can reload the configuration without having to restart TsGui. Make any desired changes to the configuration XML file, then either click 'Reload' on the [LiveData](#the-livedata-window) window, or press CTRL+F5.

These options will not work in 'production' mode.

## The LiveData Window

If the LiveData="TRUE" attribute is set in the config, a separate testing window will open showing the current values of all the elements in TsGui. The values will update as changes are made within TsGui.

The LiveData attribute will only open the LiveData window when running in test mode. If in production mode, the LiveData window will not appear.

```xml
<TsGui LiveData="TRUE">
    ...
</TsGui>
```

If you also want the LiveData window to appear in 'production' mode, you can set the Debug="TRUE" attribute.

```xml
<TsGui Debug="TRUE">
    ...
</TsGui>
```