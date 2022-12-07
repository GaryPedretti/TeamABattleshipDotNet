
namespace Battleship.Ascii
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Battleship.Ascii.TelemetryClient;
    using Battleship.GameController;
    using Battleship.GameController.Contracts;

    public class Program
    {
        private static List<Ship> myFleet;

        private static List<Ship> enemyFleet;

        private static ITelemetryClient telemetryClient;

        static void Main()
        {
            telemetryClient = new ApplicationInsightsTelemetryClient();
            telemetryClient.TrackEvent("ApplicationStarted", new Dictionary<string, string> { { "Technology", ".NET"} });

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
                throw new Exception("Fatal error", e);
            }

        }

        private static void StartGame()
        {
            Console.Clear();
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

            bool done = false;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Player, it's your turn");
                Console.WriteLine("Enter coordinates for your shot :");
                var position = ParsePosition(Console.ReadLine());                
                var isHit = GameController.CheckIsHit(enemyFleet, position);
                telemetryClient.TrackEvent("Player_ShootPosition", new Dictionary<string, string>() { { "Position", position.ToString() }, { "IsHit", isHit.ToString() } });
                if (isHit)
                {
                    Console.Beep();

                    Console.WriteLine(@"                \         .  ./");
                    Console.WriteLine(@"              \      .:"";'.:..""   /");
                    Console.WriteLine(@"                  (M^^.^~~:.'"").");
                    Console.WriteLine(@"            -   (/  .    . . \ \)  -");
                    Console.WriteLine(@"               ((| :. ~ ^  :. .|))");
                    Console.WriteLine(@"            -   (\- |  \ /  |  /)  -");
                    Console.WriteLine(@"                 -\  \     /  /-");
                    Console.WriteLine(@"                   \  \   /  /");
                }

                
                if(isHit)
                {
                    
                    WriteLine( ConsoleColor.Red,"Yeah ! Nice hit !");
                }
                else
                {
                    
                    WriteLine(ConsoleColor.Blue,"Miss"); 
                }
                Console.ForegroundColor=ConsoleColor.White;
                done = PrintShipsSunk(enemyFleet, "Enemy Ships Sunk");
                if(done)
                    break;
                PrintShipsRemaining(enemyFleet, "Enemy Ships Remaining");

                position = GetRandomPosition();
                isHit = GameController.CheckIsHit(myFleet, position);
                telemetryClient.TrackEvent("Computer_ShootPosition", new Dictionary<string, string>() { { "Position", position.ToString() }, { "IsHit", isHit.ToString() } });
                if(isHit)
                {
                    
                    WriteLine(ConsoleColor.Red,"Computer shot in {0}{1} and {2}", position.Column, position.Row, "has hit your ship !");
                }
                else
                {
                    
                    WriteLine(ConsoleColor.Blue,"Computer shot in {0}{1} and {2}", position.Column, position.Row,  "missed");
                }
               
                if (isHit)
                {
                    Console.Beep();

                    Console.WriteLine(@"                \         .  ./");
                    Console.WriteLine(@"              \      .:"";'.:..""   /");
                    Console.WriteLine(@"                  (M^^.^~~:.'"").");
                    Console.WriteLine(@"            -   (/  .    . . \ \)  -");
                    Console.WriteLine(@"               ((| :. ~ ^  :. .|))");
                    Console.WriteLine(@"            -   (\- |  \ /  |  /)  -");
                    Console.WriteLine(@"                 -\  \     /  /-");
                    Console.WriteLine(@"                   \  \   /  /");

                }
                Console.ForegroundColor=ConsoleColor.White;
                done = PrintShipsSunk(myFleet, "My Ships Sunk");
                if(done)
                    break;
                PrintShipsRemaining(myFleet, "My Ships Remaining");
            }
            while (!done);
        }
        public static void WriteLine(System.ConsoleColor color, string format, params object[] args)
        {
            var currentColor=Console.ForegroundColor;
            Console.ForegroundColor=color;

            Console.WriteLine(format,args);
            Console.ForegroundColor=currentColor;


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
            myFleet = InitializeEnemyFleet();

            enemyFleet = InitializeEnemyFleet();
        }

        private static void InitializeMyFleet()
        {
            myFleet = GameController.InitializeShips().ToList();

            Console.WriteLine("Please position your fleet (Game board size is from A to H and 1 to 8) :");

            foreach (var ship in myFleet)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the positions for the {0} (size: {1})", ship.Name, ship.Size);
                for (var i = 1; i <= ship.Size; i++)
                {
                    Console.WriteLine("Enter position {0} of {1} (i.e A3):", i, ship.Size);
                    var position = Console.ReadLine();
                    ship.AddPosition(position);
                    telemetryClient.TrackEvent("Player_PlaceShipPosition", new Dictionary<string, string>() { { "Position", position }, { "Ship", ship.Name }, { "PositionInShip", i.ToString() } });
                }
            }
        }

        private static List<Ship> InitializeEnemyFleet()
        {
            var fleet = GameController.InitializeShips().ToList();

            fleet[0].Positions.Add(new Position { Column = Letters.B, Row = 4 });
            fleet[0].Positions.Add(new Position { Column = Letters.B, Row = 5 });
            fleet[0].Positions.Add(new Position { Column = Letters.B, Row = 6 });
            fleet[0].Positions.Add(new Position { Column = Letters.B, Row = 7 });
            fleet[0].Positions.Add(new Position { Column = Letters.B, Row = 8 });

            fleet[1].Positions.Add(new Position { Column = Letters.E, Row = 6 });
            fleet[1].Positions.Add(new Position { Column = Letters.E, Row = 7 });
            fleet[1].Positions.Add(new Position { Column = Letters.E, Row = 8 });
            fleet[1].Positions.Add(new Position { Column = Letters.E, Row = 9 });

            fleet[2].Positions.Add(new Position { Column = Letters.A, Row = 3 });
            fleet[2].Positions.Add(new Position { Column = Letters.B, Row = 3 });
            fleet[2].Positions.Add(new Position { Column = Letters.C, Row = 3 });

            fleet[3].Positions.Add(new Position { Column = Letters.F, Row = 8 });
            fleet[3].Positions.Add(new Position { Column = Letters.G, Row = 8 });
            fleet[3].Positions.Add(new Position { Column = Letters.H, Row = 8 });

            fleet[4].Positions.Add(new Position { Column = Letters.C, Row = 5 });
            fleet[4].Positions.Add(new Position { Column = Letters.C, Row = 6 });
            

            return fleet;
        }

        public static bool  PrintShipsSunk(List<Ship> fleet, string label = "") {
            Console.WriteLine("");
            Console.WriteLine(label);
            var sunk = fleet.Where(x => x.IsSunk()).ToList();
            if(sunk.Count == 0) {
                Console.WriteLine("None");
            }
            if(sunk.Count==fleet.Count) {
                PrintFinalMessage(fleet);
                return true;
            }
            foreach(var ship in sunk) {
                WriteLine(ConsoleColor.Red, ship.Name);
            }
            Console.WriteLine("");
            return false;
        }

        public static void PrintShipsRemaining(List<Ship> fleet, string label = "") {
            Console.WriteLine("");
            Console.WriteLine(label);
            var sunk = fleet.Where(x => !x.IsSunk()).ToList();
            if(sunk.Count == 0) {
                Console.WriteLine("None");
            }
            foreach(var ship in sunk) {
                 WriteLine(ConsoleColor.Blue, ship.Name);
            }
            Console.WriteLine("");
        }
        

        public static void PrintFinalMessage(List<Ship> fleet) {
            Console.WriteLine("");
            Console.WriteLine("");
            if(fleet == enemyFleet) {
                Console.WriteLine("Congratulation! You have wone the game!");
            }
            else {
                Console.WriteLine("You have been sent to Davey Jones's locker!");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }

    }

}
