# Option Linking

Linking enables the value of one option to influence the value of another. This differs from groups in that groups only set whether an option is enabled or not, Linking will update options as the options they are linked to change.

**Contents**
* [Messaging](#messaging)
* [Concepts](#concepts)

## Messaging

Linking has potential to create circular calls, for example:

1. OptionA has a WMI query, and is updated by a validation refresh i.e. 'please check now'
2. OptionA sends an event indicating it has been updated
3. OptionB contains a query that will react to changes in OptionA. OptionB is triggered to update accordingly
4. As part of updating, OptionB has to process all it's queries in case one query effects another
5. OptionB triggers the query linking to OptionA
6. The query asks OptionA to update it's value to make sure it is up to date
7. Return to #2

Due to this, Option Linking makes use of message passing. The message passing [library](Messaging.md) includes checks to catch circular events. If a sender sends a message responding to it's own message, the message is stopped.

## Concepts

* ILinkSource - This is the source option. It must contain the following:
    * ```string CurrentValue { get; }``` - the current value of the option
    * ```string ID { get; }``` - a unique ID identifying the option, defined the configuration
    * ```void UpdateValue(Message message)``` - a method to update to the option, passing in a Message object which is the Message that requested the update. If UpdateValue is called by the instance, you would pass ```null``` as the message parameter

* ILinkTarget - The target option that will be updated after changes to the source
  * ```void OnSourceValueUpdated(Message updateMessage)``` - a method to be run following changes to the source. If the source was requested to update by a message, a response (or the original message at least) should be passed to this method so any loops can be captured. 

* LinkingHub - The LinkingHub is responsible for connecting ILinkSource instances with ILinkTarget instances, handling the messaging subscriptions and udpates to maintain consistency. Please follow these rules to keep the code as easy to maintain as possible:
  * Send messages relating to linking via the LinkingHub
  * Create subscriptions relating to linking via the LinkingHub