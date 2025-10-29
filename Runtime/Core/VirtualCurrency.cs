using System.ComponentModel;
using THEBADDEST.DataManagement;
using System.Runtime.CompilerServices;


namespace THEBADDEST.VirtualCurrencySystem
{


	[System.Serializable]
	public class VirtualCurrency : INotifyPropertyChanged
	{

		BigNumber m_value; // changed from float
		public CurrencyType type;


		public BigNumber value
		{
			get => m_value;
			set
			{
				m_value = value;
				OnPropertyChanged();
			}
		}

		public VirtualCurrency(CurrencyType type, BigNumber initValue)
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