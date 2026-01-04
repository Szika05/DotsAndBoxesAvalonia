using Game.Persistence;

namespace Game.Model
{
    public class GameModel
    {
        public event EventHandler<EventArgs>? GameUpdated;
        public event EventHandler<EventArgs>? GameEnded;
        public event EventHandler<EventArgs>? TurnChanged;

        private IBoxesDataAccess _dataAccess;

        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public Player CurrentPlayer { get; private set; }

        public Board Board { get; private set; }

        public GameModel(IBoxesDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            Player1 = new Player(1);
            Player2 = new Player(2);
            CurrentPlayer = Player1;
            Board = new Board(BoardSize.Small);
        }

        public void AddHorizontalLine(int row, int col)
        {
            if (!Board.AddHorizontalLine(row, col)) return;

            int completed = Board.CheckHorizontalCompletion(row, col, CurrentPlayer.PlayerId);
            CurrentPlayer.AddPoint(completed);

            GameUpdated?.Invoke(this, EventArgs.Empty);

            if (completed == 0)
                SwitchTurn();
            else if (Board.IsFull())
                GameEnded?.Invoke(this, EventArgs.Empty);
        }

        public void AddVerticalLine(int row, int col)
        {
            if (!Board.AddVerticalLine(row, col)) return;

            int completed = Board.CheckVerticalCompletion(row, col, CurrentPlayer.PlayerId);
            CurrentPlayer.AddPoint(completed);

            GameUpdated?.Invoke(this, EventArgs.Empty);

            if (completed == 0)
                SwitchTurn();
            else if (Board.IsFull())
                GameEnded?.Invoke(this, EventArgs.Empty);
        }

        private void SwitchTurn()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            TurnChanged?.Invoke(this, EventArgs.Empty);
        }

        public void NewGame(BoardSize boardSize)
        {
            Board = new Board(boardSize);
            Player1.ResetScore();
            Player2.ResetScore();
            CurrentPlayer = Player1;
        }

        public async Task LoadGameAsync(string path)
        {
            var (board, p1Score, p2Score, currentPlayer) = await _dataAccess.LoadAsync(path);

            Board = board;
            Player1.ResetScore();
            Player1.AddPoint(p1Score);
            Player2.ResetScore();
            Player2.AddPoint(p2Score);
            CurrentPlayer = currentPlayer == 1 ? Player1 : Player2;
        }

        public async Task SaveGameAsync(string path)
        {
            await _dataAccess.SaveAsync(path, Board, Player1, Player2, CurrentPlayer);
        }
    }
}
