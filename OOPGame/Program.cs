string? gameSize; int dungeonSize = 4, pitAmount = 1, maelstromAmount = 1, amarokAmount = 1, mapAmount = 1;

// lets the player choose the game configuration
Console.Write("Pick the game size: small, medium or large. ");
gameSize = Console.ReadLine()?.ToLower();

switch (gameSize)
{
    case "small":
        dungeonSize = 4;
        pitAmount = 1;
        maelstromAmount = 1;
        amarokAmount = 1;
        mapAmount = 1;

    break;
    case "medium":
        dungeonSize = 6;
        pitAmount = 2;  
        maelstromAmount = 1;
        amarokAmount = 2;
        mapAmount = 1;

    break;
    case "large":
        dungeonSize = 8;
        pitAmount = 4;
        maelstromAmount = 2;
        amarokAmount = 3;
        mapAmount = 2;

    break;
    default:
        Console.WriteLine("Not a valid size...Creating a small game!");

    break;
}
Console.WriteLine($"{dungeonSize}x{dungeonSize} dungeon created!");


// initialize a Game instance and uses it's Run method to start the game
Game jogo = new Game(dungeonSize, pitAmount, maelstromAmount, amarokAmount, mapAmount);
jogo.Run();


public record Game
{
    public Fountain _fountain {get; set;}
    public Grids[,] _dungeon {get; set;}
    public Player _player {get; set;}
    public int _gridSize {get; set;}

