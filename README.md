# Virtual Currency System

A robust and flexible virtual currency management system for Unity games. This package provides an easy-to-use framework for implementing and managing multiple types of in-game currencies with various display and animation options.

## Features

- 🎮 **Easy Integration**: Simple setup process for Unity projects
- 💰 **Multiple Currency Support**: Handle different types of currencies (Coins, Cash, Gems, etc.)
- 🔄 **Automatic Save System**: Built-in persistence for currency values
- 📊 **Flexible Display Options**:
  - Normal number format with optional thousands separator
  - Abbreviated format (K, M, B, T)
  - Customizable decimal places
- ✨ **Currency Update Effects**:
  - Sudden update
  - Smooth counter animation
  - Coin collection animation with object pooling
- 🛍️ **Purchase System**:
  - Simple purchase interface
  - Purchase validation
  - Success/Failure callbacks
  - Sufficient funds checking

## Installation

1. Open Unity Package Manager
2. Click the + button in the top-left corner
3. Select "Add package from git URL"
4. Enter: `https://github.com/yourusername/VirtualCurrency.git`

## Quick Start

### 1. Setting Up Currencies

Add your currency types to the `CurrencyType` enum:

```csharp
public enum CurrencyType
{
    Coin = 0,
    Cash = 1,
    Gems = 2  // Add your custom currencies sequentially
}
```

### 2. Displaying Currency

Add a currency display to your UI:

Add a `CurrencyCounter` or `CurrencyText` component to a UI GameObject and configure the display format in the Inspector (Normal or Abbreviated), decimal places, and thousands separators.

If you need to set values from code, update the underlying currency via the service (see next section) and the UI bindings will update automatically.

### 3. Managing Currency

```csharp
using THEBADDEST.VirtualCurrencySystem;

// Add currency (int/float overloads supported)
VirtualCurrencyService.AddValue(CurrencyType.Coin, 1000);

// Get current value (returns BigNumber)
var coins = VirtualCurrencyService.GetValue(CurrencyType.Coin);
double coinsAsDouble = coins.ToDouble();
string coinsPretty = coins.ToStringFormatted(2); // e.g., 1.23K

// Set specific value
VirtualCurrencyService.SetValue(CurrencyType.Cash, BigNumber.FromDouble(500));

// Persist values
VirtualCurrencyService.Save();
```

### 4. Implementing Purchasable Items

```csharp
using THEBADDEST.VirtualCurrencySystem;

public class ShopItem : MonoBehaviour, IPurchasableItem
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private float price;
    private bool isPurchased;

    public CurrencyType CurrencyType => currencyType;
    public BigNumber Price => BigNumber.FromDouble(price);
    public bool IsPurchased
    {
        get => isPurchased;
        set => isPurchased = value;
    }

    public void Purchase()
    {
        VirtualCurrencyService.Purchase(
            this,
            onSuccess: () => Debug.Log("Purchase successful!"),
            onFailed: () => Debug.Log("Not enough currency!")
        );
    }

    public void PurchaseSuccess() { }
    public void PurchasedFailed() { }
}
```

### 5. Authoring Purchasable Items via ScriptableObject

You can also author items as assets using `PurchasableItemSO`:

1. Right-click in the Project window → Create → Virtual Currency → Purchasable Item
2. Set `Currency Type` and `Price` (optional: Display Name, Icon)
3. Reference the asset in your shop and pass it to `VirtualCurrencyService.Purchase(asset)`

## Best Practices

1. **Save Regularly**: Call `VirtualCurrencyService.Save()` after important currency changes
2. **Use CanPurchase**: Call `VirtualCurrencyService.CanPurchase(item)` before attempting a purchase
3. **Implement Callbacks**: Use the callback version of `Purchase` for better user feedback
4. **Choose Appropriate Display**: Use abbreviated format for large numbers, normal format for small amounts

## API Reference

### VirtualCurrencyService Methods

- `AddValue(CurrencyType, BigNumber)`: Add or subtract currency
- `AddValue(CurrencyType, int|float)`: Convenience overloads
- `GetValue(CurrencyType) -> BigNumber`: Get current currency value
- `SetValue(CurrencyType, BigNumber)`: Set currency to specific value
- `Purchase(IPurchasableItem)`: Process a purchase
- `Purchase(IPurchasableItem, Action onSuccess, Action onFailed)`: Process with callbacks
- `CanPurchase(IPurchasableItem) -> bool`: Check if purchase is possible
- `Save()`: Save currency values
- `Initialize()`: Initialize the currency system (called automatically)

### Display Formats

- `Normal`: Standard number format (e.g., 1,234,567)
- `Abbreviated`: Shortened format (e.g., 1.2M)

### Currency Update Effects

- `Sudden`: Instant update
- `CounterUpdate`: Animated counting effect
- `MoveToIconPosition`: Animated coins collection effect

## Author Information

- **Name**: Umair Saifullah
- **Email**: contact@umairsaifullah.com
- **Website**: [umairsaifullah.com](https://www.umairsaifullah.com)

## License

This project is licensed under the MIT License. For more details, refer to the LICENSE file.


Made with ❤️ by Umair Saifullah
