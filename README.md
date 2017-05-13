Cyber
=====
A cyberpunk roleplaying game with online connectivity, made in Unity3D.

How to play
-----------
Currently there are no binary releases, so you're going to have to set up a dev environment to play the game.
1. Install [Unity](https://unity3d.com/).
2. Clone this repo. `git clone https://github.com/Saltosion/Cyber`
3. Download the assets from the [Dependencies](#dependencies) list, and put them in /path/to/Cyber/Assets/AssetStoreTools.
4. Launch Unity, open this repo's directory as a project.
5. In Unity, go to File -> Build Settings -> Build (remember to choose your platform from the list).
6. Play the game you just built! Note: You'll need to run the game two times, one for the server, one for the client. On the server, run `host` in the in-game console, on the client, run `join`. You can toggle the in-game console with the Tab key.

Roadmap
-------
See [the roadmap](ROADMAP.md).

Design
------
See [game design document](GDD.md).

Dependencies
------------
Before doing anything in Unity, install these into the /Assets/AssetStoreTools directory. Note that it's in the gitignore, so you need to create the directory yourself.
- [Post Processing Stack](https://www.assetstore.unity3d.com/en/#!/content/83912)

License
-------
The monospace font used is VT323 which is licensed under the [SIL Open Font License version 1.1](LICENSE-VT323.txt).

All of the assets created specifically for this project (ie. not mentioned above) are licensed under the CC-BY-NC-SA 4.0 license.
[![CC-BY-NC-SA](https://i.creativecommons.org/l/by-nc-sa/4.0/80x15.png)](https://creativecommons.org/licenses/by-nc-sa/4.0/)
