
namespace Battleship.Ascii
{
    using Battleship.Ascii.TelemetryClient;
    using Battleship.GameController;
    using Battleship.GameController.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Metrics;
    using System.Linq;

    public class Program
    {
        private static List<Ship> myFleet;

        private static List<Ship> enemyFleet;

        /// <summary>
        /// my missed for gameboard view
        /// </summary>
        private static List<Position> myMisses;

        /// <summary>
        /// enemy missed for gameboard view
        /// </summary>
        private static List<Position> enemyMisses;

        /// <summary>
        /// satellite view - allows you to see enemy ships
        /// </summary>
        private static bool satellite = false;

        /// <summary>
        /// baby fleet mode - only one ship on the board for each player
        /// </summary>
        private static bool babyFleet = false;

        private static ITelemetryClient telemetryClient;

        static void Main(string[] args)
        {

            //satellite mode
            if (args.Contains("/satellite"))
            {
                satellite = true;
            }

            //baby flet mode
            if (args.Contains("/babyfleet"))
            {
                babyFleet = true;
            }

            telemetryClient = new ApplicationInsightsTelemetryClient();
            telemetryClient.TrackEvent("ApplicationStarted", new Dictionary<string, string> { { "Technology", ".NET" } });
            try
            {
                Console.Title = "Battleship";
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                Console.WriteLine("                                     |__");
                Console.WriteLine(@"                                     |\/");
                Console.WriteLine("                                     ---");
                Console.WriteLine("                                     / | [");
                Console.WriteLine("                              !      | |||");
                Console.WriteLine("                            _/|     _/|-++'");
                Console.WriteLine("                        +  +--|    |--|--|_ |-");
                Console.WriteLine(@"                     { /|__|  |/\__|  |--- |||__/");
                Console.WriteLine(@"                    +---------------___[}-_===_.'____                 /\");
                Console.WriteLine(@"                ____`-' ||___-{]_| _[}-  |     |_[___\==--            \/   _");
                Console.WriteLine(@" __..._____--==/___]_|__|_____________________________[___\==--____,------' .7");
                Console.WriteLine(@"|                        Welcome to Battleship                         BB-61/");
                Console.WriteLine(@" \_________________________________________________________________________|");
                Console.WriteLine();


                InitializeGame();

                StartGame();
            }
            catch (Exception e)
            {
                Console.WriteLine("A serious problem occured. The application cannot continue and will be closed.");
                telemetryClient.TrackException(e);
                Console.WriteLine("");
                Console.WriteLine("Error details:");
            }

        }

        public static void DrawCanon()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("                  __");
            Console.WriteLine(@"                 /  \");
            Console.WriteLine("           .-.  |    |");
            Console.WriteLine(@"   *    _.-'  \  \__/");
            Console.WriteLine(@"    \.-'       \");
            Console.WriteLine("   /          _/");
            Console.WriteLine(@"  |      _  /""");
            Console.WriteLine(@"  |     /_\'");
            Console.WriteLine(@"   \    \_/");
            Console.WriteLine(@"    """"""""");
            Console.ResetColor();
        }

        public static void DrawHit()
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"                \         .  ./");
            Console.WriteLine(@"              \      .:"";'.:..""   /");
            Console.WriteLine(@"                  (M^^.^~~:.'"").");
            Console.WriteLine(@"            -   (/  .    . . \ \)  -");
            Console.WriteLine(@"               ((| :. ~ ^  :. .|))");
            Console.WriteLine(@"            -   (\- |  \ /  |  /)  -");
            Console.WriteLine(@"                 -\  \     /  /-");
            Console.WriteLine(@"                   \  \   /  /");
            Console.ResetColor();
        }

