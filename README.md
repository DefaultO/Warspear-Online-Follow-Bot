Last Version of this Project I was willing to share to everyone for free. If you got more advanced Questions you can reach up to me. I don't play the Support Role for every basic Question though.

Here's the Story though of what I have done without anyone knowing till now. I thought some might learn out of it, to not give up if you can't use the direct path to the solution. Also I don't see many stories about how people reversed/build their logic for their programs. Here's mine.

I had no Gold and couldn't afford amping my Equipment to +10, that's why shortly after the source release I made a couple of Adjustments you could say. I wanted to make a Bot now that earns me some Gold without me doing anything. This is a small write-up on what I had to solve during the development of the (now) private project.

The Version you can find on GitHub, already contains my stupid idea (but nice workaround) of looping through the whole map with the cursor and reading out the Cursor Flag of it. I do it this way, because I couldn't find the World Data and Entity List by myself. This Idea had one single disadvantage, it's slow. I had to built-in a forced Sleep between every tile hop because the game doesn't update the Cursor Flag Value that fast. But I mean, if it works, it works, right? <- Don't do this at home. Wrong attitude.

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

Give the current room you are standing in the ID 1.
| ID: 1    |
| ---      |

Enter the Room on the right through the interchange section. That Room has got the ID 2.
| ID: 1    | ID: 2    |
| ---        | ---          |

Go back to the Room with the ID 1. Go to the Room on the left. You are now in the Room with the ID 1, and the original Room with the ID 1 turned into Room ID 2.
| ID: 1    | ID: ~~1~~ 2   | ID: ~~2~~ 3   |
| ---        | ---          | ---          |

Hope you can imagine that, I kept it simple and only went Horizontal, as it even gets more complex vertically. Even if it looks not complex anymore because I added those tables for easier understanding. Coding the logic so that the rooms are dynamic located in a two dimensional list is not simple.
| ID: 1    | ---      | ID: 2      |
| ---      | ---      | ---      |
| **ID: ~~1~~ 3**   | **ID: ~~2~~ 4**  | **ID: ~~3~~ 5**  |
