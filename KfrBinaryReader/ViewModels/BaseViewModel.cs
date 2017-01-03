using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KfrBinaryReader.ViewModels {
	public abstract class BaseViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
			if(!EqualityComparer<T>.Default.Equals(storage, value)) {
				storage = value;
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
