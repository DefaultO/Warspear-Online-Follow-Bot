### Warspear Online PoC
#### Updated Pointers for 9.3.3: https://github.com/DefaultO/Warspear-Online-Follow-Bot/blob/main/Warspear-Online-9.3.3.CT
#### "Tutorial" on how to update the Pointers in case they get outdated again. Or rather me recording on what I did to update the Pointers. https://youtu.be/sLFoqJx4hWk

Last Version of this Project I was willing to share to everyone for free. If you got more advanced Questions you can reach up to me. I don't play the Support Role for every basic Question though.

What you probably would want to know is that in order to start another Game Instance, you have to close the Mutex/Mutant Handle ``..\BaseNamedObjects\E49C5EC071B44f14B2`` of the Warspear Process. See: https://github.com/DefaultO/Warspear-Online-Follow-Bot/blob/main/src/Warspear-Online-Follow-Bot-Test/Program.cs#L289

I didn't clean the Code, because it already contains Gems like printing the World to Console. A simple for-in-for loop, but quite satisfying to watch. Looks cool how the console boxes change it's color based on the byte that stands in it.

## My Developing Road afterwards
Here's the Story though of what I have done without anyone knowing, till now. I thought some might learn out of it, to not give up if you can't use the direct path to the solution. Also I don't see many stories about how people reversed/build their logic for their programs. Here's mine.

I had no Gold and couldn't afford amping my Equipment to +10, that's why shortly after the source release, there were made a couple of Adjustments you could say. I wanted to make a Bot now that earns me some Gold without me doing anything. This is a small write-up on what I had to solve during the development of the (now) private project.

The Version you can find on GitHub, already contains my stupid idea (but nice workaround) of looping through the whole map with the cursor and reading out the Cursor Flag of it. I do it this way, because I couldn't find the World Data and Entity List by myself. This Idea had one single disadvantage, it's slow. I had to built-in a forced Sleep between every tile hop because the game doesn't update the Cursor Flag Value that fast. But I mean, if it works, it works, right? <- Don't do this at home. Wrong attitude.

If you want to change the Cursor Position for yourself, but encounter, that the Cursor gets setback. Here's a simple Patch for that one. If there was a huge game update and that address changed, look what accesses the Cursor Position. You probably will find the right one on your own. Nop the whole instruction.
```csharp
// Original Bytes: 66 89 41 08
mem.WriteMemory("warspear.exe+0x19708A", "bytes", "90 90 90 90");
// Original Bytes: 66 89 41 0A
mem.WriteMemory("warspear.exe+0x1970B1", "bytes", "90 90 90 90");
```

So what exactly does that mean?
- World Data

Yes. I unironically have the World Data now. If you have all possible Cursor Flags you can map a room out. It works like this.
- Tiles you can walk on (Hollow).
- Tiles you can't walk on (Cross).
- Interchange Sections (Arrow). Tiles you can walk on but teleport you to the with it connected room.
- Enemies (Attack/Sword). Also Tiles you can walk on.

Those are the basic Cursor Flags you can get. There are more like 'something you can use' (Hand), 'talk to a NPC' (Several different Icons), 'loading' (Loading Circle), and so on. But this really is enough to know where you can go, and where you can not go.

But how does this help working on a Bot? Well, you can already attack Mobs and know where you can walk to, the in-game path-finding does the rest. But there is still a problem. I don't know which room I am in, means the Bot can't yet know which rooms it has to enter to come to it's destination. What I did was googling and asking around if someone worked with Matrix Anomalies yet. Matrix because you can display the World Data as a two dimensional 28x28 Grid giving every small box a number.

I had no luck doing so but what my Idea was is to give the rooms some own ID and detect a room and the resulting ID with it based on specific Map Layouts. I had to discontinue this Idea because it took too long and there are hardly any writings about the idea of detecting anomalies that are easy to understand and don't require you to have gone to Oxford or something.

What did I do instead? Everyone likes Puzzles, right? I don't like them, I find them boring. But that gave me an Idea. What if you only care about the room you started in to build out the huge map of rooms?

<p align="center">
  <img src="https://i.pinimg.com/originals/bf/54/9a/bf549a0cd4c7f2a062167dfa1209b081.gif" width="350" title="hover text">
</p>

Give the current room you are standing in the ID 1.
| ID: 1    |
| ---      |

Enter the Room on the right through the interchange section. That Room has got the ID 2.
| ID: 1    | ID: 2    |
| ---        | ---          |

Go back to the Room with the ID 1. Go to the Room on the left. You are now in the Room with the ID 1, and the original Room with the ID 1 turned into Room ID 2.
| ID: 1    | ID: ~~1~~ 2   | ID: ~~2~~ 3   |
| ---        | ---          | ---          |

Hope you can imagine that, I kept it simple and only went Horizontal, as it even gets more complex vertically. Even if it looks not complex anymore because I added those tables for easier understanding. Coding the logic so that the rooms are dynamic located in a three dimensional list is not simple. Three dimensional because if you enter caves you go a level down. In short you can say that the numbers become bigger from left to right, from top to down and from high to low. And to confuse you even more, you can allign all the islands to each other too. That would make a cool World Map if I ever come to turn values into visible data/stats you can look at, 3D graphs or something.

<p align="center">
  <img src="https://cdn.dribbble.com/users/846207/screenshots/5645189/isometric_cube_animation.gif" width="350" title="hover text">
</p>

Complexity paid out though. I can tell the Bot now to go to the Room with ID: 41, X: 6, Y: 10, and it would go there, on the exact specified tile, all on it's own. Also the rooms know all of it's neighbours too. So the Bot doesn't have to access the whole World just to get the room that would come next. It only accesses the whole World when there is a new Entry, and then only to add an entry.

So nothing stopped me from doing my Bot now. I did set-up my paths to the farm Spots. Tested the walking. And since everything worked fine I moved on to the actual farming logic (later-on added the option to use the mc mace class skill "wolf spirit" which gives you a speed buff). I decided to work on the game logic in the ice caves of the winterfest event in the game, there are no rats, and the mobs can drop 20-50 gold. which in the end alone is enough gold to buy a repair scroll for your whole near broken equipment. it repairs everything you wear and have in your inventory, so extra equip can actually benefit you as you can farm that way longer, means you get more gold for the same process.

The actual fighting logic looks like this. It searched the area around you using the cursor and cursor flag for the enemy. Since the Bot was made for my Account in the first place, I expect it to not die within the first fight. There are skill combos you do once in a while when you have a lot of mana and having high mana regen made my bot be able to use skills more often, means enemies die even faster, or I survive longer. In case my health drops low mid-fight, or after a fight, the bot gets told to run away to the next room, then back to the previous room and a safe spot next to the interchange section. If the bot died, it just went the path back (if the bot died too fast, the bot has to expect an enemy player from the other faction killed it, then it sleeps for 10 minutes after respawning, and only then walks back). And this got run 24/7 at some point. Since I had my inventory maxed out, that wasn't a problem. This made the price of Krakatau Nuts sink drastically from 1500 Gold/Set to 1000 Gold/Set, to 750 Gold/Set, to 500 Gold/Set, to 300 Gold/Set. I overprovided the Market with those Nuts, which made the price sink. Also there were people that tried to underprice me. I made from the Winterfest alone a profit of 2.000.000 Gold (200.000 Gold is equal to 8€). All I asked for.

And that basically is it. I of course added first tries of a leveling Bot (you can sell Level 20 Accounts for 20€ each), and other experiments, so that the code had multiple 'while (true)' loops that I commented out when I didn't need them at the moment. Cheers.
