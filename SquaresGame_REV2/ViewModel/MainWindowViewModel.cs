using SquaresGame.Model;
using SquaresGame.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;

namespace SquaresGame_REV2.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        //FIELDS View
        private int fieldSize;
        private double dotRadius;
        private readonly double canvasWidth = 600.0;
        private readonly double canvasHeight = 600.0;

        //FIELDS Mouse related
        private DotViewModel startClickDot = null;
        private DotViewModel endClickDot = null;

        //FIELDS Game
        private SquaresGameModel model;
        private SquaresGameDataAccess persistence;
        private PlayerViewModel playerOne;
        private PlayerViewModel playerTwo;

        //Shape collections
        public ShapeWrapper Shapes { get; set; }

        //COMMANDS
        public DelegateCommand SelectFirstDot { get; set; }
        public DelegateCommand SelectSecondDot { get; set; }
        public DelegateCommand NewGameCommand { get; set; }

        //CTOR
        public MainWindowViewModel()
        {
            //DEFAULT SETTINGS
            FieldSize = 5;
            dotRadius = 15;
            playerOne = new PlayerViewModel(new Player("Player One", Colors.Coral),Colors.Coral);
            playerTwo = new PlayerViewModel(new Player("Player Two", Colors.Green),Colors.Green);

            persistence = new SquaresGameDataAccess();
            model = new SquaresGameModel(FieldSize, playerOne.Player, playerTwo.Player, persistence);
            model.UpdateUI += UpdateUI;
            model.EndGame += PlayerWon;
            Shapes = new ShapeWrapper();

            //Commands
            SelectFirstDot = new DelegateCommand(HandleFirstDotClicked);
            SelectSecondDot = new DelegateCommand(HandleSecondDotClicked);
            NewGameCommand = new DelegateCommand(NewGame);

            InitDots(FieldSize);
        }

        //METHODS
        private void InitDots(int N)
        {
            Shapes.Dots.Clear();
            for (int x = 1; x <= N; ++x)
            {
                for (int y = 1; y <= N; ++y)
                {
                    float xCenter = x * ((float)canvasWidth / (N + 1));
                    float yCenter = y * ((float)canvasHeight / (N + 1));
                    Shapes.Dots.Add(new DotViewModel(xCenter-dotRadius, yCenter-dotRadius, x - 1, y - 1, 2*dotRadius));
                }
            }
        }

        public void HandleFirstDotClicked(object o)
        {
            var values = (int[])o;
            int Row = values[0];
            int Col = values[1];

            //Clicked Same
            if(startClickDot != null && startClickDot.Row == Row && startClickDot.Col == Col)
            {
                //Deselect
                startClickDot.IsSelected = false;
                startClickDot = null;
                return;
            }

            //Clicked Another
            if(startClickDot != null)
            {
                startClickDot.IsSelected = false;
                startClickDot = null;
            }

            startClickDot = Shapes.Dots[FieldSize * Row + Col];
            startClickDot.IsSelected = true;
        }

        public void HandleSecondDotClicked(object o)
        {
            //Start must precede
            if (startClickDot == null) return;

            var values = (int[])o;
            int Row = values[0];
            int Col = values[1];

            //Toggle on
            if (IsValidEndPoint(Row, Col))
            {
                endClickDot = Shapes.Dots[FieldSize * Row + Col];
                model.AddNewLine(new Tuple<Point, Point>(new Point(startClickDot.Row, startClickDot.Col)
                                                        ,new Point(endClickDot.Row, endClickDot.Col)));

                startClickDot.IsSelected = false;
                startClickDot = null;
                endClickDot = null;
            }
        }

        public bool IsValidEndPoint(int row, int col)
        {
            if (startClickDot != null)
            {
                int dRow = Math.Abs(startClickDot.Row - row);
                int dCol = Math.Abs(startClickDot.Col - col);

                if (dRow == 0 && dCol == 0) return false;

                return dRow <= 1 && dCol <= 1 && dRow * dCol != 1; 
            }
            return false;
        }

        private void UpdateUI(object sender, EventArgs e)
        {
            Shapes.Lines.Clear();
            for(int i = 0; i < model.Lines.Count; ++i)
            {
                int Row1Num = model.Lines[i].Item1.X;
                int Col1Num = model.Lines[i].Item1.Y;

                int Row2Num = model.Lines[i].Item2.X;
                int Col2Num = model.Lines[i].Item2.Y;

                double Left1 = Shapes.Dots[FieldSize * Row1Num + Col1Num].Left + dotRadius;
                double Top1 = Shapes.Dots[FieldSize * Row1Num + Col1Num].Top + dotRadius;
                double Left2 = Shapes.Dots[FieldSize * Row2Num + Col2Num].Left + dotRadius;
                double Top2 = Shapes.Dots[FieldSize * Row2Num + Col2Num].Top + dotRadius;

                Shapes.Lines.Add(new LineViewModel(Left1, Top1, Row1Num, Col1Num,
                                                   Left2, Top2, Row2Num, Col2Num,
                                                   model.Lines[i].Item3.PlayerColor));
            }

            Shapes.Rectangles.Clear();
            for(int i = 0; i < model.Rectangles.Count; ++i)
            {
                int Row1Num = model.Rectangles[i].Item1.X;
                int Col1Num = model.Rectangles[i].Item1.Y;

                int Row2Num = model.Rectangles[i].Item2.X;
                int Col2Num = model.Rectangles[i].Item2.Y;

                double Left1 = Shapes.Dots[FieldSize * Row1Num + Col1Num].Left + dotRadius;
                double Top1 = Shapes.Dots[FieldSize * Row1Num + Col1Num].Top + dotRadius;
                double Left2 = Shapes.Dots[FieldSize * Row2Num + Col2Num].Left + dotRadius;
                double Top2 = Shapes.Dots[FieldSize * Row2Num + Col2Num].Top + dotRadius;

                double Width = Math.Abs(Left2 - Left1);
                double Height = Math.Abs(Top2 - Top1);
                Shapes.Rectangles.Add(new RectangleViewModel(Left1, Top1, Width, Height, model.Rectangles[i].Item3.PlayerColor));
            }

            OnPropertyChanged("PlayerOne");
            OnPropertyChanged("PlayerTwo");
        }

        private void PlayerWon(object sender, Player p)
        {
            String message;
            if (p == null) message = "Draw";
            else message = String.Format("{0} has won with {1} points", p.PlayerName, p.Points);

            System.Windows.MessageBox.Show(message);
            model.Restart();
            UpdateUI(this, EventArgs.Empty);
        }

        private void NewGame(object o)
        {
            int size = int.Parse((String)o);
            FieldSize = size;
            PlayerOne.Player.Points = 0;
            PlayerTwo.Player.Points = 0;
            OnPropertyChanged("PlayerOne");
            OnPropertyChanged("PlayerTwo");
            model = new SquaresGameModel(FieldSize, PlayerOne.Player, PlayerTwo.Player, persistence);
            model.UpdateUI += UpdateUI;
            model.EndGame += PlayerWon;
            Shapes.Clear();
            InitDots(FieldSize);
        }

        //PROPERTIES
        public PlayerViewModel PlayerOne
        {
            get { return playerOne; }
            private set { playerOne = value; OnPropertyChanged(); }
        }
        public PlayerViewModel PlayerTwo
        {
            get { return playerTwo; }
            private set { playerTwo = value; OnPropertyChanged(); }
        }
        public int FieldSize
        {
            get { return fieldSize; }
            set { fieldSize = value; OnPropertyChanged(); }
        }
        public double DotSize
        {
            get { return dotRadius; }
            set { dotRadius = value; OnPropertyChanged(); }
        }
    }
}
