using System.ComponentModel;
using THEBADDEST.DataManagement;
using System.Runtime.CompilerServices;


namespace THEBADDEST.VirtualCurrencySystem
{


	[System.Serializable]
	public class VirtualCurrency : INotifyPropertyChanged
	{

		float               m_value;
		public CurrencyType type;


		public float value
		{
			get => m_value;
			set
			{
				m_value = value;
				OnPropertyChanged();
			}
		}

		public VirtualCurrency(CurrencyType type, float initValue = 0)
		{
			this.type  = type;
			this.value = initValue;
		}


		public event PropertyChangedEventHandler PropertyChanged;


		public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}


}