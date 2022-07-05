# Russlyman.Rcon
<p>
  <a href="https://github.com/Russlyman/Russlyman.Rcon/releases/latest" alt="Release">
    <img src="https://img.shields.io/github/v/release/Russlyman/Russlyman.Rcon" /></a>
  <a href="https://www.nuget.org/packages/Russlyman.Rcon" alt="Nuget">
    <img src="https://img.shields.io/nuget/v/Russlyman.Rcon" /></a>
  <a href="https://github.com/Russlyman/Russlyman.Rcon/blob/main/LICENSE" alt="License">
    <img src="https://img.shields.io/github/license/Russlyman/Russlyman.Rcon" /></a>
</p>

Russlyman.Rcon is a C# .NET library for sending RCon messages to game servers that implement the Quake III Arena RCon protocol.

While this library should be compatible with any game server that implements the Quake III Arena RCon protocol, it has only been tested and confirmed working with FiveM.

## Compatibility

- Quake III Arena
- FiveM (Confirmed)

## Basic Usage

#### Sends RCon command to server and writes response to console.

```csharp
using Russlyman.Rcon;
using System;

var rcon = new RconClient();
rcon.Connect("127.0.0.1", 30120, "fivem");
var reply = rcon.Send("restart Vita");
rcon.Dispose();

Console.WriteLine(reply);
```
## Download

- [Nuget](https://www.nuget.org/packages/Russlyman.Rcon)
- [Latest ZIP Release](https://github.com/Russlyman/Russlyman.Rcon/releases/latest)

## Documentation

### RconClient Class

```csharp
RconClient(int replyTimeoutMs = 3000)
```

#### Description

The RCon client.

#### Parameters

replyTimeoutMs - The timeout in milliseconds for how long the client should wait for a reply from the server after a command has been sent.

### Connect Method

```csharp
void Connect(string ip, int port, string password)
```

#### Description

Connects to a server.

This method can be used multiple times to connect to other servers without creating a new object.

#### Parameters

ip - The IP Address for the server.

port - The RCon port for the server.

password - The RCon password for the server.

### Send Method

```csharp
string Send(string command)
```

#### Description

Sends a command to the server.

#### Parameters

command - The command to send to the connected server.

#### Returns

The servers response to the sent command.

### SendAsync Method

```csharp
async Task<string> SendAsync(string command)
```

#### Description

Asynchronously sends a command to the connected server.

#### Parameters

command - The command to send to the server.

#### Returns

A task containing the servers response to the sent command.

### Dispose Method

```csharp
void Dispose()
```

#### Description

Disposes the class.

Should be called when you don't want to send anymore commands and connect to other servers.

## License

This project is licensed under MIT which can be viewed from the `LICENSE` file.
