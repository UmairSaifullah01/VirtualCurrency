# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2025-01-15

### Added

- **Configurable Currency System**: Replaced hardcoded `CurrencyType` enum with inspector-configurable `CurrencyConfig` list
  - Each currency can be configured with name, initial value, and value type (Int, Float, or BigNumber)
  - Currencies can be added/removed dynamically in the Unity Inspector
- **CurrencyValueType Enum**: Support for different value types per currency (Int, Float, BigNumber)
- **CurrencyType Static Class**: Static readonly string constants for default currency names (Coin, Gem, Gold)
- **Custom Property Drawer**: `CurrencyConfigDrawer` that conditionally shows only the relevant initial value field based on selected value type
- **Resources Loading**: `EnsureInitialized()` now loads `VirtualCurrencyService` from Resources folder
- **GetValue Overloads**: Added `GetValueInt()` and `GetValueFloat()` methods for convenient type conversion
- **SetValue Overloads**: Added `SetValue()` overloads that accept `int` and `float` directly
- **Direct Purchase Methods**: Added `Purchase(string currencyName, price)` overloads for direct purchases without `IPurchasableItem`
  - Supports `int`, `float`, and `BigNumber` price types

### Changed

- **Currency System Architecture**: Migrated from enum-based to string-based currency identification
  - All currency operations now use currency name strings instead of enum values
  - `IPurchasableItem.CurrencyType` property renamed to `CurrencyName` (returns string)
  - `VirtualCurrency.type` field renamed to `currencyName` (string)
- **Initialization**: Service now attempts to load from Resources before creating a new instance
- **Default Currencies**: Changed from Coin/Cash/Gems to Coin/Gem/Gold to match `CurrencyType` constants

### Migration Notes

- Replace `CurrencyType.Coin` enum usage with `CurrencyType.Coin` string constant (or use string directly)
- Update `IPurchasableItem` implementations: change `CurrencyType CurrencyType` to `string CurrencyName`
- Update `CurrencyBinding` components: change `CurrencyType currencyTypeName` to `string currencyName`
- Create a `VirtualCurrencyService` asset in a `Resources` folder named "VirtualCurrencyService" for proper initialization
- Configure currencies in the Inspector via the `currencyConfigs` list in `VirtualCurrencyService` ScriptableObject

### Fixed

- Improved type safety with explicit value type configuration per currency
- Better Inspector experience with conditional field display based on value type

## [2.0.0] - 2025-10-29

### Added

- `PurchasableItemSO` ScriptableObject implementing `IPurchasableItem` for asset-authored items
- XML documentation across purchasing types for clearer API usage
- README updates with ScriptableObject workflow and `BigNumber` examples

### Changed

- Renamed `VCHandler` to `VirtualCurrencyService` (central API for init/save/purchase)
- Replaced `BigCurrency` with `BigNumber` for numeric operations and formatting
- Renamed `IPurchasable`/`IPurchasableWithCurrency` to `IPurchasableItem`
- Updated samples and API docs to use `BigNumber` and new service/interface names

### Removed

- Old `IPurchasable` and `IPurchasableWithCurrency` references in docs/samples

### Migration Notes

- Replace `VCHandler.*` calls with `VirtualCurrencyService.*`
- Swap value type usages from `BigCurrency` to `BigNumber`
- Update implementations from `IPurchasable` to `IPurchasableItem`

### Fixed

- Documentation mismatches between README and code

## [1.0.1] - 2025-03-08

### Added

- New `IPurchasableWithCurrency` interface for simplified purchasing
- Object pooling system for coin animations using Unity's built-in pooling
- `CanPurchase` method to check if purchase is possible before attempting
- Abbreviated number format (K, M, B, T) for currency display
- Customizable decimal places for abbreviated numbers
- Thousands separator option for normal number format

### Changed

- Improved coin collection animation with better curve movement
- Optimized currency update effects
- Better type safety in purchase system
- Reorganized code structure for better maintainability

### Deprecated

- `IPurchasable` interface and related methods (use `IPurchasableWithCurrency` instead)
- Old `Buy` methods in favor of new `Purchase` methods
- Multiple currency support per purchasable item

### Fixed

- Memory leaks in coin animation system
- Currency value precision issues
- UI update synchronization problems
- Save system race conditions

## [1.0.0] - 2024-03-10

### Added

- Basic save system for currency values
- Simple currency display
- Basic purchase system with `IPurchasable`

### Fixed

- Currency value not updating properly
- Save system not working on some platforms
- UI refresh issues

## [0.1.0] - 2024-03-01

### Added

- Initial release
- Basic currency management system
- Simple currency display
- Basic purchase interface
- Currency type enum
- Basic save/load functionality
