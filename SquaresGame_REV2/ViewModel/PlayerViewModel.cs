using SquaresGame.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaresGame_REV2.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        //FIELDS
        private Player player;
        private SolidColorBrush color;

        //CTOR
        public PlayerViewModel(Player player, Color color)
        {
            this.Player = player;
            this.Color = new SolidColorBrush(color);
        }


        //PROPERTIES
        public Player Player
        {
            get { return player; }
            set { player = value; OnPropertyChanged(); }
        }

        public SolidColorBrush Color
        {
            get { return color; }
            set { color = value; OnPropertyChanged(); }
        }
    }
}
