namespace Battleship.GameController.Tests.GameControllerTests
{
    using System.Collections.Generic;

    using Battleship.GameController.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;

    /// <summary>
    /// The is ship valid tests.
    /// </summary>
    [TestClass]
    public class ShipIsSunkTests
    {
        /// <summary>
        /// The ship is not sunk.
        /// </summary>
        [TestMethod]
        public void ShipIsSunk()
        {
            var positions = new List<Position> { new Position(Letters.A, 1), new Position(Letters.A, 2), new Position(Letters.A, 3) };

            var ship = new Ship { Name = "TestShip", Size = 3, Positions = positions };

            var shot1 = new Position(Letters.A, 1);
            var shot2 = new Position(Letters.A, 2);
            var shot3 = new Position(Letters.A, 3);

            var isHit1 = ship.CheckHit(shot1);
            var isHit2 = ship.CheckHit(shot2);
            var isHit3 = ship.CheckHit(shot3);

            Console.WriteLine(isHit1);
            Console.WriteLine(isHit2);
            Console.WriteLine(isHit3);

            Assert.IsTrue(ship.IsSunk());


        }

        /// <summary>
        /// The ship is sunk.
        /// </summary>
        [TestMethod]
        public void ShipIsNotSunk()
        {
            var positions = new List<Position> { new Position(Letters.A, 1), new Position(Letters.A, 2), new Position(Letters.A, 3) };

            var ship = new Ship { Name = "TestShip", Size = 3, Positions = positions };
             var shot1 = new Position(Letters.A, 1);
            var shot2 = new Position(Letters.A, 2);
            var shot3 = new Position(Letters.A, 3);

            var isHit1 = ship.CheckHit(shot1);
            var isHit2 = ship.CheckHit(shot2);

            Assert.IsFalse(ship.IsSunk());
        }
    }
}