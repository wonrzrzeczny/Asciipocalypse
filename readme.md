![Banner](/Screenshots/banner.png)

# Asciipocalypse

**3d first person shooter drawn using nothing but ascii characters!**

Delve deep into the dark dungeon full of terrifying ascii monsters in this roguelike first person shooter game. Level up your character (no pun intended) and make your way down through the floors, that become progressively harder and harder.

[Asciipocalypse](https://wonrzrzeczny.itch.io/asciipocalypse) is my submission for [DOS Games Jam](https://itch.io/jam/dos-games-jam).


The project
-----------
The game was made mainly for educational purposes. The main goal of this project was to implement a 3d real-time rendering algorithm based on rasterization, entirely from scratch. To truly capture the aesthetics of text-based DOS games, instead of drawing image pixel by pixel, I've decided to present rendered images using ascii characters, individualy selected per image pixel to add a greater sense of depth.

Everything shown on the screen (scene, UI, etc.) is first passed to a simple 8-bit color console simulation, whose content is drawn to the screen every frame (see [Console.cs](/ASCII_FPS/Console.cs) and [ASCII_FPS.cs Draw() method](/ASCII_FPS/ASCII_FPS.cs#L303)). The 3d image is created using triangle rasterization and visible surface determination is done via z-buffer (which is also used to select a character to be written to a console). Due to the nature of in-game levels, the portal rendering technique is used, which provides a significant speed-up. Full rendering code is located in [Rasterizer.cs](/ASCII_FPS/Geometry/Rasterizer.cs).


Screenshots
-----------
![](/Screenshots/game2.png)
![](/Screenshots/game4.png)
![](/Screenshots/game5.png)


Technology used
---------------
Game written in C# using [Monogame framework](https://www.monogame.net/).\
3d models created using [Blender](https://www.blender.org/).\
Textures created using [GIMP](https://www.gimp.org/).\
Sound effects created using [Magical 8bit VST plugin](http://www.ymck.net/en/download/magical8bitplug/).


Final remarks
-------------
The project started over a year ago and has already been through a major update. However, the game in terms of gameplay still lacks some content. I plan to keep working on the game (as long as I'll find time to do so :) ), to improve its replay value and to make overall experience more diverse. Therefore, feel free to leave any suggestions for new content/improvements that could be introduced. The same goes for glitches and/or performance issues you've encountered during your play.
