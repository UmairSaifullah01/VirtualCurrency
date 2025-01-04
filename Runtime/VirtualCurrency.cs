using System.ComponentModel;
using System.Runtime.CompilerServices;
using THEBADDEST.DataManagement;


namespace THEBADDEST.VirtualCurrencySystem
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
		public bool     IsAvailable => DataPersistor.Contains(Name.ToString());

		public float value
		{
			get => DataPersistor.Get<float>(Name.ToString());
			set
			{
				DataPersistor.Save(Name.ToString(), value);
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