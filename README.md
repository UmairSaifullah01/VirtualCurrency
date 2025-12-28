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

#### Create VirtualCurrencyService Asset

1. Create a `Resources` folder in your project (if it doesn't exist)
2. Right-click in the Resources folder → Create → Virtual Currency → Virtual Currency Service
3. Name it `VirtualCurrencyService` (exactly this name)
4. In the Inspector, configure your currencies in the `Currency Configuration` list:
   - **Currency Name**: The name of the currency (e.g., "Coin", "Gem", "Gold")
   - **Value Type**: Choose Int, Float, or BigNumber
   - **Initial Value**: Set based on the selected value type
     - For Int: Set `initialValueInt`
     - For Float: Set `initialValueFloat`
     - For BigNumber: Set `initialValueBigNumber` (format: "mantissa|exponent", e.g., "0|0")

#### Using Currency Name Constants

Use the static `CurrencyType` class for default currency names:

```csharp
using THEBADDEST.VirtualCurrencySystem;

// Use predefined constants
string coinName = CurrencyType.Coin;  // "Coin"
string gemName = CurrencyType.Gem;    // "Gem"
string goldName = CurrencyType.Gold;  // "Gold"

// Or use custom currency names directly
string customCurrency = "Diamonds";
```

### 2. Displaying Currency

Add a currency display to your UI:

Add a `CurrencyCounter` or `CurrencyText` component to a UI GameObject and configure the display format in the Inspector (Normal or Abbreviated), decimal places, and thousands separators.

If you need to set values from code, update the underlying currency via the service (see next section) and the UI bindings will update automatically.

### 3. Managing Currency

```csharp
using THEBADDEST.VirtualCurrencySystem;

// Add currency (int/float/BigNumber overloads supported)
VirtualCurrencyService.AddValue(CurrencyType.Coin, 1000);        // int
VirtualCurrencyService.AddValue(CurrencyType.Coin, 1000.5f);     // float
VirtualCurrencyService.AddValue(CurrencyType.Coin, new BigNumber(100, 0)); // BigNumber

// Get current value - multiple return types available
BigNumber coinsBN = VirtualCurrencyService.GetValue(CurrencyType.Coin);
int coinsInt = VirtualCurrencyService.GetValueInt(CurrencyType.Coin);
float coinsFloat = VirtualCurrencyService.GetValueFloat(CurrencyType.Coin);

// Convert BigNumber to other formats
double coinsAsDouble = coinsBN.ToDouble();
string coinsPretty = coinsBN.ToStringFormatted(2); // e.g., 1.23K

// Set specific value - multiple input types supported
VirtualCurrencyService.SetValue(CurrencyType.Coin, 500);              // int
VirtualCurrencyService.SetValue(CurrencyType.Coin, 500.5f);           // float
VirtualCurrencyService.SetValue(CurrencyType.Coin, BigNumber.FromDouble(500)); // BigNumber

// Persist values
VirtualCurrencyService.Save();
```

### 4. Making Purchases

#### Direct Purchase (Simple)

```csharp
using THEBADDEST.VirtualCurrencySystem;

// Direct purchase with currency name and price
bool success = VirtualCurrencyService.Purchase(CurrencyType.Coin, 100);      // int price
bool success2 = VirtualCurrencyService.Purchase(CurrencyType.Coin, 100.5f);  // float price
bool success3 = VirtualCurrencyService.Purchase(CurrencyType.Coin, new BigNumber(100, 0)); // BigNumber price

if (success)
{
    Debug.Log("Purchase successful!");
}
else
{
    Debug.Log("Insufficient funds!");
}
```

#### Using IPurchasableItem Interface

```csharp
using THEBADDEST.VirtualCurrencySystem;

public class ShopItem : MonoBehaviour, IPurchasableItem
{
    [SerializeField] private string currencyName = CurrencyType.Coin;
    [SerializeField] private float price;
    private bool isPurchased;

    public string CurrencyName => currencyName;
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

1. **Resources Setup**: Always create the `VirtualCurrencyService` asset in a `Resources` folder named exactly "VirtualCurrencyService"
2. **Currency Configuration**: Configure all currencies in the Inspector via the `currencyConfigs` list before building
3. **Value Type Selection**: Choose the appropriate value type (Int/Float/BigNumber) based on your currency needs:
   - Use `Int` for simple whole number currencies
   - Use `Float` for currencies that need decimal precision
   - Use `BigNumber` for very large numbers (millions, billions, etc.)
4. **Save Regularly**: Call `VirtualCurrencyService.Save()` after important currency changes
5. **Use CanPurchase**: Call `VirtualCurrencyService.CanPurchase(item)` before attempting a purchase
6. **Direct Purchase**: Use the direct `Purchase(currencyName, price)` methods for simple purchases without creating `IPurchasableItem` objects
7. **Type-Specific Methods**: Use `GetValueInt()` or `GetValueFloat()` when you know the expected type for better performance
8. **Choose Appropriate Display**: Use abbreviated format for large numbers, normal format for small amounts

## API Reference

### VirtualCurrencyService Methods

#### Currency Value Management

- `GetValue(string currencyName) -> BigNumber`: Get current currency value as BigNumber
- `GetValueInt(string currencyName) -> int`: Get current currency value as int
- `GetValueFloat(string currencyName) -> float`: Get current currency value as float
- `SetValue(string currencyName, BigNumber value)`: Set currency to specific BigNumber value
- `SetValue(string currencyName, int value)`: Set currency to specific int value
- `SetValue(string currencyName, float value)`: Set currency to specific float value
- `AddValue(string currencyName, BigNumber value) -> BigNumber`: Add or subtract currency (BigNumber)
- `AddValue(string currencyName, int value) -> BigNumber`: Add or subtract currency (int)
- `AddValue(string currencyName, float value) -> BigNumber`: Add or subtract currency (float)

#### Purchase Methods

- `Purchase(string currencyName, BigNumber purchasePrice) -> bool`: Direct purchase with BigNumber price
- `Purchase(string currencyName, int purchasePrice) -> bool`: Direct purchase with int price
- `Purchase(string currencyName, float purchasePrice) -> bool`: Direct purchase with float price
- `Purchase(IPurchasableItem) -> bool`: Process a purchase using IPurchasableItem
- `Purchase(IPurchasableItem, Action onSuccess, Action onFailed) -> bool`: Process purchase with callbacks
- `CanPurchase(IPurchasableItem) -> bool`: Check if purchase is possible

#### System Methods

- `Save()`: Save currency values
- `Initialize(VirtualCurrencyService instance)`: Initialize the currency system (called automatically)
- `OnValueChangeRegister(string currencyName, PropertyChangedEventHandler)`: Register for currency change events
- `OnValueChangeUnregister(string currencyName, PropertyChangedEventHandler)`: Unregister from currency change events

### CurrencyType Static Class

Provides default currency name constants:
- `CurrencyType.Coin` → "Coin"
- `CurrencyType.Gem` → "Gem"
- `CurrencyType.Gold` → "Gold"

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
