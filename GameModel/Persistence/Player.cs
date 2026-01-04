namespace Game.Persistence
{
    public class Player
    {
        private int _score;
        public int PlayerId { get; private set; }

        public int Score => _score;

        public Player(int id)
        {
            _score = 0;
            PlayerId = id;
        }

        public int AddPoint(int value)
        {
            _score += value;
            return _score;
        }

        public void ResetScore() { _score = 0; }
    }
}
