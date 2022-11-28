# GuildChat
Co-operative client-server Discord chat bridge for FFXIV Free Company chat channels

### Roadmap
GuildChat intends to be a **one-way** Discord chat bridge - i.e. sending messages from FFXIV chat to Discord. GuildChat does not, and will not, intend to support sending messages from Discord to FFXIV. Such practice would likely be a violation of the FINAL FANTASY® XIV User Agreement.
#### Immediate
Near-term or currently working on goals:
- ASP.NET-based WebSocket co-ordinator (server)
  - [x] Listen for connections
  - [x] Keep connections alive
  - [ ] Enforce authorization flow
  - [ ] Verify Free Company members 
    - Likely using Lodestone profile text verification (standard practice with FFXIV services)
  - [ ] Record and send chat messages to Discord
- Dalamud plugin (client)
  - [ ] Connect to co-ordinator
  - [ ] Authorize
  - [ ] Send stream of Free Company chat messages if requested
  - [ ] Graceful disconnect
  - [ ] Allow user to change co-ordinator server address
 
 #### Far future
 Not planned or out-of-scope:
 - Linkshell support
 
 ### Unsolved architectural questions
  - How will Free Company users be verified?
  - How will the co-ordinator verify that plugin users are who they say they are?
    - A system similar to Mare Synchronos with user-verified secret keys?
  - How will coordinator access work?
    - An open-source model with a public coordinator server, but the option to use private servers
    
 ### Copyright
 GuildChat co-ordinator server, assets, and plugin source code &copy; 2022 Abyssal under the MIT License
