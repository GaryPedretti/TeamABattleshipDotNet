namespace Battleship.GameController.Tests.GameControllerTests
{
    using System.Collections.Generic;

    using Battleship.GameController.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The is ship valid tests.
    /// </summary>
    [TestClass]
    public class IsShipValidTests
    {
        /// <summary>
        /// The ship is not valid.
        /// </summary>
        [TestMethod]
        public void ShipIsNotValid()
        {
            var ship = new Ship { Name = "TestShip", Size = 3 };
            var result = GameController.IsShipValid(ship);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// The ship is valid.
        /// </summary>
        [TestMethod]
        public void ShipIsValid()
        {
            var positions = new List<Position> { new Position(Letters.A, 1), new Position(Letters.A, 1), new Position(Letters.A, 1) };

            var ship = new Ship { Name = "TestShip", Size = 3, Positions = positions };
            var result = GameController.IsShipValid(ship);

            Assert.IsTrue(result);
        }
    }
}