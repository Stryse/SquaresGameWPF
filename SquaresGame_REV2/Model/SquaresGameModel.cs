using SquaresGame.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;


namespace SquaresGame.Model
{
    public class Point
    {
        public int X, Y;
        public Point(int X, int Y)
        {
            this.X = X; this.Y = Y;
        }
        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Point p = (Point)obj;
                return (X == p.X) && (Y == p.Y);
            }
        }
    }
    public class Player
    {
        public String PlayerName  { get; set; }
        public Color  PlayerColor { get; set; }
        public int    Points      { get; set; }

        public Player(String pName, Color pColor, int points)
        {
            PlayerName  = pName;
            PlayerColor = pColor;
            Points      = points;
        }

        public Player(String pName, Color pColor) : this(pName,pColor,0)
        {
        }
    }

    public class SquaresGameModel { 

        #region Fields
        //============= Fields =============//
        private List<Tuple<Point, Point, Player>> lines;
        private int linesToEnd;
        private List<Tuple<Point, Point, Player>> rectangles;
        private int registeredRectCount;

        // DataAccess
        private readonly ISquaresGameDataAccess dataAccess;
        #endregion

        #region Properties
        //=========== Properties ===========//
        public int FieldSize { get; private set; }
        public Player PlayerOne { get; private set; }
        public Player PlayerTwo { get; private set; }
        public Player ActivePlayer { get; private set; }
        public bool GameEnded { get; private set; }
        public IReadOnlyList<Tuple<Point,Point,Player>> Lines { get { return lines.AsReadOnly(); } }
        public IReadOnlyList<Tuple<Point,Point,Player>> Rectangles { get { return rectangles.AsReadOnly(); } }

        #endregion

        #region Events
        //============= Events =============// 
        public event EventHandler UpdateUI;
        public event EventHandler<Player> EndGame;

        #endregion

        #region Constructors
        //============= CTORS ==============//
        public SquaresGameModel(int fieldSize, Player playerOne, Player playerTwo, ISquaresGameDataAccess dAccess)
        {
            //Field inits
            FieldSize = (fieldSize > 1) ? fieldSize : throw new ArgumentOutOfRangeException("FieldSize",fieldSize,"value must be > 1");
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            dataAccess = dAccess;

            ActivePlayer = PlayerOne;
            GameEnded  = false;
            lines      = new List<Tuple<Point, Point, Player>>();
            linesToEnd = CalcLinesToEnd();
            rectangles = new List<Tuple<Point, Point, Player>>();
            registeredRectCount = 0;
        }

        //Static factory
        public static SquaresGameModel FromSave(GameStateWrapper state, ISquaresGameDataAccess dAccess)
        {
            SquaresGameModel instance = new SquaresGameModel(state.FieldSize, state.PlayerOne, state.PlayerTwo, dAccess);
            instance.ActivePlayer = state.ActivePlayer;
            instance.lines = state.Lines;
            instance.rectangles = state.Rectangles;
            instance.registeredRectCount = state.RegisteredRectCount;

            return instance;
        }

        #endregion

        #region Methods
        //=========== Methods ===========//

        #region Public
        //=== Public ===//
        public void AddNewLine(Tuple<Point, Point> line)
        {
            //Check validity of input line
            if (IsInBounds(line) && IsLine(line) && IsPermittedLine(line) && !lines.Any(p => IsSameLine(p,line)))
            {
                //If adding line produces new rectangle(s) register rectangle(s)
                UpdateRectangles(line);

                //Adding new Line
                var newLine = SanitizeLine(line);
                var newLineWithPlayer = new Tuple<Point,Point,Player>(newLine.Item1,newLine.Item2,ActivePlayer);
                lines.Add(newLineWithPlayer);

                //Check if game ended
                GameEnded = CheckWinCondition();

                //Check if scored
                if (!IsScored() && !GameEnded)
                    ChangeActivePlayer();
                else
                    RegisterPoints();

                //Broadcast event (UpdateUI)
                OnUpdateUI(this,EventArgs.Empty);
            }

            // Broadcast event (EndGame) if game ended
            if (GameEnded)
            {
                if      (PlayerOne.Points > PlayerTwo.Points) OnEndGame(this, PlayerOne);
                else if (PlayerTwo.Points > PlayerOne.Points) OnEndGame(this, PlayerTwo);
                else                                          OnEndGame(this, null);                  
            }
        }

        public void AddNewLine(Point a, Point b)
        {
            AddNewLine(new Tuple<Point, Point>(a, b));
        }

        public async Task SaveGameAsync(String filePath)
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            GameStateWrapper state = new GameStateWrapper();
            state.PlayerOne = PlayerOne;
            state.PlayerTwo = PlayerTwo;
            state.ActivePlayer = ActivePlayer;
            state.Lines = lines;
            state.Rectangles = rectangles;
            state.RegisteredRectCount = registeredRectCount;
            state.FieldSize = FieldSize;

            await dataAccess.SaveGameAsync(state, filePath);
        }

        public async Task LoadGameAsync(String filePath)
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            GameStateWrapper state = await dataAccess.LoadGameAsync(filePath);
            this.GameEnded = false;
            this.FieldSize = state.FieldSize;
            this.PlayerOne = state.PlayerOne;
            this.PlayerTwo = state.PlayerTwo;
            this.ActivePlayer = state.ActivePlayer;
            this.lines = state.Lines;
            this.linesToEnd = CalcLinesToEnd();
            this.rectangles = state.Rectangles;
            this.registeredRectCount = state.RegisteredRectCount;
        }
        public void Restart()
        {
            //Restore init state
            lines.Clear();
            rectangles.Clear();
            registeredRectCount = 0;
            PlayerOne.Points = 0;
            PlayerTwo.Points = 0;
            ActivePlayer = PlayerOne;
            GameEnded = false;
        }
        #endregion

        #region Private
        //=== Private ===//
        private void ChangeActivePlayer()
        {
            ActivePlayer = (ActivePlayer == PlayerOne) ? PlayerTwo : PlayerOne;
        }

        private Tuple<Point,Point> SanitizeLine(Tuple<Point, Point> line) 
        {
            // Sets line direction left-to-right or top-to-bottom
            int dRow = line.Item2.X - line.Item1.X;
            int dCol = line.Item2.Y - line.Item1.Y;

            if (dRow >= 0 && dCol >= 0)
                return line;
            else
                //Flip
                return new Tuple<Point, Point>(line.Item2, line.Item1);
        }

        private void UpdateRectangles(Tuple<Point, Point> line)
        {
            //Get differences
            int dRow = Math.Abs(line.Item2.X - line.Item1.X);
            int dCol = Math.Abs(line.Item2.Y - line.Item1.Y);

            // Flip Difference vector
            Point diffV = new Point(dCol, dRow);

            //Check existence of parallel lines and side lines in two directions
            for (int i = 0; i < 2; ++i)
            {
                //Change vector direction
                diffV.X *= (-1);
                diffV.Y *= (-1);

                //Setup Parallel line
                Point parallelP1 = new Point(line.Item1.X + diffV.X, line.Item1.Y + diffV.Y);
                Point parallelP2 = new Point(line.Item2.X + diffV.X, line.Item2.Y + diffV.Y);
                var parallel = SanitizeLine(new Tuple<Point, Point>(parallelP1, parallelP2));

                //Check if parallel exist
                bool parallelExists = lines.Any(l =>
                {
                    return (l.Item1.Equals(parallel.Item1) && l.Item2.Equals(parallel.Item2));
                });

                // Only proceed if parallel exists
                if (parallelExists)
                {
                    //Setup Side line
                    var sideOne = SanitizeLine(new Tuple<Point, Point>(line.Item1, parallelP1));
                    var sideTwo = SanitizeLine(new Tuple<Point, Point>(line.Item2, parallelP2));

                    //Check if both side lines exist
                    bool sidesExist = lines.Any(l =>
                    {
                        return l.Item1.Equals(sideOne.Item1) && l.Item2.Equals(sideOne.Item2);
                    })
                                   && lines.Any(l =>
                    {
                        return l.Item1.Equals(sideTwo.Item1) && l.Item2.Equals(sideTwo.Item2);
                    });

                    //All conditions met -> create rectangle
                    if (sidesExist)
                    {
                        Point[] rectPoints = { line.Item1, line.Item2, parallelP1, parallelP2 };
                        Tuple<Point, Point> rect = PointsToRectangle(rectPoints);
                        rectangles.Add(new Tuple<Point, Point, Player>(rect.Item1, rect.Item2, ActivePlayer));
                    }
                }
            }
        }

        private Tuple<Point,Point> PointsToRectangle(Point[] points)
        {
            //Check array validity
            if (points.Length != 4) 
                throw new ArgumentNullException("points", "size of points array must be 4 to be rectangle corner points");           


            Point topLeft;
            Point bottomRight;

            topLeft = points[0];
            bottomRight = points[0];
            for(int i = 0; i < points.Length; ++i)
            {
                if (points[i].X + points[i].Y < topLeft.X + topLeft.Y)
                    topLeft = points[i];

                if (points[i].X + points[i].Y > bottomRight.X + bottomRight.Y)
                    bottomRight = points[i];
            }

            // Check rectangle validity
            if (topLeft.Equals(bottomRight))
                throw new ArgumentException("provided points cannot form a rectangle");

            return new Tuple<Point, Point>(topLeft,bottomRight);
        }

        private bool IsScored()
        {
            return rectangles.Count > registeredRectCount;
        }

        private void RegisterPoints()
        {
            int unregistered = rectangles.Count - registeredRectCount;
            ActivePlayer.Points += unregistered;
            registeredRectCount += unregistered;
        }

        private int CalcLinesToEnd()
        {
            // We are calculating the number of all possible permitted lines with math formula
            
            // 4 corners points -> 2 possible lines per point
            int maxCornerLCount = 4 * 2; 
            
            // (n^2-4n+4) inner points -> 4 possible lines per point
            int maxInnerLCount = ((FieldSize * FieldSize) - (4 * FieldSize) + 4) * 4;

            // (4n-8) side points -> 3 possible lines per point
            int maxSideLCount = ((4 * FieldSize) - 8) * 3;

            // Each line counted twice -> divide sum by 2
            return (maxCornerLCount + maxInnerLCount + maxSideLCount) / 2;
        }

        private bool CheckWinCondition()
        {
            return lines.Count == linesToEnd;
        }

        private bool IsLine(Tuple<Point,Point> line)
        {
            // Line <=> p1 != p2
            return !(line.Item2.X - line.Item1.X == 0 && line.Item2.Y - line.Item1.Y == 0); 
        }

        private bool IsSameLine(Tuple<Point,Point,Player> p1,Tuple<Point,Point> p2)
        {
            // Lines with same points considered to be same regardless direction
            return (p1.Item1 == p2.Item1 && p1.Item2 == p2.Item2) 
              ||   (p1.Item2 == p2.Item1 && p1.Item1 == p2.Item2);
        }

        private bool IsPermittedLine(Tuple<Point,Point> line)
        {
            // Only non diagonal lines are allowed of length 1
            int dRow = Math.Abs(line.Item2.X - line.Item1.X);
            int dCol = Math.Abs(line.Item2.Y - line.Item1.Y);

            return dRow <= 1 && dCol <= 1 && dRow * dCol != 1;
        }

        private bool IsInBounds(Tuple<Point, Point> line)
        {
            return line.Item1.X >= 0 && line.Item1.X < FieldSize
              &&   line.Item1.Y >= 0 && line.Item1.Y < FieldSize
              &&   line.Item2.X >= 0 && line.Item2.X < FieldSize
              &&   line.Item2.Y >= 0 && line.Item2.Y < FieldSize;
        }

        #endregion

        #endregion

        #region Event senders
        //=========== Event Senders ===========//
        protected virtual void OnUpdateUI(object sender, EventArgs e) => UpdateUI?.Invoke(sender,e);
        protected virtual void OnEndGame(object sender,Player p) => EndGame?.Invoke(sender,p);

        #endregion
    }
}
