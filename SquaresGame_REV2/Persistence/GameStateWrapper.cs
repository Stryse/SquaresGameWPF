using SquaresGame.Model;
using System;
using System.Collections.Generic;
using System.Media;

namespace SquaresGame.Persistence
{
    public class GameStateWrapper
    {
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public Player ActivePlayer { get; set; }
        public List<Tuple<Point, Point, Player>> Lines { get; set; }
        public List<Tuple<Point, Point, Player>> Rectangles { get; set; }
        public int RegisteredRectCount { get; set; }
        public int FieldSize { get; set; }

        public GameStateWrapper()
        {
            Lines = new List<Tuple<Point, Point, Player>>();
            Rectangles = new List<Tuple<Point, Point, Player>>();
        }
    }
}
