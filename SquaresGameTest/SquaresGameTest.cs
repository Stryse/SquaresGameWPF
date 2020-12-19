using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaresGame.Model;
using SquaresGame.Persistence;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SquaresGame.Model;
using System.Windows.Media;

namespace SquaresGameTest
{
    [TestClass]
    public class SquaresGameTest
    {
        //======== Fields ========//
        SquaresGameModel model;
        Mock<ISquaresGameDataAccess> mockDAccess;
        GameStateWrapper state;
        int endGameRaised = 0;

        [TestInitialize]
        public void Initialize()
        {
            Player PlayerOne = new Player("TestPlayer1", Colors.Red);
            Player PlayerTwo = new Player("TestPlayer2", Colors.Blue);

            state = new GameStateWrapper
            {
                PlayerOne = PlayerOne,
                PlayerTwo = PlayerTwo,
                ActivePlayer = PlayerOne,
                FieldSize = 3,
                Lines = new List<System.Tuple<Point, Point, Player>>(),
                Rectangles = new List<System.Tuple<Point, Point, Player>>(),
                RegisteredRectCount = 0
            };

            mockDAccess = new Mock<ISquaresGameDataAccess>();
            mockDAccess.Setup(mock => mock.LoadGameAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(state));

            model = SquaresGameModel.FromSave(state, mockDAccess.Object);
            model.EndGame += EndGameAssert;
            model.EndGame += (sender, e) => ++endGameRaised;

        }

        [TestMethod]
        public void TestAddLineValid()
        {
            model.Restart();

            //Add Valid Lines
            model.AddNewLine(new Point(0, 0), new Point(0, 1)); // Horizontal | Left  -> Right
            model.AddNewLine(new Point(1, 1), new Point(1, 0)); // Horizontal | Right -> Left
            model.AddNewLine(new Point(1, 0), new Point(2, 0)); // Vertical   | Top   -> Bottom
            model.AddNewLine(new Point(2, 1), new Point(1, 1)); // Vertical   | Bottom -> Top

            //There must be 4 valid lines
            Assert.AreEqual(4, model.Lines.Count);
            Assert.AreEqual(0, model.Rectangles.Count);
            Assert.AreEqual(false, model.GameEnded);

            //Add same lines
            model.AddNewLine(new Point(0, 1), new Point(0, 0)); // line0 flipped - same - no add
            model.AddNewLine(new Point(1, 0), new Point(1, 1)); // line1 flipped - same - no add
            model.AddNewLine(new Point(2, 0), new Point(1, 0)); // line2 flipped - same - no add
            model.AddNewLine(new Point(1, 1), new Point(2, 1)); // line3 flipped - same - no add

            //There must be 4 valid lines
            Assert.AreEqual(4, model.Lines.Count);
            Assert.AreEqual(0, model.Rectangles.Count);
            Assert.AreEqual(false, model.GameEnded);
        }

        [TestMethod]
        public void TestAddLineInvalid()
        {
            model.Restart();

            //Add Invalid Lines
            model.AddNewLine(new Point(0, 0), new Point(1, 1)); // Diagonal invalid
            model.AddNewLine(new Point(0, 0), new Point(2, 0)); // Horizontal - Longer than one unit
            model.AddNewLine(new Point(0, 0), new Point(2, 2)); // Diagonal   - Longer than one unit
            model.AddNewLine(new Point(1, 1), new Point(1, 1)); // Not a line - same dot
            model.AddNewLine(new Point(10000, 10000), new Point(10000, 9999)); // out of bounds

            //There must be 0 valid lines
            Assert.AreEqual(0, model.Lines.Count);
            Assert.AreEqual(0, model.Rectangles.Count);
            Assert.AreEqual(false, model.GameEnded);

        }

        [TestMethod]
        public void TestSingleRectangle()
        {
            model.Restart();

            //Add lines
            model.AddNewLine(new Point(0, 0), new Point(0, 1));
            model.AddNewLine(new Point(1, 0), new Point(1, 1));
            model.AddNewLine(new Point(0, 1), new Point(1, 1));

            //There must be 3 lines - 0 rectangle
            Assert.AreEqual(3, model.Lines.Count);
            Assert.AreEqual(0, model.Rectangles.Count);
            Assert.AreEqual(false, model.GameEnded);

            model.AddNewLine(new Point(1, 0), new Point(0, 0)); //Adding final line

            //There must be 4 lines - 1 rectangle
            Assert.AreEqual(4, model.Lines.Count);
            Assert.AreEqual(1, model.Rectangles.Count);
            Assert.AreEqual(false, model.GameEnded);
        }

        [TestMethod]
        public void TestDualRectangle()
        {
            model.Restart();

            //Add Lines
            model.AddNewLine(new Point(0, 0), new Point(0, 1));
            model.AddNewLine(new Point(0, 1), new Point(1, 1));
            model.AddNewLine(new Point(1, 0), new Point(0, 0));

            model.AddNewLine(new Point(1, 0), new Point(2, 0));
            model.AddNewLine(new Point(2, 0), new Point(2, 1));
            model.AddNewLine(new Point(2, 1), new Point(1, 1));

            //There must be 3 lines - 0 rectangle
            Assert.AreEqual(6, model.Lines.Count);
            Assert.AreEqual(0, model.Rectangles.Count);
            Assert.AreEqual(false, model.GameEnded);

            model.AddNewLine(new Point(1, 0), new Point(1, 1)); //Adding final line

            //There must be 7 lines - 2 rectangle
            Assert.AreEqual(7, model.Lines.Count);
            Assert.AreEqual(2, model.Rectangles.Count);
            Assert.AreEqual(false, model.GameEnded);
        }

        [TestMethod]
        public void TestGame()
        {
            model.Restart();

            //Horizontals
            for (int i = 0; i < model.FieldSize; ++i)
                for (int j = 0; j < model.FieldSize-1; ++j)
                    model.AddNewLine(new Point(i, j), new Point(i, j+1));

            //Verticals
            for (int j = 0; j < model.FieldSize; ++j)
                for (int i = 0; i < model.FieldSize-1; ++i)
                {
                    if (i != model.FieldSize - 2 && j != model.FieldSize - 1)
                        Assert.IsFalse(model.GameEnded);

                    model.AddNewLine(new Point(i,j), new Point(i+1,j));
                }

            Assert.IsTrue(model.GameEnded);
            Assert.AreEqual(1, endGameRaised);
        }

        public void EndGameAssert(object sender, Player p)
        {
            Assert.IsTrue(model.GameEnded);
            Assert.IsTrue(model.Rectangles.Count == (model.FieldSize - 1) * (model.FieldSize - 1));
        }
    }
}