    // Game constructor
    public Game(int gridSize, int pitAmount, int maelstromAmount, int amarokAmount, int mapAmount)
    {

        _gridSize = gridSize;
        _dungeon = new Grids[gridSize,gridSize];

        // Set every dungeon's tile with the nothing grid, then procceed to fill it with the other grids
        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                _dungeon[i,j] = Grids.Nothing;
            }
        }

        // initialize a random object, used to randomize the position of the tiles
        Random random = new Random();

        _dungeon[0,random.Next(1,gridSize)] = Grids.Fountain;
        _dungeon[0,0] = Grids.Exit;
        

        for(int i = 0; i < pitAmount; i++)
        {
            int x, y;

            do
            {
                x = random.Next(0,gridSize);
                y = random.Next(0,gridSize);

            }while(_dungeon[x,y] != Grids.Nothing);    

            _dungeon[x,y] = Grids.Pit;
        
        }

        for(int i = 0; i < maelstromAmount; i++)
        {
            int x, y;

            do
            {
                x = random.Next(0,gridSize);
                y = random.Next(0,gridSize);

            }while(_dungeon[x,y] != Grids.Nothing);    

            _dungeon[x,y] = Grids.Maelstrom;
        
        }

        for(int i = 0; i < amarokAmount; i++)
        {
            int x, y;

            do
            {
                x = random.Next(0,gridSize);
                y = random.Next(0,gridSize);

            }while(_dungeon[x,y] != Grids.Nothing);    

            _dungeon[x,y] = Grids.Amarok;
        
        }

        for(int i = 0; i < mapAmount; i++)
        {
            int x, y;

            do
            {
                x = random.Next(0,gridSize);
                y = random.Next(0,gridSize);

            }while(_dungeon[x,y] != Grids.Nothing);    

            _dungeon[x,y] = Grids.Map;
        
        }


        _player = new Player(0,0);
        _fountain = Fountain.Disabled;

    }

    // the main method 
    public void Run()
    {
        
        // prints the initial menu once
        InitialMenu();


        while(_player.inDungeon)
        {
            // player wins
            if(_dungeon[_player.Row,_player.Column] == Grids.Exit && _fountain == Fountain.Enabled)
            {
                Console.WriteLine("Fountain of objects has been reactivated, and you have escaped with your life!");
                Console.WriteLine("You win!");
                _player.inDungeon = false;

            }

            // player step on a pit and loses
            if (_dungeon[_player.Row, _player.Column] == Grids.Pit)
            {
                Console.WriteLine("You have fallen into a pit!");
                Console.WriteLine("You lose!");
                _player.inDungeon = false;    
            }
            
            // pĺayer steps on a maelstrom
            if (_dungeon[_player.Row, _player.Column] == Grids.Maelstrom)
            {

                // removes the maelstrom from the tile
                Console.WriteLine("A maelstrom got you! you'll get moved one space north and two spaces east");
                _dungeon[_player.Row, _player.Column] = Grids.Nothing;

                // sets the maelstrom in a new tile
                if(_player.Row + 1 == _gridSize && _player.Column - 2 >= 0)
                {
                    _dungeon[_player.Row, _player.Column - 2] = Grids.Maelstrom;

                }else if(_player.Row + 1 <= _gridSize - 1 && _player.Column - 2 < 0)
                {
                    _dungeon[_player.Row + 1, _player.Column] = Grids.Maelstrom;
                
                }
                else
                {
                    _dungeon[_player.Row + 1, _player.Column - 2] = Grids.Maelstrom;
                    
                }

                // moves the player
                _player.Move("north", _gridSize);
                _player.Move("east",_gridSize);
                _player.Move("east",_gridSize);

            }

            // player steps on a Amarok
            if(_dungeon[_player.Row, _player.Column] == Grids.Amarok)
            {
                Console.WriteLine("You got killed by a amarok!");
                Console.WriteLine("You lose!");
                _player.inDungeon = false;

            }

            if(_dungeon[_player.Row, _player.Column] == Grids.Map)
            {
                Console.WriteLine("You found a map!");
                Console.WriteLine("use the action :'use map' to reveal the dungeon!");
                _player.map = true;

            }

            // verifies if the player still in the dungeon, if so, it reads the player's action
            if(_player.inDungeon)
            {
                
                Menu();

                string? action;
                action = Console.ReadLine().ToLower();
                string[] commands = action.Split(' ');
                
                if(commands.Length < 2)
                {
                    Console.WriteLine($"{commands[0]} is not a valid command by its own, try using the help command");
                
                }else
                {
                    PlayerAction(commands);
                }
                Console.Clear();
            }

        
        }

    }

    public void InitialMenu()
    {
        Console.WriteLine("____________________________________________________________________________________________________________________");
        Console.WriteLine("| You enter the Cavern of Objects, a maze of rooms filled with dangerous pits in searchof the Fountain of Objetcs.");
        Console.WriteLine("| Light is visible only in the entrance, and no other light is seen anywhere in the caverns.");
        Console.WriteLine("| You must navigate the Caverns with your other senses.");
        Console.WriteLine("| Find the Fountain of Objects, activate it, and return to the entrance.");
        Console.WriteLine("| Look out for pits. You will feel a breeze if a pit is in an adjacent room. If you enter a room with a pit, you will die.");
        Console.WriteLine("| Maelstroms are violent forces of sentient wind. Entering a room with one will transport you to any other location in the caverns. You will be able to hear their growling in the nearby rooms.");
        Console.WriteLine("| Amaroks roam the caverns. Encontering one is certain death, but you can smell their rotten stench in nearby rooms.");
        Console.WriteLine("| You carry with you a bow and a quiver of arrows. You can use them to shoot monsterns in the caverns but be warned: you have a limited supply");
        Console.WriteLine("____________________________________________________________________________________________________________________\n");

    
    }

    // A method that prints the game status
    public void Menu()
    {

        Console.WriteLine($"\nFountain status: {_fountain}");
        Console.WriteLine($"Arrows ({_player.Arrows}/5)");
        Console.WriteLine("____________________________________________________________________________________________________________________");
        Console.WriteLine($"You are in the room at (Row = {_player.Row + 1}, Column = {_player.Column + 1}).");

        _player.CheckSurroundings(_dungeon, _fountain);

        Console.Write($"\nWhat do you want to do ? ");

    }

    // A method that handles the player's input
    public void PlayerAction(string[] commands)
    {
        switch(commands[0])
        {
            case "move":
                _player.Move(commands[1], _gridSize);
            break;
            
            case "shoot":
                _player.Shoot(commands[1], _dungeon);
            break;
            
            case "enable":
                _fountain = _player.EnableFountain(_fountain, _dungeon);
            break;

            case "help":
                _player.GetHelp();
            break;
            
            case "use":
                _player.Use(commands[1], _gridSize, _dungeon);
            break;
            
            default:
                Console.WriteLine($"Command {commands[0]} not valid!");
            break;
        }
    }

}

public record Player
{
    public int Row {get; set;}
    public int Column {get; set;}
    public int Arrows {get; set;}
    public bool map {get; set;}
    public bool inDungeon {get; set;}

    // player construction
    public Player(int Row, int Column)
    {
        
        this.Row = Row;
        this.Column = Column;
        this.Arrows = 5;
        this.map = false;
        inDungeon = true;

    }  

