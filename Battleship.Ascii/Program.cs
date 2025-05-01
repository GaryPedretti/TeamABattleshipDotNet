
namespace Battleship.Ascii
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Transactions;
    using System.Xml.Schema;
    using Battleship.Ascii.TelemetryClient;
    using Battleship.GameController;
    using Battleship.GameController.Contracts;

    public class Program
    {
        private static String[] shipNames = {"1", "2", "3", "4", "5"};
        private static int[] shipSizes = {5, 4, 3, 3, 2};

        private static List<Ship> myFleet;

        private static List<Ship> enemyFleet;

        
        private static String[,] myGrid =  {
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "}
            };
        private static String[,] enemyGrid =  {
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "},
                                {"W ", "W ", "W ", "W ", "W ", "W ", "W ", "W "}
            };

        private static ITelemetryClient telemetryClient;

        private static int totalHitCountPlayer;

        private static int totalHitCountComputer;

        private static int numPositions = 17;

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
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
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

            do
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("Player, it's your turn");
                Console.ForegroundColor = ConsoleColor.White;
                Position position = GetShotCoordinates();              
                var isHit = GameController.CheckIsHit(enemyFleet, position);
                telemetryClient.TrackEvent("Player_ShootPosition", new Dictionary<string, string>() { { "Position", position.ToString() }, { "IsHit", isHit.ToString() } });
                if (isHit)
                {
                    totalHitCountPlayer++;
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
                    if (totalHitCountPlayer == numPositions)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("You are a Winner!");
                        while(true);
                    }
                }
                if(isHit){
                    Console.ForegroundColor = ConsoleColor.Red;
                }else{
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.WriteLine(isHit ? "Yeah ! Nice hit !" : "Miss");
                resetConsoleColor();
                position = GetRandomPosition();
                isHit = GameController.CheckIsHit(myFleet, position);
                telemetryClient.TrackEvent("Computer_ShootPosition", new Dictionary<string, string>() { { "Position", position.ToString() }, { "IsHit", isHit.ToString() } });
                Console.WriteLine();
                if(isHit){
                    Console.ForegroundColor = ConsoleColor.Red;
                }else{
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.WriteLine("Computer shot in {0}{1} and {2}", position.Column, position.Row, isHit ? "has hit your ship !" : "missed");
                if (isHit)
                {
                    totalHitCountComputer++;
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
                    if (totalHitCountComputer == numPositions)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("You Lose!");
                        while(true);
                    }

                }
                resetConsoleColor();
            }
            while (true);


        }

        private static Position GetShotCoordinates()
        {
            Position position;
            while (true)
            {
                Console.WriteLine("Enter coordinates for your shot :");
                try 
                {
                    position = ParsePosition(Console.ReadLine());
                    if ((int)position.Column > 8)
                    {
                        Console.WriteLine("Column is invalid!");
                        continue;
                    }
                    else if (position.Row > 8)
                    {
                        Console.WriteLine("Row is invalid!");
                        continue;
                    }
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Coordinates are invalid!");
                    continue;
                }
            }
            return position;
        }
        
        public static Position ParsePosition(string input)
        {
            if (input[0] < 'A' || input[0] > 'H')
            {
                throw new InvalidDataException();
            }
            var letter = (Letters)Enum.Parse(typeof(Letters), input.ToUpper().Substring(0, 1));
            var number = int.Parse(input.Substring(1));
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

        private static void AddEnemyShipPosition(Position pos)
        {
            int row = pos.Row;
            int col = (int)pos.Column;

            enemyGrid[row, col] = "S ";
        }

        private static void AddMyShipPosition(Position pos)
        {
            int row = pos.Row;
            int col = (int)pos.Column;

            myGrid[row-1, col] = "S ";
        }

        private static void PrintMyFleetPositions()
        {
            Console.WriteLine();
            Console.WriteLine("MY SHIPS:");
            Console.WriteLine();

            // TODO: not 8
            for(int i = 0; i < 8; i++)
            {
                // TODO: not 8
                for(int j = 0; j < 8; j++)
                {
                    Console.Write(myGrid[i,j]);
                }

                Console.WriteLine();
            }
            
            Console.WriteLine();
        }

        private static void PrintEnemyFleetPositions()
        {
            Console.WriteLine();
            Console.WriteLine("ENEMY SHIPS:");
            Console.WriteLine();

            // TODO: not 8
            for(int i = 0; i < 8; i++)
            {
                // TODO: not 8
                for(int j = 0; j < 8; j++)
                {
                    Console.Write(enemyGrid[i,j]);
                }
                
                Console.WriteLine();
            }
            
            Console.WriteLine();
        }

        private static Position ValidatePositionStr(string positionStr)
        {
            var pos = new Position();

            String[] letters = {"A", "B", "C", "D", "E", "F", "G", "H"};
            String[] numbers = {"1", "2", "3", "4", "5", "6", "7", "8"};

            /*
            if(positionStr.Equals("SKIP ALL") || positionStr.Equals("RDM"))
            {
                return GetRandomPosition();
            }
            */
            /*else*/ if(positionStr.Length != 2)
            {
                return null;
            }

            var success = false;
            for(int i = 0; i < letters.Length; i++)
            {
                if (positionStr.Substring(0,1) == letters[i])
                {
                    success = true;
                    pos.Column = (Letters)(i);
                    break;
                }
            }

            if(success)
            {
                success = false;

                for(int i = 0; i < numbers.Length; i++)
                {
                    if (positionStr.Substring(1,1) == numbers[i])
                    {
                        success = true;
                        pos.Row = i+1;
                        break;
                    }
                }
            }

            if(!success)
            {
                return null;
            }

            return pos;
        }

        private static void InitializeGame()
        {
            InitializeEnemyFleet();
            InitializeMyFleet();
        }

        private static void InitializeMyFleet()
        {
            myFleet = GameController.InitializeShips().ToList();
            List<String> usedFleetPositions = new List<String>();
            resetConsoleColor();
            Console.WriteLine("Please position your fleet (Game board size is from A to H and 1 to 8) : // type TEST to enter test mode");
            if(Console.ReadLine().Equals("TEST")){
                InitializeMyFleetForTest();
                Console.WriteLine("TEST MODE ENTERED");
            }else{

                foreach (var ship in myFleet)
                {
                    bool addOK = false;
                    var inputStr = "RDM";
                    Console.WriteLine();
                    Console.WriteLine("Please enter the positions for the {0} (size: {1})", ship.Name, ship.Size);
                    for (var i = 1; i <= ship.Size; i++)
                    {
                        do {

                        Position pos = null;
                        
                        while(pos == null)
                        {
                            Console.WriteLine("Enter position {0} of {1} (i.e A3):", i, ship.Size);
                            inputStr = Console.ReadLine();

                            pos = ValidatePositionStr(inputStr);

                            if(pos == null)
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("Invalid position, please try again.");
                                resetConsoleColor();
                                Console.WriteLine();
                            }
                            else
                            { 
                                addOK = ship.AddPosition(pos);
                                AddMyShipPosition(pos);
                            }
                        }

                        if(usedFleetPositions.Contains(inputStr)){
                            addOK = false;
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Invalid position, please try again.");
                            resetConsoleColor();
                            Console.WriteLine();
                        }else{                     
                            usedFleetPositions.Add(ship.AddPosition(inputStr));
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Position " + inputStr + " added successfully.");
                            resetConsoleColor();
                            Console.WriteLine();
                            addOK = true;
                        }
                        PrintMyFleetPositions();
                        telemetryClient.TrackEvent("Player_PlaceShipPosition", new Dictionary<string, string>() { { "Position", inputStr }, { "Ship", ship.Name }, { "PositionInShip", i.ToString() } });
                        } while (addOK == false);
                    }
                    numPositions++;
                }
            }
        }

        private static void InitializeMyFleetForTest()
        {
            myFleet = GameController.InitializeShips().ToList();

            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 4 });
            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 5 });
            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 6 });
            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 7 });
            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 8 });

            myFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 6 });
            myFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 7 });
            myFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 8 });
            myFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 9 });

            myFleet[2].Positions.Add(new Position { Column = Letters.A, Row = 3 });
            myFleet[2].Positions.Add(new Position { Column = Letters.B, Row = 3 });
            myFleet[2].Positions.Add(new Position { Column = Letters.C, Row = 3 });

            myFleet[3].Positions.Add(new Position { Column = Letters.F, Row = 8 });
            myFleet[3].Positions.Add(new Position { Column = Letters.G, Row = 8 });
            myFleet[3].Positions.Add(new Position { Column = Letters.H, Row = 8 });

            myFleet[4].Positions.Add(new Position { Column = Letters.C, Row = 5 });
            myFleet[4].Positions.Add(new Position { Column = Letters.C, Row = 6 });
        }

        private static void InitializeEnemyFleet()
        {
            enemyFleet = GameController.InitializeShips().ToList();

            int shipCount = 0;
            foreach(var ship in enemyFleet)
            {
                int shipSize = shipSizes[shipCount];
                for(int i = 0; i < ship.Size; i++)
                {
                    Position pos = GetRandomPosition();

                    enemyFleet[shipCount].Positions.Add(pos);
                    AddEnemyShipPosition(pos);
                }

                shipCount++;
            }

            PrintEnemyFleetPositions();

            /*
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
            */
        }
    

    private static void resetConsoleColor(){
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
    }
}
}
