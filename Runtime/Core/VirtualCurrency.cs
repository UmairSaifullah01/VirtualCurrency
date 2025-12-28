using System.ComponentModel;
using THEBADDEST.DataManagement;
using System.Runtime.CompilerServices;


namespace THEBADDEST.VirtualCurrencySystem
{


	[System.Serializable]
	public class VirtualCurrency : INotifyPropertyChanged
	{

		BigNumber m_value; // changed from float
		public string currencyName;


		public BigNumber value
		{
			get => m_value;
			set
			{
				m_value = value;
				OnPropertyChanged();
			}
		}

		public VirtualCurrency(string currencyName, BigNumber initValue)
		{
			this.currencyName = currencyName;
			this.value = initValue;
		}


		public event PropertyChangedEventHandler PropertyChanged;


		public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}


}