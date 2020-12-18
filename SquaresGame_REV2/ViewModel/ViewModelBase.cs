using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SquaresGame_REV2.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region --Events--

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region --Constructor--

        protected ViewModelBase()
        {
        }

        #endregion


        #region --Event Methods--

        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
