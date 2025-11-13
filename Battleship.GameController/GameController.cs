using System.Linq;

namespace Battleship.GameController
{
    using System;
    using System.Collections.Generic;

    using Battleship.GameController.Contracts;

    /// <summary>
    ///     The game controller.
    /// </summary>
    public class GameController
    {

        /// <summary>
        /// Checks the is hit.
        /// </summary>
        /// <param name="ships">
        /// The ships.
        /// </param>
        /// <param name="shot">
        /// The shot.
        /// </param>
        /// <returns>
        /// True if hit, else false
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// ships
        ///     or
        ///     shot
        /// </exception>
        public static Tuple<bool, Ship?> CheckIsHit(IEnumerable<Ship> ships, Position shot)
        {
            if (ships == null)
            {
                throw new ArgumentNullException("ships");
            }

            if (shot == null)
            {
                throw new ArgumentNullException("shot");
            }

            foreach (var ship in ships)
            {
                if (ship.CheckHit(shot))
                {
                    return new Tuple<bool, Ship?>(true, ship);
                }     
            }

            return new Tuple<bool, Ship?>(false, null);
        }

        public static bool CheckAllSunk(IEnumerable<Ship> ships)
        {
            if (ships == null)
            {
                throw new ArgumentNullException("ships");
            }

            if (ships.Count() == 0) //if no ships, no ships are sunk
                return true;

            foreach (var ship in ships)
            {
                if (!ship.IsSunk())
                    return false;
            }

            return true;
        }

        public static IEnumerable<Ship> ListSunkShips(IEnumerable<Ship> ships)
        {
            if (ships == null)
            {
                throw new ArgumentNullException("ships");
            }

            return ships.Where(ship => ship.IsSunk());
        }

        /// <summary>
        ///     The initialize ships.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<Ship> InitializeShips()
        {
            return new List<Ship>()
                       {
                           new Ship() { Name = "Aircraft Carrier", Size = 5, Color = ConsoleColor.Blue }, 
                           new Ship() { Name = "Battleship", Size = 4, Color = ConsoleColor.Red }, 
                           new Ship() { Name = "Submarine", Size = 3, Color = ConsoleColor.Gray }, 
                           new Ship() { Name = "Destroyer", Size = 3, Color = ConsoleColor.Yellow }, 
                           new Ship() { Name = "Patrol Boat", Size = 2, Color = ConsoleColor.Green }
                       };
        }

        /// <summary>
        /// The is ships valid.
        /// </summary>
        /// <param name="ship">
        /// The ship.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsShipValid(Ship ship)
        {
            return ship.Positions.Count == ship.Size;
        }

        public static Position GetRandomPosition(int size)
        {
            var random = new Random();
            var letter = (Letters)random.Next(size);
            var number = random.Next(size);
            var position = new Position(letter, number);
            return position;
        }
     }
}