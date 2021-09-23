# magentoapi.authentication

This library provides requesting authentication tokens from a magento 2 shop.

## Install cake

```
dotnet tool install --global Cake.Tool --version 1.2.0
```

if cake is already install update

```
dotnet tool update --global Cake.Tool --version 1.2.0
```

## Build

Call the build target in solution root.

```
dotnet cake --target="Build"
```

## Test

Call the test target in solution root.

```
dotnet cake --target="Test"
```

## Deploy

Call the deploy target in solution root. The argument ```NugetDeployApiKey``` is mandatory.

```
dotnet cake --target="Deploy" --NugetDeployApiKey=ABC1234
```
