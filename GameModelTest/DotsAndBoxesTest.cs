using DotsAndBoxes.Model;
using DotsAndBoxes.Persistence;
using Moq;
using System.Text;

namespace Boxes.Test
{
    [TestClass]
    public sealed class BoxesGameModelTest
    {
        private GameModel _model = null!; // a tesztelendő modell
        private Mock<IBoxesDataAccess> _mock = null!; // az adatelérés mock-ja
        private GameModel _mockedModel = null!;

        [TestInitialize]
        public void TestInit()
        {
            _mock = new Mock<IBoxesDataAccess>();
            _model = new GameModel(_mock.Object);
            _mockedModel = new GameModel(_mock.Object);

            _mockedModel.AddHorizontalLine(1, 1);
            _mockedModel.AddHorizontalLine(2, 1);
            _mockedModel.AddVerticalLine(1, 1);
            _mockedModel.AddVerticalLine(0, 1);
            _mockedModel.AddVerticalLine(2, 2);


            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult((_mockedModel.Board, _mockedModel.Player1.Score, _mockedModel.Player2.Score, _mockedModel.CurrentPlayer.PlayerId)));
        }

        [TestMethod]
        public void GameModelNewGameSmallTest()
        {
            _model.NewGame(BoardSize.Small);

            Assert.AreEqual(3, _model.Board.GridSize);
            Assert.IsFalse(_model.Board.GetHLines().Cast<bool>().Any(e => e));
            Assert.IsFalse(_model.Board.GetVLines().Cast<bool>().Any(e => e));
            Assert.AreEqual(0, _model.Player1.Score);
            Assert.AreEqual(0, _model.Player2.Score);
        }

        [TestMethod]
        public void GameModelNewGameMediumTest()
        {
            _model.NewGame(BoardSize.Medium);

            Assert.AreEqual(5, _model.Board.GridSize);
            Assert.IsFalse(_model.Board.GetHLines().Cast<bool>().Any(e => e));
            Assert.IsFalse(_model.Board.GetVLines().Cast<bool>().Any(e => e));
            Assert.AreEqual(0, _model.Player1.Score);
            Assert.AreEqual(0, _model.Player2.Score);
        }

        [TestMethod]
        public void GameModelNewGameLargeTest()
        {
            _model.NewGame(BoardSize.Large);

            Assert.AreEqual(9, _model.Board.GridSize);
            Assert.IsFalse(_model.Board.GetHLines().Cast<bool>().Any(e => e));
            Assert.IsFalse(_model.Board.GetVLines().Cast<bool>().Any(e => e));
            Assert.AreEqual(0, _model.Player1.Score);
            Assert.AreEqual(0, _model.Player2.Score);
        }

        [TestMethod]
        public async Task ModelLoadTest()
        {
            _model.NewGame(BoardSize.Medium);
            Assert.AreEqual(_model.Player1, _model.CurrentPlayer);

            await _model.LoadGameAsync(String.Empty);
            Assert.AreEqual(3, _model.Board.GridSize);
            Assert.AreEqual(_model.Player2.PlayerId, _model.CurrentPlayer.PlayerId);
        }

        [TestMethod]
        public async Task ModelScoreTest()
        {
            await _model.LoadGameAsync(String.Empty);

            _model.AddVerticalLine(1, 2);
            Board board = _model.Board;

            Assert.AreEqual(_model.Player2.PlayerId, board.GetBoxes()[1, 1].OwnerId);
            Assert.AreEqual(1, _model.Player2.Score);
            Assert.AreEqual(_model.Player2.PlayerId, _model.CurrentPlayer.PlayerId);
        }
    }
}
