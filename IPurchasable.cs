using System;
using UnityEngine.Events;

namespace UMGS
{
    public interface IPurchasable
    {
        int Price
        {
            get;
            set;
        }
        string VirtualCurrencyID
        {
            get;
            set;
        }
        bool IsPurchased
        {
            get;
            set;
        }
        bool Purchase (Action OnPurchaseSuccess,Action OnPurchaseFailed,bool isFree=false);
    }
}
