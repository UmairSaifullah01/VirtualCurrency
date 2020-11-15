using System.ComponentModel;
using GameDevUtils.DataManagement;
using System.Runtime.CompilerServices;


namespace GameDevUtils.VirtualCurrencySystem
{


	[System.Serializable]
	public class VirtualCurrency : INotifyPropertyChanged
	{

		public VirtualCurrency(Currency name, float initValue = 0)
		{
			Name = name;
			if (!IsAvailable)
			{
				value = initValue;
			}
		}

		public Currency Name;
		public bool     IsAvailable => DataManager.Contains(Name.ToString());

		public float value
		{
			get => DataManager.Get<float>(Name.ToString());
			set
			{
				DataManager.Save(Name.ToString(), value);
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;


		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}


}