using Battleship.GameController.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Battleship.GameController.ATDD
{
    [Binding]
    public class IsShipValidSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public IsShipValidSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        private bool result { get; set; }
        private Ship ship; 

        [Given(@"I have a (.*) ship with (.*) positions")]
        public void GivenIHaveA_P0_ShipWith_P1_Positions(int size, int positions)
        {
            ship = new Ship();
            ship.Size = size;
            for (int i = 1; i <= positions; i++)
            {
                ship.Positions.Add(new Position(Letters.A, i));
            }
        }

        [When(@"I check if the ship is valid")]
        public void WhenICheckIfTheShipIsValid()
        {
            result = GameController.IsShipValid(ship);
        }

        [Then(@"the result should be (.*)")]
        public void ThenTheResultShouldBe_P0(bool expected)
        {
            Assert.AreEqual(expected, result);
        }
    }
}