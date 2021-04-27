# Messaging

TsGui contains a Messaging library to control message flow between objects in the application. This library continues the 20Road [tradition](https://github.com/MikePohatu/Birdsnest-Explorer/tree/master/source/BirdsNest.Net/Console/ClientApp/src/assets/ts/webcrap) of using 'crap' names for ancillary libraries and functions. The **MessageCrap** namespace contains the ```MessageHub``` static class and ```Message``` class for creating and sending messages. 

**Contents**
* [Message class](#message-class)
  * [Callback](#callback)
  * [ID](#id)
  * [Payload](#payload)
  * [ResponseExpected](#responseexpected)
  * [RespondingTo](#respondingto)
  * [RootMessage](#rootmessage)
  * [Set Functions](#set-functions)
  * [Topic](#topic)
* [Guidelines](#guidelines)

## Message class

### Callback
A anonymous function can be set on the message if desired. This callback can be run by a receiver.

### ID 
Each message has a GUID generated on creation. Trace logging will track these GUIDs as they are sent, cancelled, loops closed etc.

### Payload
An arbitrary payload can be attached to the message if desired. The payload type should always be the same for a given [topic](#topic).

### ResponseExpected
The *ResponseExpected* property defines whether the message is expecting to have a response. If set to true, a timer will be started (timeout configurable with the *ResponseTimeoutMilliseconds* property, default 30seconds). If a response is not received before the timeout, the missed response will be logged.

### RespondingTo
The message that the current message is responding to will be set here. This will return ```null``` if this is the first message in the chain. 

### RootMessage
The root message is the first message sent in this message chain i.e. it is not a response to another message. The RootMessage is checked when a message comes in. If the sender of the message and it's RootMessage are the same, there has been full loop and the message will be stopped. All subscriptions for the stopped message will be ignored. This will return ```null``` if this is the first message in the chain. 

### Set Functions
The message class contains a number of Set*Property*() functions that set the relevant property and return the message object. These can be used to daisy chain calls together, and then be sent using the ```Send()``` function, for example:

```C#
MessageHub.CreateMessage(this, message).SetTopic(Topics.ReprocessRequest).SetPayload(this._source.ID).SetResponseExpected(true).Send()
```

### Topic
A message topic defines what the message is about, e.g. ```TsGui.Linking.ReprocessRequest```. Classes defining the ```ITopicSubscriber``` interface can subscribe to these topics and will receive all messages for this topic.



## Guidelines

Use the following guidelines when using the MessageCrap library:

* When in doubt, pass the message. This will catch loops
* Use the namespace when defining topic names. This reduces the chance of duplicates and name conflicts. 
* Create a ```Topics``` class in your namespace containing ```const string``` values for your topic names. This will ensure consistency of sent messages
```c#
public static class Topics
{
    public const string SourceValueChanged = "TsGui.Linking.SourceValueChanged";
    public const string ReprocessRequest = "TsGui.Linking.ReprocessRequest";
}
```
