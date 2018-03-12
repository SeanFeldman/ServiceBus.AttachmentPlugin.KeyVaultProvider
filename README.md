![Icon](https://github.com/SeanFeldman/ServiceBus.AttachmentPlugin.KeyVaultProvider/blob/develop/images/project-icon.png)

### This is a KeyVault provider for  [ServiceBus.AttachmentPlugin](https://github.com/SeanFeldman/ServiceBus.AttachmentPlugin) add-in

License: [![License](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/SeanFeldman/ServiceBus.AttachmentPlugin.KeyVaultProvider/blob/develop/LICENSE)

Build status: [![develop](https://img.shields.io/appveyor/ci/seanfeldman/ServiceBus-AttachmentPlugin-KeyVaultProvider/develop.svg?style=flat-square)](https://ci.appveyor.com/project/seanfeldman/ServiceBus-AttachmentPlugin)

Opened issues: [![Issues](https://img.shields.io/github/issues-raw/badges/shields/website.svg)](https://github.com/SeanFeldman/ServiceBus.AttachmentPlugin.KeyVaultProvider)

### Nuget package

[![NuGet Status](https://buildstats.info/nuget/ServiceBus.AttachmentPlugin.KeyVaultProvider?includePreReleases=true)](https://www.nuget.org/packages/ServiceBus.AttachmentPlugin.KeyVaultProvider/)

Available here http://nuget.org/packages/ServiceBus.AttachmentPlugin.KeyVaultProvider

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package ServiceBus.AttachmentPlugin.KeyVaultProvider

## Examples

### Convert body into attachment, no matter how big it is

Configuration and registration

```c#
var provider = new KeyVaultProvider("client-id", "client-secret", "secret-identifier"); // secret-identifier only for MSI
var configuration = new AzureStorageAttachmentConfiguration(new KeyVaultProvider("&lt;secret-identifier&gt;"));

var queueClient = new QueueClient(...);
queueClient.RegisterAzureStorageAttachmentPlugin(configuration);

await queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes("payload to transfer via storage account")));

var messageReceiver = new MessageReceiver(...);
messageReceiver.RegisterAzureStorageAttachmentPlugin(configuration);

var message = await messageReceiver.ReceiveAsync();
```

## Icon

Created by Gregor Cresnar from the Noun Project