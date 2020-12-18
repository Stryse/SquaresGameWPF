using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaresGame_REV2.ViewModel
{
    public class LineViewModel : ViewModelBase
    {
        //FIELDS
        private double left1;
        private double top1;
        private int row1;
        private int col1;

        private double left2;
        private double top2;
        private int row2;
        private int col2;

        private SolidColorBrush color;

        //CTOR
        public LineViewModel(double left1, double top1, int row1, int col1,
                             double left2, double top2, int row2, int col2,
                             Color color)
        {
            this.Left1 = left1;
            this.Top1 = top1;
            this.Row1 = row1;
            this.Col1 = col1;

            this.Left2 = left2;
            this.Top2 = top2;
            this.Row2 = row2;
            this.Col2 = col2;
            this.Color = new SolidColorBrush(color);
        }

        //PROPERTIES
        public double Top1
        {
            get { return top1; }
            set { top1 = value; OnPropertyChanged(); }
        }
        public double Left1
        {
            get { return left1; }
            set { left1 = value; OnPropertyChanged(); }
        }
        public int Row1
        {
            get { return row1; }
            set { row1 = value; OnPropertyChanged(); }
        }
        public int Col1
        {
            get { return col1; }
            set { col1 = value; OnPropertyChanged(); }
        }

        public double Top2
        {
            get { return top2; }
            set { top2 = value; OnPropertyChanged(); }
        }
        public double Left2
        {
            get { return left2; }
            set { left2 = value; OnPropertyChanged(); }
        }
        public int Row2
        {
            get { return row2; }
            set { row2 = value; OnPropertyChanged(); }
        }
        public int Col2
        {
            get { return col2; }
            set { col2 = value; OnPropertyChanged(); }
        }
        public SolidColorBrush Color
        {
            get { return color; }
            set { color = value; OnPropertyChanged(); }
        }
    }
}
