using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SquaresGame_REV2.ViewModel
{
    public class ShapeWrapper
    {

        public ObservableCollection<DotViewModel> Dots { get; set; }

        public ObservableCollection<LineViewModel> Lines { get; set; }

        public ObservableCollection<RectangleViewModel> Rectangles { get; set; }

        public ShapeWrapper()
        {
            Dots = new ObservableCollection<DotViewModel>();
            Lines = new ObservableCollection<LineViewModel>();
            Rectangles = new ObservableCollection<RectangleViewModel>();
        }

        public void Clear()
        {
            Dots.Clear();
            Lines.Clear();
            Rectangles.Clear();
        }
    }
}
