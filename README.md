Last Version of this Project I was willing to share to everyone for free. If you got more advanced Questions you can reach up to me. I don't play the Support Role for every basic Question though.

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

But how does this help working on a Bot? Well, you can already attack Mobs and know where you can walk to, the in-game path-finding does the rest. But there is still a problem. I don't know which room I am in, means the Bot can't yet know which rooms it has to enter to come to it's destination. What I did was googling and asking around if someone worked with Matrix Anomalies yet. What my Idea was is to give the rooms some own ID and detect a room and the resulting ID with it based on specific Map Layouts.

