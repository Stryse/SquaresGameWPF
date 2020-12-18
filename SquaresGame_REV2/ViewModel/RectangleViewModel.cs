using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaresGame_REV2.ViewModel
{
    public class RectangleViewModel : ViewModelBase
    {
        //FIELDS
        private double width;
        private double height;
        private double left;
        private double top;
        private SolidColorBrush color;


        //CTOR
        public RectangleViewModel(double Left,double Top,double Width, double Height, Color color)
        {
            this.Left = Left;
            this.Top = Top;
            this.Width = Width;
            this.Height = Height;
            this.Color = new SolidColorBrush(color);
        }

        //PROPERTIES
        public double Width
        {
            get { return width; }
            set { width = value; OnPropertyChanged(); }
        }
        public double Height
        {
            get { return height; }
            set { height = value; OnPropertyChanged(); }
        }
        public double Left
        {
            get { return left; }
            set { left = value; OnPropertyChanged(); }
        }
        public double Top
        {
            get { return top; }
            set { top = value; OnPropertyChanged(); }
        }
        public SolidColorBrush Color
        {
            get { return color; }
            set { color = value; OnPropertyChanged(); }
        }
    }
}
