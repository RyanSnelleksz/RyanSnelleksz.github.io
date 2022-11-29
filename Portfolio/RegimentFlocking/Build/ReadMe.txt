This is a regiment/flocking AI by Ryan Snelleksz.




Demo Controls:

Scroll with the mouse to zoom in and out.

WASD and arrow keys to move the camera.

Left click to choose a new locationm to move the blue army which belongs to the player.
NOTE: It will take a second to update the pathing visuals because it waits for their last move to finish before re-routing.




The goal of this project was to make a library project in which someone could feed relevant information to the 'War Manager' and it would feed new positions
back.
This was made using my own physics engine and a Line Renderer I was given during an old product. I did not make the line renderer and I used the glm and
GLFW libraries as well but the rest is my own. Imgui is also present but it was not used.

This was made by implementing Dikstra pathfinding and a grid based node map. The way the AI works is that you set it up by creating 'Clone Armies' that have
'Clones'. This represent the data of the armies and units inside the armies of the program that is using the library.
It also has a war manager which essentially is a black board with some additional function.

The armies also have stances in which they can take. These include, Hold, Attack, Move and Flee.

When holding armies will stay still, however if another enemy army is in range and in line of sight, they will turn to face that army.
If set to attack, the army will pursue any army they can see.
If set to move, the army will move to whatever location they where told to move to anf then will hold.
Armies who have lost too many units will take the flee stance and will break formation, and each unit will run directly away from the enemy army.

The army clone AI will provide positions for the soldiers by calculating using the current amount of soldiers remaining and the set values for spacing and the soldiers then seek out those positions.