using System;
using System.Collections.Generic;
using System.Text;

namespace SquaresGame_REV2.ViewModel
{
    public class DotViewModel : ViewModelBase
    {
        //FIELDS
        private double left;
        private double top;
        private double radius;
        private int row;
        private int col;
        private bool isSelected;

        //CTOR
        public DotViewModel(double left, double top, int row, int col,double radius)
        {
            this.Left = left;
            this.Top = top;
            this.Row = row;
            this.Col = col;
            this.Radius = radius;
            this.IsSelected = false;
        }

        //PROPERTIES
        public double Top
        {
            get { return top; }
            set { top = value; OnPropertyChanged(); }
        }
        public double Left
        {
            get { return left; }
            set { left = value; OnPropertyChanged(); }
        }
        public double Radius
        {
            get { return radius; }
            set { radius = value; OnPropertyChanged(); }
        }
        public int Row
        {
            get { return row; }
            set { row = value; OnPropertyChanged(); }
        }
        public int Col
        {
            get { return col; }
            set { col = value; OnPropertyChanged(); }
        }
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; OnPropertyChanged(); }
        }
    }
}
