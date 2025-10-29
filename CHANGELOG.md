# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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

## [1.1.0] - 2025-03-08

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

## [1.0.1] - 2024-03-10

### Added

- Basic save system for currency values
- Simple currency display
- Basic purchase system with `IPurchasable`

### Fixed

- Currency value not updating properly
- Save system not working on some platforms
- UI refresh issues

## [1.0.0] - 2024-03-01

### Added

- Initial release
- Basic currency management system
- Simple currency display
- Basic purchase interface
- Currency type enum
- Basic save/load functionality
