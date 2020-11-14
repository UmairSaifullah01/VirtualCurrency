using System;

namespace UMGS
{
    public interface IPurchasable
    {
        float Price
        {
            get;
            set;
        }
        Currency CurrencyName
        {
            get;
        }
        bool IsPurchased
        {
            get;
            set;
        }
        void Purchased() ;
    }
}
