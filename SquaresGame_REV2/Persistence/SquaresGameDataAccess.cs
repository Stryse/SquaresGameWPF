using SquaresGame.Model;
using System;
using System.Media;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SquaresGame.Persistence
{
    public class SquaresGameDataAccess : ISquaresGameDataAccess
    {
        //========= Properties =========//

        // ISquaresGameDataAccess INTERFACE IMPLEMENTATION
        // Loading
        /*
            Input file format:
            LINE: |PlayerName:Str|PlayerColor:Str|Points:int
            LINE: |PlayerName:Str|PlayerColor:Str|Points:int
            LINE: |ActivePlayerInd(0|1):int|FieldSize:int
            LINE: |LineCount:int
            1...LineCount -> [
            LINE: |P1.X P1.Y|P2.X P2.Y|PlayerInd(0|1):int
            ]
            LINE: |RegisteredRectCount:int
            1...RegisteredRectCount -> [
            LINE: |P1.X P1.Y|P2.X P2.Y|PlayerInd(0|1):int
            ]
            EOF
        */
        public async Task<GameStateWrapper> LoadGameAsync(String path)
        {
            GameStateWrapper state = new GameStateWrapper();
            Player[] players = new Player[2];
            
            String line;
            String[] splitLine;

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    //Reading Players
                    for (int i = 0; i < 2; ++i)
                    {
                        //Reading
                        line = await reader.ReadLineAsync();
                        splitLine = line.Split();

                        //Populate Player
                        String pName = splitLine[0];
                        Color pColor = (Color)ColorConverter.ConvertFromString(splitLine[1]);
                        //Color pColor = Color.FromArgb(int.Parse(splitLine[1]));
                        int points = int.Parse(splitLine[2]);
                        players[i] = new Player(pName, pColor, points);
                    }
                    state.PlayerOne = players[0];
                    state.PlayerTwo = players[1];

                    //Read Active Player
                    line = await reader.ReadLineAsync();
                    splitLine = line.Split();
                    state.ActivePlayer = players[int.Parse(splitLine[0])];
                    //Read FieldSize
                    state.FieldSize = int.Parse(splitLine[1]);

                    //Read Lines
                    line = await reader.ReadLineAsync();
                    splitLine = line.Split();
                    int linesLength = int.Parse(splitLine[0]);

                    for(int i = 0; i < linesLength; ++i)
                    {
                        line = await reader.ReadLineAsync();
                        splitLine = line.Split();

                        Point p1 = new Point(int.Parse(splitLine[0]), int.Parse(splitLine[1]));
                        Point p2 = new Point(int.Parse(splitLine[2]), int.Parse(splitLine[3]));
                        int playerInd = int.Parse(splitLine[4]);
                        state.Lines.Add(new Tuple<Point,Point,Player>(p1,p2,players[playerInd]));
                    }

                    //Read Rectangles
                    line = await reader.ReadLineAsync();
                    splitLine = line.Split();
                    state.RegisteredRectCount = int.Parse(splitLine[0]);

                    for (int i = 0; i < state.RegisteredRectCount; i++)
                    {
                        line = await reader.ReadLineAsync();
                        splitLine = line.Split();

                        Point p1 = new Point(int.Parse(splitLine[0]), int.Parse(splitLine[1]));
                        Point p2 = new Point(int.Parse(splitLine[2]), int.Parse(splitLine[3]));
                        int playerInd = int.Parse(splitLine[4]);
                        state.Rectangles.Add(new Tuple<Point, Point, Player>(p1, p2, players[playerInd]));
                    }
                    reader.Close();
                }
                return state;

            } catch(Exception e) {
                throw new Exception("File not exists or invalid");
            }
        }

        // Saving
        /*
            Input file format:
            LINE: |PlayerName:Str|PlayerColor:Str|Points:int
            LINE: |PlayerName:Str|PlayerColor:Str|Points:int
            LINE: |ActivePlayerInd(0|1):int|FieldSize:int
            LINE: |LineCount:int
            1...LineCount -> [
            LINE: |P1.X P1.Y|P2.X P2.Y|PlayerInd(0|1):int
            ]
            LINE: |RegisteredRectCount:int
            1...RegisteredRectCount -> [
            LINE: |P1.X P1.Y|P2.X P2.Y|PlayerInd(0|1):int
            ]
            EOF
        */
        public async Task SaveGameAsync(GameStateWrapper state, String path)
        {
            try
            {
                using(StreamWriter writer = new StreamWriter(path))
                {
                    //Writing players
                    Player p = state.PlayerOne;
                    ColorConverter colorConverter = new ColorConverter();
                    await writer.WriteLineAsync(String.Format("{0} {1} {2}",p.PlayerName,colorConverter.ConvertToString(p.PlayerColor),p.Points));
                    p = state.PlayerTwo;
                    await writer.WriteLineAsync(String.Format("{0} {1} {2}", p.PlayerName,colorConverter.ConvertToString(p.PlayerColor), p.Points));

                    //ActivePlayerInd and FieldSize
                    int activeP = (state.ActivePlayer == state.PlayerOne) ? 0 : 1;
                    await writer.WriteLineAsync(String.Format("{0} {1}",activeP,state.FieldSize));
                    
                    //LineCount and lines
                    await writer.WriteLineAsync(state.Lines.Count.ToString());
                    
                    for(int i = 0; i < state.Lines.Count; ++i)
                    {
                        activeP = (state.Lines[i].Item3 == state.PlayerOne) ? 0 : 1;
                        await writer.WriteLineAsync(String.Format("{0} {1} {2} {3} {4}",
                            state.Lines[i].Item1.X,
                            state.Lines[i].Item1.Y,
                            state.Lines[i].Item2.X,
                            state.Lines[i].Item2.Y,
                            activeP
                            ));
                    }

                    //RegisteredRectanglesCount and rectangles
                    await writer.WriteLineAsync(state.RegisteredRectCount.ToString());

                    for(int i = 0; i < state.RegisteredRectCount; ++i)
                    {
                        activeP = (state.Rectangles[i].Item3 == state.PlayerOne) ? 0 : 1;
                        await writer.WriteLineAsync(String.Format("{0} {1} {2} {3} {4}",
                            state.Rectangles[i].Item1.X,
                            state.Rectangles[i].Item1.Y,
                            state.Rectangles[i].Item2.X,
                            state.Rectangles[i].Item2.Y,
                            activeP
                            ));
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch
            {
                throw new Exception("Error occured during saving process");
            }
        }
    }
}
