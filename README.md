
# Warspear Online Follow Bot
Tired of not having any teammates in this Game to fight a boss? Easy Solution. Be your own Team! This Project is all you need to take quest bosses down. Esspecially here, because you only need 1 Tank Class, 1 Damage Class and a few Healers. See Below.

### Note: You will need multiple accounts to play multiple characters. They only allow one character per account to be online! This Tool makes it possible tho to run multiple game instances. Means you only need one semi-good PC (1 warspear opened is 120MB of RAM) to run all your accounts!

[![](https://res.cloudinary.com/marcomontalbano/image/upload/v1609086121/video_to_markdown/images/streamable--jeqbi5-c05b58ac6eb4c4700831b2b3070cd403.jpg)](https://streamable.com/jeqbi5 "")

This Tool got made in a couple of hours and attacks a problem a lot of new players have - not having any mates and not being interested to beg in the chat for someone to help you or get lucky and meet someone who needs to do the same boss. It is in an early stage and is not optimized. Feel free to grab some code partions. I am pretty sure some will copy the Multibox thing, because that alone makes the life easier for so many people playing this game. Having more than 1 Game Instance opened. I would love to see ideas contributed to this POC though as everything merged makes the end-user-experience better. And I am not creative enough to come up with neat features.

Here are the used Pointers by the way to copy-paste, as I am not sure when I will be able to clean the code to make it a whole Repository worth:

*Maybe worth mentioning that those aren't from the steam version, they might work tho on the steam one. If they don't. get the official Windows Client from here: https://warspear-online.com/*

**Memory.dll String Format Pointers:**
```
Current HP:       warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,9C    [Type: 4 Bytes]
Max HP:           warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,A0    [Type: 4 Bytes]

Current MP:       warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,A4    [Type: 4 Bytes]
Max MP:           warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,A8    [Type: 4 Bytes]

Position X:       warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,B0    [Type: 2 Bytes]
Position Y:       warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,B2    [Type: 2 Bytes]

Destination X:    warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,B4    [Type: 2 Bytes]
Destination Y:    warspear.exe+0x005A8BF8,0,10,D4,4,B8,4C,B6    [Type: 2 Bytes]
```
**Pointers in my Format:**
```
```

## Features it already got:
- Multibox Bypass
- Walk to your Main Character (your "bots" will attack if you are lucky enough when you attack a monster, because they try to walk to where you want to walk).

## Coming Soon (TM) Features
- Auto Heal your Main Character when low.
- Switch to a different mage when Mana is low or there is a cooldown.
- A lot of Optimization / Recoding as this is only a Base Concept
- Create World Routes so you don't have to set-up all your bots into the same room. Not sure if this is even possible. But I mean, the game must know on it's own what room you switch to/enter, right?
- Autofarmer(s). Not sure if I want that as this tool on it's own shouldn't break the economy but only help you fight bosses because of the lack of friendly people/players due to the game dying.

## Already known Issues:
- Interception Sections are Buggy af. The "Bots" will reenter the previous Room and walk to where you would technically stand based on the coords they calculate with. You will need to manually intefer. Don't close the program. Just spam until it switches to the right room and walk a few tiles aways. Then it will get to you on it's own.

Possible Fix: There is a flag when you enter such a zone. If I check for it, then determine which side I entered the current room from based on the current coordinates, I could make them walk a few tiles to the center. This should solve the problem. But it really is a ghetto way to solve it. I am open for Ideas.
