# Message System

## Introduction

The **MessageSystem** package is intended to facilitate the implementation of the **Observer Pattern** that allows the communication of multiple classes using C# events.

## Functions

Using the MessageSystem is very simple. The Channel class contains static references to all instances of itself, and there are only 3 functions that own the `Channel<T>` instances:

1. `Attach(Enum, Action<IMessage<Enum>>);`
2. `Detach(IMessage<IEnum>);`
3. `Send(IMessage<IEnum>);`

## Attach

This function is used to listen to a message from the channel. It is necessary to indicate the type of message and the function to call when receiving the message.
This function must receive as parameter an `IMessage<TEnum>` and the Enum type must be the same as the channel. You will find more information about the `IMessage` interface later in this documentation.

Example:

`Channel.Unity.Attach(UnityET.OnApplicationPause, OnApplicationPauseMessage);`

## Detach

You should use this function when you no longer want to receive the messages you `Attach`.
**Important**: Use this function in the `OnDestroy()` to avoid errors.

Example:

`Channel.Unity.Detach(UnityET.OnApplicationPause, OnApplicationPauseMessage);`

## Send

This function sends a message. It receives as parameter an `IMessage<TEnum>` that has to be of the same type as the channel from which it is being sent. If the message is null, the send will be cancelled.

Examples:

Long way:
`Channel.Unity.Send(new Message<UnityET>(UnityET.OnApplicationPause, pause, false));`

Short way:
`Channel.Unity.Send(UnityET.OnApplicationPause, pause);`

The short way simply creates a message with the assigned parameters and executes the long way.

## Create a New Channel

Step-by-step:

1. Go to the path `"Message System/Runtime/"` and copy the contents of the `UnityChannel.cs` script.
2. Create a new script inside the "Assets" folder with the name of your new channel and add `Channel` at the end to follow the naming convention.
3. Replace the content of your script with the `UnityChannel` script you copied previously.
4. Change the name of the `Unity` channel to your own without including the word `Channel`.
5. Do the same with the enum name found in the script.
6. Modify the values of the enum leaving as first value `None`.
7. Use the new channel you created: `Channel.YourChannelName.Send(YourChannelName.MessageType);`.

## `IMessage`, `IMessage<TEnum>` and `Message<TEnum>`

I recommend you to open the `IMessage.cs` script located in `"Message System/Runtime/"` and read its content at the top, it will help you to understand easily how it works, anyway, I will explain it below.

The inheritance of these interfaces and the Message class is like this:

`IMessage <- IMessage<TEnum> <- Message<TEnum>`

`IMessage` (without generic type) is not necessary for the operation of the **MessageSystem**, it exists only to allow, if desired, a reference to any type of `IMessage<TEnum>`.

`IMessage<TEnum>` Any object that implements this interface can be used through a channel. You should avoid having multiple implementations of this interface, as it would be a bad practice, instead, the `IMessage.cs` script already provides some generic classes that should save you the need for new implementations.

`Message<TEnum>` implements the `IMessage<TEnum>` interface.
When creating a new `Message<TEnum>` it will be necessary to pass as parameter only the message type, but in case you think it is necessary, you can send a content (default is null) and indicate if the message will be sent from an asynchronous function `isSenderAsync` (default is false).
This last parameter is used to know if the message should be sent at the instant the `Send` function is executed or at the next Update.
Important: Messages sent in **Unity** from an asynchronous function that do not have the `isSenderAsync` parameter set to `true`, will not be executed correctly by **Unity**, that is why this parameter was created.

## `MessageSystemInstance.cs`

This class allows sending messages from asynchronous functions and messages from the `Unity` channel. You must not delete the `GameObject` created by this class.
You can use this `GameObject` to simulate sending messages.
