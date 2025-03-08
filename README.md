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
    Coin = 1 << 0,
    Cash = 1 << 1,
    Gems = 1 << 2  // Add your custom currencies
}
```

### 2. Displaying Currency

Add a currency display to your UI:

```csharp
[SerializeField] private CurrencyCounter currencyCounter;

// Configure display format
currencyCounter.displayFormat = CurrencyDisplayFormat.Abbreviated;
```

### 3. Managing Currency

```csharp
// Add currency
VCHandler.AddValue(CurrencyType.Coin, 1000);

// Get current value
float coins = VCHandler.GetValue(CurrencyType.Coin);

// Set specific value
VCHandler.SetValue(CurrencyType.Cash, 500);
```

### 4. Implementing Purchasable Items

```csharp
public class ShopItem : MonoBehaviour, IPurchasableWithCurrency
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private float price;
    private bool isPurchased;

    public CurrencyType CurrencyType => currencyType;
    public float Price => price;
    public bool IsPurchased
    {
        get => isPurchased;
        set => isPurchased = value;
    }

    public void Purchase()
    {
        VCHandler.Purchase(this,
            onSuccess: () => Debug.Log("Purchase successful!"),
            onFailed: () => Debug.Log("Not enough currency!")
        );
    }

    public void PurchaseSuccess()
    {
        // Handle successful purchase
    }

    public void PurchasedFailed()
    {
        // Handle failed purchase
    }
}
```

## Best Practices

1. **Save Regularly**: Call `VCHandler.Save()` after important currency changes
2. **Use CanPurchase**: Check if a purchase is possible before attempting it
3. **Implement Callbacks**: Use the callback version of Purchase for better user feedback
4. **Choose Appropriate Display**: Use abbreviated format for large numbers, normal format for small amounts

## API Reference

### VCHandler Methods

- `AddValue(CurrencyType, float)`: Add or subtract currency
- `GetValue(CurrencyType)`: Get current currency value
- `SetValue(CurrencyType, float)`: Set currency to specific value
- `Purchase(IPurchasableWithCurrency)`: Process a purchase
- `CanPurchase(IPurchasableWithCurrency)`: Check if purchase is possible
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
