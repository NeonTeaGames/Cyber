Roadmap
=======
This file contains all the intended features for each version, so in addition to being a roadmap for the future, it acts as a changelog.

Yes, the plans go so far into the future that the universe will probably die a heat death before we are done with this list. Mostly because generating version names is fun.

Version 0.8.0 - Purgatory
-------------------------
Like the name suggests, the purgatory. This is our first _"release"_ and possibly
our first proper milestone as a game. This is quite an "all-around" update, but
mostly it'll focus on getting out a deathmatch-style gamemode to test out.
- [ ] Cleaning up and fixing some bugs
- [ ] Create an actual deathmatch map
- [ ] Create some proper deathmatch mechanics and stuff

Version 0.7.0 - Crazy
---------------------
- [ ] Generated room layouts.
- [ ] Generated interior items.
- [ ] Current "development room" should lead out to a corridor with doors to these generated rooms.

Version 0.6.0 - Snub
--------------------
- [ ] Character model with rig.
- [ ] Clothes models with rig.
- [ ] Pistol model.
- [ ] Furniture models: bed, nightstand, high lamp, wardrobe.
- [ ] Invent more roadmap

Version 0.5.0 - Overrate
------------------------
- [ ] Shooty guns.
- [ ] Health, with HUD.
- [ ] Death and respawning for players.

Version 0.4.0 - Hug
-------------------
- [x] Items and inventory.
- [x] Equipment, can wear items (just basic cubes) on head and hands.
- [x] Equipment, can cause some effect by "using" an item equipped in hand.
- [x] Reorganizing inventory and polishing stuff like that

Version 0.3.0 - Juniper
-----------------------
_(previously 0.2.0)_
- [x] Basic interactions and an environment for testing.
  - [x] A room with a door, a button and a screen.
  - [x] Both players should be able to open the door and press the button, and see a reaction in the world (door opening, screen flashing).

Version 0.2.0 - Mortician
-------------------------
_(previously 0.3.0)_
- [x] In-game console.
- [x] API for the console, should be able to print stuff on the screen on command.

Version 0.1.0 - Gigantic
------------------------
- [x] Spawning basic capsule characters.
- [x] ...With movement capabilities.
- [x] Client-server connectivity, players can see each other moving around.
  - [x] 1. Client-server connectivity: the clients can connect to the server and send some messages back and forth.
  - [x] 2. Players can see eachothers capsule-characters and send commands to move them around.
  - [x] 3. Basic syncing (Send sync packages at certain intervals) in order to sync positions correctly between server and client.
