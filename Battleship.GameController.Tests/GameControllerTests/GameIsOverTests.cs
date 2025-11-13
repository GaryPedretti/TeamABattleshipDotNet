namespace Battleship.GameController.Tests.GameControllerTests
{
    using Battleship.GameController.Contracts;
    using Microsoft.VisualStudio.CodeCoverage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Metrics;
    using System.Linq;

    /// <summary>
    /// The is ship valid tests.
    /// </summary>
    [TestClass]
    public class GameIsOverTests
    {
        /// <summary>
        /// The ship is not sunk.
        /// </summary>
        [TestMethod]
        public void GameIsOver()
        {
            List<List<Position>> shipPositions = new List<List<Position>>
            {
                { new List<Position> { new Position(Letters.A, 1), new Position(Letters.A, 2), new Position(Letters.A, 3), new Position(Letters.A, 4), new Position(Letters.A, 5) } },
                { new List<Position> {new Position(Letters.B, 1), new Position(Letters.B, 2), new Position(Letters.B, 3), new Position(Letters.B, 4) } },
                { new List<Position> {new Position(Letters.C, 1), new Position(Letters.C, 2), new Position(Letters.C, 3)} },
                { new List<Position> {new Position(Letters.D, 1), new Position(Letters.D, 2), new Position(Letters.D, 3)} },
                { new List<Position> {new Position(Letters.E, 1), new Position(Letters.E, 2)}},
            };

            var ships = GameController.InitializeShips();

            for (int i = 0; i < 5; i ++)
            {
                foreach (var pos in shipPositions[i])
                {
                    ships.ElementAt(i).Positions.Add(pos);
                }
            }

            foreach (var ship in shipPositions)
            {
                foreach (var pos in ship)
                {
                    var isHit = GameController.CheckIsHit(ships, pos);
                }
            }

            var result = GameController.CheckAllSunk(ships);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// The ship is sunk.
        /// </summary>
        [TestMethod]
        public void GameIsNotOver()
        {
            List<List<Position>> shipPositions = new List<List<Position>>
            {
                { new List<Position> { new Position(Letters.A, 1), new Position(Letters.A, 2), new Position(Letters.A, 3), new Position(Letters.A, 4), new Position(Letters.A, 5) } },
                { new List<Position> {new Position(Letters.B, 1), new Position(Letters.B, 2), new Position(Letters.B, 3), new Position(Letters.B, 4) } },
                { new List<Position> {new Position(Letters.C, 1), new Position(Letters.C, 2), new Position(Letters.C, 3)} },
                { new List<Position> {new Position(Letters.D, 1), new Position(Letters.D, 2), new Position(Letters.D, 3)} },
                { new List<Position> {new Position(Letters.E, 1), new Position(Letters.E, 2)}},
            };

            var ships = GameController.InitializeShips();

            for (int i = 0; i < 5; i++)
            {
                foreach (var pos in shipPositions[i])
                {
                    ships.ElementAt(i).Positions.Add(pos);
                }
            }

            //foreach (var ship in shipPositions)
            //{
            //    foreach (var pos in ship)
            //    {
            //        var isHit = GameController.CheckIsHit(ships, pos);
            //    }
            //}

            var result = GameController.CheckAllSunk(ships);

            Assert.IsFalse(result);
        }
    }
}