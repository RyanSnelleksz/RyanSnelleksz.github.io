
IMPORTANT: I cannot upload the build of this project to github because I am using a free account, however I have provided a link below to a google drive file
which is a build for you to check out.

Build: https://drive.google.com/file/d/1uTpzeIMxuYJ6Y1wJynWIsWKZr_ZBC6O7/view?usp=sharing

Dark Acropolis is a first person thriller game where you must explore a castle whilst evading a monster. In order to win you have to find and destroy all the seals
so that you can destroy the monsters heart which is also hidden in the castle. You need to stay hidden but if you are discovered, you are equipped with a gauntlet
which you can use to flash and blind the monster temporarily, to get away. However, you have limited charges to your gauntlet and need to break seals to get more.

This game was made in Unity and I worked on it with a team consisting of 3 designers, 4 artists and two programmers, including myself.

I was the lead programmer on the team and so my role, other documentation, consisted of creating the AI which was our major feature and some gameplay elements.

I specifically worked on all the AI, monster related stuff(including the animation controller but excluding sound), the gauntlet(animation and gems) and some 
other gameplay stuff.
My programming partner did the audio programming, UI programming and most of the gameplay like the character controller, heart, seals and gauntlet(flash mechanic).

The AI I chose to implement for the monster, was a AI directly inspired by Alien Isolation, where there is a monster AI and a director AI. The premise is that the
monster has no advantage over the player, information wise, and the director has information on both and works to make the game interesting by applying pressure
but also fair by releasing pressure when the player is seemingly in long high stress scenarios.

The way I've implemented this is by having the director measure the players stress based of player proximity to the monster, whether or not the player can see the
monster.
If those values are false, the 'menace gauge', which is a metric used by Alien Isolation to measure player stress, decreases and when it gets too low, the director
will give the monster a vague clue as to where the player might be. In our system, the map is seperated as areas and the areas are cut up as rooms. A vague clue in
our case is the director will tell the monster which area to search.
If the values are true however, when the 'menace gauge' gets too high, the monster will recieve no clues and will have to find the player with no information whatsoever.

I created 3 main scripts for this duo AI. The directior AI, the Monsters script for Line of sight/animations/sounds/etc for the monster, and a decision making script for the monster.
The director ended up being easier to implement than I had initially expected and the monster did take quite a-bit of work, especially when trying to make 
line of sight work favourbly for the monster but feel natural t othe player, however one of the biggest challenges I faced when creating the monsters decision making script. The challenge was to make the monster able to attempt to track the player when he lost them in a chase without and omniscience. Pretty much acting purely of line of sight and information such as the last room the monster saw the player in.
It didn't have to be very accurate but it needed to be reliable and intelligent to the player.

In order to solve this I theorised in my note book and had two initial ideas. It's important to note that the monster would always go to where he last saw the player first, in case he can see you.(For example when chasing around corners.)
My first idea was to have the monster guess off the adjacent rooms to the room the player or monster(depends on who and who isn't in a room as per how the monste rlast saw them) were last in. This sounded good but I knew that it would cause a-lot code for covering specific situations according to the monster and players positioning.
My second idea was to guess the room that was closest to the monster by navmesh path length(excluding the monsters last room), however I knew that this solution was very simple solution and when theorising in my notes I foresaw that there could be issues with this.

From here I decided to implement the easier solution and if that wasn't good enough I can try the other and if it was good, I could improve it later.
I went through both solutions and both seemed to be about as good as the other with the first idea still neding some better situation solutions.
So I went with a third idea of guessing the closest adjacent room to the last room that didn't backtrack through the last room, but when testing the idea in my notes
I found that the rooms that made the monster need to turn the least was often the best guess that would make the monster either guess the right room or move past 
the players hiding spot which would increase tension, and simply make least optimal choices less often.

So I ended up with a solution where the monster would take a guess from the rooms whose paths too started in front of the monster from a smaller field of view.
Later in the project this evolved to a smaller fov which would grow everytime the previous fov didn't produce good results.