        public static void DrawMiss()
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"                						");
            Console.WriteLine(@"              							");
            Console.WriteLine(@"                  						");
            Console.WriteLine(@"            							");
            Console.WriteLine(@"               							");
            Console.WriteLine(@"            	~~~~~~~~~~				");
            Console.WriteLine(@"             ~~~~~~~~~~~~~~~~~			");
            Console.WriteLine(@"           ~~~~~~~~~~~~~~~~~~~~~  		");
            Console.ResetColor();
        }

        private static void StartGame()
        {
            Console.Clear();
            DrawCanon();

            bool gameIsOver = false;

            do
            {
                PrintComputerGameBoard();
                PrintPlayerGameBoard();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Player, it's your turn");
                Console.WriteLine("Enter coordinates for your shot :");
                Console.ResetColor();
                var position = ParsePosition(Console.ReadLine());

                var hitSunk = GameController.CheckIsHit(enemyFleet, position);
                telemetryClient.TrackEvent("Player_ShootPosition", new Dictionary<string, string>() { { "Position", position.ToString() }, { "IsHit", hitSunk.Item1.ToString() } });
                if (hitSunk.Item1)
                {
                    DrawHit();
                }
                else
                {
                    myMisses.Add(position);
                    DrawMiss();
                }

                if(hitSunk.Item1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.WriteLine(hitSunk.Item1 ? "Yeah ! Nice hit !" : "Miss");
                Console.ResetColor();

                if (hitSunk.Item2 is not null) //if sunk a ship
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} was sunk!", hitSunk.Item2.Name);
                    Console.ResetColor();
                }

                //list sunk ships
                var sunkEnemyShips = GameController.ListSunkShips(enemyFleet);
                if (sunkEnemyShips.Count() > 0)
                {
                    Console.WriteLine("Sunk ships:");
                    foreach (var ship in sunkEnemyShips)
                    {
                        Console.WriteLine("\t{0}", ship.Name);
                    }
                }

                //check game end
                if (GameController.CheckAllSunk(enemyFleet))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("You are the winner!");
                    Console.ResetColor();
                    gameIsOver = true;
                    continue;
                }

                position = GetRandomPosition();
                hitSunk = GameController.CheckIsHit(myFleet, position);
                telemetryClient.TrackEvent("Computer_ShootPosition", new Dictionary<string, string>() { { "Position", position.ToString() }, { "IsHit", hitSunk.Item1.ToString() } });
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Computer shot in {0}{1} and {2}", position.Column, position.Row, hitSunk.Item1 ? "has hit your ship !" : "missed");
                Console.ResetColor();
                if (hitSunk.Item1)
                {
                    DrawHit();
                }
                else
                {
                    enemyMisses.Add(position);
                    DrawMiss();
                }

                if (hitSunk.Item2 is not null) //if sunk a ship
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} was sunk!", hitSunk.Item2.Name);
                    Console.ResetColor();
                }

                //check game end
                if (GameController.CheckAllSunk(myFleet))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("You lost!");
                    Console.ResetColor();
                    gameIsOver = true;
                    continue;
                }
            }
            while (!gameIsOver);
        }



        public static Position ParsePosition(string input)
        {
            var letter = (Letters)Enum.Parse(typeof(Letters), input.ToUpper().Substring(0, 1));
            var number = int.Parse(input.Substring(1, 1));
            return new Position(letter, number);
        }

        private static Position GetRandomPosition()
        {
            int rows = 8;
            int lines = 8;
            var random = new Random();
            var letter = (Letters)random.Next(lines);
            var number = random.Next(rows);
            var position = new Position(letter, number);
            return position;
        }

        private static void InitializeGame()
        {
            myMisses = new List<Position>();
            enemyMisses = new List<Position>();

            InitializeMyFleet();

            InitializeEnemyFleet();
        }

        private static void InitializeMyFleet()
        {
            myFleet = GameController.InitializeShips(babyFleet).ToList();

            PrintGameboardInstructions();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Please position your fleet (Game board size is from A to H and 1 to 8) :");

            foreach (var ship in myFleet)
            {
                PrintPlayerGameBoard();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Please enter the positions for the {0} (size: {1})", ship.Name, ship.Size);
                for (var i = 1; i <= ship.Size; i++)
                {

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Enter position {0} of {1} (i.e A3):", i, ship.Size);
                    Console.ResetColor();
                    var position = Console.ReadLine();
                    ship.AddPosition(position);
                    PrintPlayerGameBoard();
                    telemetryClient.TrackEvent("Player_PlaceShipPosition", new Dictionary<string, string>() { { "Position", position }, { "Ship", ship.Name }, { "PositionInShip", i.ToString() } });
                }
            }
            Console.ResetColor();
        }

        private static void InitializeEnemyFleet()
        {
            enemyFleet = GameController.InitializeShips(babyFleet).ToList();
            if (babyFleet)
            {
                
                enemyFleet[0].Positions.Add(new Position { Column = Letters.C, Row = 5 });
                enemyFleet[0].Positions.Add(new Position { Column = Letters.C, Row = 6 });
            }
            else
            {
                enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 4 });
                enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 5 });
                enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 6 });
                enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 7 });
                enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 8 });

                enemyFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 6 });
                enemyFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 7 });
                enemyFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 8 });
                enemyFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 9 });

                enemyFleet[2].Positions.Add(new Position { Column = Letters.A, Row = 3 });
                enemyFleet[2].Positions.Add(new Position { Column = Letters.B, Row = 3 });
                enemyFleet[2].Positions.Add(new Position { Column = Letters.C, Row = 3 });

                enemyFleet[3].Positions.Add(new Position { Column = Letters.F, Row = 8 });
                enemyFleet[3].Positions.Add(new Position { Column = Letters.G, Row = 8 });
                enemyFleet[3].Positions.Add(new Position { Column = Letters.H, Row = 8 });

                enemyFleet[4].Positions.Add(new Position { Column = Letters.C, Row = 5 });
                enemyFleet[4].Positions.Add(new Position { Column = Letters.C, Row = 6 });
            }
        }

        /// <summary>
        /// prints game board key so user knows what they are looking at
        /// </summary>
        private static void PrintGameboardInstructions()
        {
            Console.WriteLine("");
            Console.WriteLine("___________________________________________________________________________________");
            Console.WriteLine("The gameboard(s) below show the playing fields, hits, misses, and ships positions.");

            Console.Write("Untouched water will show as ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("~");
            Console.ResetColor();

            Console.Write("Hits will show as ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("X");
            Console.ResetColor();

            Console.Write("Misses will show as ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("X");
            Console.ResetColor();


            Console.Write("Ship positions will show as ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("V ");
            Console.ResetColor();
            Console.WriteLine("(You can only see your own ship positions.");
            Console.WriteLine("The enemy ship positions that have not been hit will show as untouched water)");
            Console.WriteLine("___________________________________________________________________________________");
            Console.WriteLine("");

        }

        /// <summary>
        /// prints enemy board
        /// </summary>
        private static void PrintComputerGameBoard()
        {
            Console.Write("***ENEMY GAMEBOARD***");

            List<Position> computerShipPos = new List<Position>();
            List<Position> ComputerHitPos = new List<Position>();

            foreach (Ship myShip in enemyFleet)
            {
                if (satellite)
                {
                    computerShipPos.AddRange(myShip.Positions);
                }
                ComputerHitPos.AddRange(myShip.Hits);
            }


            PrintGameBoard(ComputerHitPos, computerShipPos, myMisses);
        }

        /// <summary>
        /// prints player board
        /// </summary>
        private static void PrintPlayerGameBoard()
        {
            Console.Write("***YOUR GAMEBOARD***");

            List<Position> myShipPos = new List<Position>();
            List<Position> myHitPos = new List<Position>();

            foreach (Ship myShip in myFleet)
            {
                myShipPos.AddRange(myShip.Positions);
                myHitPos.AddRange(myShip.Hits);
            }

            PrintGameBoard(myHitPos, myShipPos, enemyMisses);
        }

        /// <summary>
        /// prints game board
        /// </summary>
        /// <param name="hitPositions">posistions that are hits and will show in red X's</param>
        /// <param name="boatPositions">posistions that are boats and will show in gray V's</param>
        /// <param name="missPositions">posistions that are misses and will show in blue X's</param>
        private static void PrintGameBoard(List<Position> hitPositions, List<Position> boatPositions, List<Position> missPositions)
        {
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("  A B C D E F G H");

            for (int i = 1; i <= 8; i++)

            {
                Console.Write(i.ToString() + " ");
                for (char c = 'A'; c <= 'H'; c++)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    string output = "~ ";
                    Position testPos = ParsePosition(c + i.ToString());

                    if (hitPositions.Contains(testPos))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        output = "X ";
                    }
                    else if (boatPositions.Contains(testPos))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        output = "V ";
                    }
                    else if (missPositions.Contains(testPos))
                    {
                        output = "X ";
                    }

                    Console.Write(output);
                    Console.ResetColor();
                }
                Console.WriteLine("");
                Console.ResetColor();
            }

            Console.ResetColor();
        }
    }
}