    // let's the player enable the fountain
    public Fountain EnableFountain(Fountain fountain, Grids[,] dungeon)
    {
        //checks if the fountain is already enabled
        if(fountain == Fountain.Enabled && dungeon[Row, Column] == Grids.Fountain)
        {
            Console.WriteLine("The fountain is already enabled!");
            return fountain;
        }

        // checks if the player is in the fountain tile
        if(dungeon[Row, Column] == Grids.Fountain)
        {
            fountain = Fountain.Enabled;
            return fountain;
        }
        
        Console.WriteLine("You're not in the fountain room!");
        return fountain;

    }

    // let's the player moves in four directions
    public void Move(string direction, int gridSize)
    {

        switch(direction)
        {
            case "north":
                
                if (Row == 0)
                {
                    Console.WriteLine("You cannot go further in that direction!\n");
                    break;
                }
                Console.WriteLine("\nMoved to the north!\n");
                Row--;

            break;
            case "south":
                
                if (Row == gridSize - 1)
                {
                    Console.WriteLine("You cannot go further in that direction!\n");
                    break;
                }
                Console.WriteLine("\nMoved to the south!\n");

                Row++;

            break;
            case "west":
                
                if (Column == 0)
                {
                    Console.WriteLine("You cannot go further in that direction!\n");
                    break;
                }
                Console.WriteLine("\nMoved to the west!\n");

                Column--;
            
            break;
            case "east":
                if (Column == gridSize - 1)
                {
                    Console.WriteLine("You cannot go further in that direction!\n");
                    break;
                }
                Console.WriteLine("\nMoved to the east!\n");
        
                Column++;

            break;
            default:
                Console.WriteLine("Not a valid direction! ");

            break;

        }
    }

    // let's the player shoots in four directions
    public void Shoot(string direction, Grids[,] dungeon)
    {
        if(Arrows <= 0)
        {   
            Console.WriteLine($"You've got no arrows left!");
            return;
        }

        switch(direction)
        {
            case "north":
                
                if (Row > 0 && dungeon[Row-1, Column] == Grids.Amarok || Row > 0 && dungeon[Row - 1, Column] == Grids.Maelstrom)
                {
                    Console.WriteLine($"You shot a {dungeon[Row - 1, Column]}!");
                    dungeon[Row - 1, Column] = Grids.Nothing;

                }
                else
                {
                    Console.WriteLine("You shot in a empty room!");
                
                }

                Arrows--; 


            break;

            case "south":
                
                if (Row < dungeon.GetLength(0) - 1 && dungeon[Row + 1, Column] == Grids.Amarok ||Row < dungeon.GetLength(0) - 1 && dungeon[Row + 1, Column] == Grids.Maelstrom)
                {
                    Console.WriteLine($"You shot a {dungeon[Row + 1, Column]}!");
                    dungeon[Row + 1, Column] = Grids.Nothing;

                }
                else
                {
                    Console.WriteLine("You shot in a empty room!");
                
                }

                Arrows--; 

            break;

            case "west":
                
                if (Column > 0 && dungeon[Row, Column - 1] == Grids.Amarok || Column > 0 && dungeon[Row, Column - 1] == Grids.Maelstrom)
                {
                    Console.WriteLine($"You shot a {dungeon[Row, Column - 1]}!");
                    dungeon[Row, Column - 1] = Grids.Nothing;

                }
                else
                {
                    Console.WriteLine("You shot in a empty room!");
                
                }

                Arrows--; 
            
            break;

            case "east":
                if (Column < dungeon.GetLength(0) - 1 && dungeon[Row, Column + 1] == Grids.Amarok || Column < dungeon.GetLength(0) - 1 && dungeon[Row, Column + 1] == Grids.Maelstrom)
                {
                    Console.WriteLine($"You shot a {dungeon[Row, Column + 1]}!");
                    dungeon[Row, Column + 1] = Grids.Nothing;

                }
                else
                {
                    Console.WriteLine("You shot in a empty room!");
                
                }

                Arrows--; 
            break;
            
            default:

                Console.WriteLine("Not a valid direction! ");
                
            break;
        }
    }
    
