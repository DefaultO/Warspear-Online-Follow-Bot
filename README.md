Last Version of this Project I was willing to share to everyone for free. If you got more advanced Questions you could reach up to me. I don't play the Support Role for every basic Question though.

I had no Gold and couldn't afford amping my Equipment to +10, that's why shortly after the source release I made a couple of Adjustments you could say. I wanted to make a Bot now that earns me some Gold without me doing anything. This is a small write-up on what I had to solve during the development of the (now) private project.

The Version you can find on GitHub, already contains my stupid idea (but nice workaround) of looping through the whole map with the cursor and reading out the Cursor Flag of it. I do it this way, because I couldn't find the World Data and Entity List by myself.

This came with a Problem:
- Definetly slower than what the game does to get Map. Since the Game doesn't update the Cursor Flag Value that fast, you have to implement a forced Sleep inbetween every Tile.

This Issue was a bigger problem. And I couldn't solve it by finding how the game determines which room you are in right now. So my base idea was to cache all rooms you enter from now on & save them to disk (neccessary, to not lose the cache upon program close).

