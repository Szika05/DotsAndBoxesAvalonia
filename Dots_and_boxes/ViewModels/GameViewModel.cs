using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Game.Model;
using Game.Persistence;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace Dots_and_boxes.ViewModels
{

    // define items for ui binding
    public class BoardItem
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }

    public class LineItem : BoardItem
    {
        public string? Id { get; set; } // "H_0_0" or "V_0_0"
        public bool IsDrawn { get; set; }
    }

    public class BoxItem : BoardItem
    {
        public int OwnerId { get; set; }
    }


    public class GameViewModel : ViewModelBase
    {
        #region Fields
        private readonly GameModel _model; //game model
        private string? _statusMessage;
        #endregion

        #region Commands
        public ICommand MakeMoveCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand LoadGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        #endregion

        #region Properties for data bindings

        public int GridSize => _model.Board.GridSize;
        public Board Board => _model.Board;
        public string? StatusMessage
        {
            get => _statusMessage;
            private set { _statusMessage = value; OnPropertyChanged(); }
        }
        public int Player1Score => _model.Player1.Score;
        public int Player2Score => _model.Player2.Score;

        public ObservableCollection<BoxItem> DisplayBoxes { get; } = new();
        public ObservableCollection<LineItem> DisplayHorizontalLines { get; } = new();
        public ObservableCollection<LineItem> DisplayVerticalLines { get; } = new();
        public ObservableCollection<BoardItem> DisplayDots { get; } = new();

        public double BoardWidth => (GridSize * 50);
        public double BoardHeight => (GridSize * 50);

        #endregion

        public GameViewModel(GameModel? gameModel = null)
        {
            if (gameModel is null) _model = new GameModel(new BoxesFileDataAccess());
            else _model = gameModel;

            // Subscribe to model events
            _model.GameUpdated += OnGameUpdated;
            _model.GameEnded += OnGameEnded;
            _model.TurnChanged += OnTurnChanged;

            // Initialize commands
            MakeMoveCommand = new RelayCommand(OnMove);
            NewGameCommand = new RelayCommand(OnNewGame);
            LoadGameCommand = new RelayCommand(OnLoadGame);
            SaveGameCommand = new RelayCommand(OnSaveGame);

            // Initial status update
            _model.NewGame(BoardSize.Small);
            UpdateStatus();
            RefreshBoardDisplay();
        }

        /// <summary>
        /// Refreshing all bindings for board display.
        /// </summary>
        private void RefreshBoardDisplay()
        {
            // clearing collections
            DisplayBoxes.Clear();
            DisplayHorizontalLines.Clear();
            DisplayVerticalLines.Clear();
            DisplayDots.Clear();

            int size = _model.Board.GridSize;

            // creating new dots
            for (int r = 0; r <= size; r++)
                for (int c = 0; c <= size; c++)
                    DisplayDots.Add(new BoardItem { Row = r, Col = c });

            // displaying boxes with appropriate owner colors
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    var box = _model.Board.GetBoxes()[r, c];
                    DisplayBoxes.Add(new BoxItem { Row = r, Col = c, OwnerId = box.OwnerId });
                }
            }

            // displaying horizontal and vertical lines according to the game state
            for (int r = 0; r <= size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    bool isDrawn = _model.Board.GetHLines()[r, c];
                    DisplayHorizontalLines.Add(new LineItem { Row = r, Col = c, Id = $"H_{r}_{c}", IsDrawn = isDrawn });
                }
            }

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c <= size; c++)
                {
                    bool isDrawn = _model.Board.GetVLines()[r, c];
                    DisplayVerticalLines.Add(new LineItem { Row = r, Col = c, Id = $"V_{r}_{c}", IsDrawn = isDrawn });
                }
            }

            OnPropertyChanged(nameof(BoardWidth));
            OnPropertyChanged(nameof(BoardHeight));
        }

        #region player move commands
        /// <summary>
        /// Only allow actions on the board when there are legal moves.
        /// </summary>
        private bool CanExecuteMakeMove(object? parameter) => !Board.IsFull();

        /// <summary>
        /// Adding a line to the game models horizontal and vertical lines.
        /// </summary>
        private void ExecuteMakeMove(object? parameter)
        {
            if (parameter is not string lineId) return;

            // lineId format: "H_row_col" or "V_row_col"
            string[] parts = lineId.Split('_');
            if (parts.Length != 3) return;

            if (!int.TryParse(parts[1], out int row) || !int.TryParse(parts[2], out int col)) return;

            if (parts[0] == "H")
            {
                _model.AddHorizontalLine(row, col);
            }
            else if (parts[0] == "V")
            {
                _model.AddVerticalLine(row, col);
            }
        }

        /// <summary>
        /// Creating a new game based on chosen difficulty.
        /// </summary>
        private void ExecuteNewGame(object? parameter)
        {

            if (parameter is BoardSize size)
            {
                _model.NewGame(size);
            }
            else
            {
                _model.NewGame(BoardSize.Small);
            }


            OnPropertyChanged(nameof(GridSize));
            OnPropertyChanged(nameof(Board));
            OnPropertyChanged(nameof(Player1Score));
            OnPropertyChanged(nameof(Player2Score));
            UpdateStatus();
            RefreshBoardDisplay();
        }
        #endregion

        #region Data loading and saving
        /// <summary>
        /// Loading in a saved game.
        /// </summary>
        private async Task ExecuteLoadGameAsync()
        {
            // Access the TopLevel to get the StorageProvider (works on Desktop and Android)
            var topLevel = GetTopLevel();
            if (topLevel == null) return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Load Boxes and Dots Game",
                FileTypeFilter = new[] { new FilePickerFileType("Boxes Game Save") { Patterns = new[] { "*.box" } } }
            });

            if (files.Count > 0)
            {
                try
                {
                    StatusMessage = "Loading game...";
                    // Use LocalPath for compatibility with your existing model
                    await _model.LoadGameAsync(files[0].Path.LocalPath);
                    RefreshBoardDisplay();
                    StatusMessage = "Game successfully loaded.";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error loading game: {ex.Message}";
                }
            }
        }

        // Helper to get the current TopLevel (Window or VisualRoot)
        private TopLevel? GetTopLevel()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.MainWindow;
            if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
                return TopLevel.GetTopLevel(singleView.MainView);
            return null;
        }

        /// <summary>
        /// Saving the ongoing game.
        /// </summary>
        private async Task ExecuteSaveGameAsync()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Boxes Game Save (*.box)|*.box",
                Title = "Save Boxes and Dots Game",
                FileName = $"BoxesGame_{DateTime.Now:yyyyMMdd_HHmmss}.box"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    StatusMessage = "Saving game...";

                    // The model handles the async saving using the injected data access
                    await _model.SaveGameAsync(saveFileDialog.FileName);

                    StatusMessage = $"Game successfully saved.";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error saving game: {ex.Message}";
                }
            }
        }
        #endregion

        #region event handlers

        /// <summary>
        /// When something happens in the game update ui.
        /// </summary>
        private void OnGameUpdated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Player1Score));
            OnPropertyChanged(nameof(Player2Score));
            RefreshBoardDisplay();
        }

        /// <summary>
        /// Changing whose going to make the next moves according to rules.
        /// </summary>
        private void OnTurnChanged(object? sender, EventArgs e)
        {
            UpdateStatus();
        }

        /// <summary>
        /// Display winner of the game on the top, and end the game.
        /// </summary>
        private void OnGameEnded(object? sender, EventArgs e)
        {
            int p1 = _model.Player1.Score;
            int p2 = _model.Player2.Score;

            string winner = (p1 > p2) ? "Player 1 Wins!"
                          : (p2 > p1) ? "Player 2 Wins!"
                          : "It's a Draw!";

            StatusMessage = $"Game Over! {winner} (P1: {p1}, P2: {p2})";
        }

        /// <summary>
        /// Update the top status bar according to whose turn it is.
        /// </summary>
        private void UpdateStatus()
        {
            string playerColor = (_model.CurrentPlayer.PlayerId == 1) ? "Blue" : "Red";
            StatusMessage = $"Player {_model.CurrentPlayer.PlayerId}'s turn ({playerColor})";
        }
        #endregion
    }
}