    // a method to check the player surrounding tiles
    public void CheckSurroundings(Grids[,] dungeon, Fountain fountain)
    {   
        int size = dungeon.GetLength(0);
        
        // check if player is on the exit grid
        if(dungeon[Row,Column] == Grids.Exit)
        {
            Console.WriteLine($"You see light coming from the cavern entrance");
        }

        // check if player is on the fountain grid
        if(dungeon[Row,Column] == Grids.Fountain)
        {
                
            if(fountain == Fountain.Disabled)
            {
                Console.WriteLine($"You hear water dripping in this room. The fountain of objects is here!");
                    
            }else
            {
                Console.WriteLine($"You hear the rushing waters from the Fountain Of Objects. It has been reactivated!");
            }

        }

        // check for surrounding pits
        if ((Row > 0 && dungeon[Row - 1, Column] == Grids.Pit) ||
            (Row < size - 1 && dungeon[Row + 1, Column] == Grids.Pit) ||
            (Column > 0 && dungeon[Row, Column - 1] == Grids.Pit) ||
            (Column < size - 1 && dungeon[Row, Column + 1] == Grids.Pit))
        {
            Console.WriteLine("There is a draft of air pushing through the pits into adjacent rooms");
        }

        //  check for surrounding maelstroms
        if ((Row > 0 && dungeon[Row - 1, Column] == Grids.Maelstrom) ||
            (Row > 0 && Column > 0 &&  dungeon[Row - 1, Column - 1] == Grids.Maelstrom) ||
            (Row > 0 && Column < size - 1 && dungeon[Row - 1, Column + 1] == Grids.Maelstrom) ||
            (Column > 0 && dungeon[Row, Column - 1] == Grids.Maelstrom) ||
            (Column < size - 1 && dungeon[Row, Column + 1] == Grids.Maelstrom) ||
            (Row < size - 1 && dungeon[Row + 1, Column] == Grids.Maelstrom) ||
            (Row < size - 1 && Column > 0 && dungeon[Row + 1, Column - 1] == Grids.Maelstrom) ||
            (Row < size - 1 && Column < size - 1 && dungeon[Row + 1, Column + 1] == Grids.Maelstrom))
        {
            Console.WriteLine("You hear the growling and groaning of a maelstrom nearby.");
        }

        // check for surrounding amaroks
        if ((Row > 0 && dungeon[Row - 1, Column] == Grids.Amarok) ||
            (Row > 0 && Column > 0 &&  dungeon[Row - 1, Column - 1] == Grids.Amarok) ||
            (Row > 0 && Column < size - 1 && dungeon[Row - 1, Column + 1] == Grids.Amarok) ||
            (Column > 0 && dungeon[Row, Column - 1] == Grids.Amarok) ||
            (Column < size - 1 && dungeon[Row, Column + 1] == Grids.Amarok) ||
            (Row < size - 1 && dungeon[Row + 1, Column] == Grids.Amarok) ||
            (Row < size - 1 && Column > 0 && dungeon[Row + 1, Column - 1] == Grids.Amarok) ||
            (Row < size - 1 && Column < size - 1 && dungeon[Row + 1, Column + 1] == Grids.Amarok))
        {
            Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room.");
        }
    
    }

    // let's the player see possible actions
    public void GetHelp()
    {
        
        Console.WriteLine("____________________________________________________________________________________________________________________");
        Console.WriteLine("| You can move NORTH, SOUTH, EAST or WEST using the command 'move'");
        Console.WriteLine("| You can shoot NORTH, SOUTH, EAST or WEST using the command 'shoot'");
        Console.WriteLine("| You can enable the fountain of objects using the command 'enable fountain'");
        Console.WriteLine("____________________________________________________________________________________________________________________");

    }

    // let's the player use different items
    public void Use(string item, int gridSize, Grids [,] dungeon)
    {
        switch(item)
        {
            case "map":
            if(this.map == true)
            {
                Console.WriteLine("");
                for(int i = 0; i < gridSize; i++)
                {
                    
                    for(int j = 0; j < gridSize; j++)
                    {
                        if(dungeon[i,j] == Grids.Exit)
                            Console.Write("E ");
                        
                        if(dungeon[i,j] == Grids.Amarok)
                            Console.Write("A ");
                        
                        if(dungeon[i,j] == Grids.Maelstrom)
                            Console.Write("M ");
                        
                        if(dungeon[i,j] == Grids.Pit)
                            Console.Write("P ");
                        
                        if(dungeon[i,j] == Grids.Fountain)
                            Console.Write("F ");
                        
                        if(dungeon[i,j] == Grids.Map)
                            Console.Write("M ");
                        
                        if(dungeon[i,j] == Grids.Nothing)
                            Console.Write("N ");

                        if(i == this.Row && j == this.Column)
                            Console.Write("Y ");
                    }
                    Console.WriteLine("");
                    
                }
            }else
            {
                Console.WriteLine("You do not have a map!");

            }
            break;
            
            default:
                Console.WriteLine("Not a valid item");

            break;
        }
    }

}

public enum Grids {Nothing, Exit, Fountain, Pit, Maelstrom, Amarok, Map};
public enum Fountain {Enabled, Disabled};