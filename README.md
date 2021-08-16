# Libre Cards Against Humanity

<a href="https://github.com/petrspelos/libre-cards-against-humanity/actions">
  <img alt="GitHub Workflow Status (branch)" src="https://img.shields.io/github/workflow/status/petrspelos/libre-cards-against-humanity/.NET/main?style=for-the-badge">
</a>

<a href="https://github.com/petrspelos/libre-cards-against-humanity/graphs/contributors">
  <img src="https://img.shields.io/github/contributors/petrspelos/libre-cards-against-humanity.svg?style=for-the-badge" alt="contributors">
</a>
<a href="https://spelos.net/chat">
  <img src="https://img.shields.io/badge/Chat-Matrix-%230DBD8B?style=for-the-badge" alt="matrix">
</a>
<a href="https://github.com/petrspelos/libre-cards-against-humanity/blob/master/LICENSE">
  <img src="https://img.shields.io/badge/License-GPLv3-blue.svg?style=for-the-badge" alt="license">
</a>

Free and Open Source implementation of Cards against humanity using Signal R.

## Requirements

🔧 In order to build this project, you'll need the [.NET 6 preview SDK](https://dotnet.microsoft.com/download/dotnet/6.0).

## How to contribute

♥ If you'd like to contribute, please see the [Contributing Document](CONTRIBUTING.md) to see how to get started.

## Building the project

There are two main ways of building the project.

### CLI

1. Verify you have the correct version of .NET installed by running the following command in a command line:

```bash
dotnet --list-sdks
```

_This should output at least one version starting with `6.`_

2. Run the following command to build the project:

```bash
dotnet build src/
```

### Visual Studio

1. [Visual Studio 2022](https://devblogs.microsoft.com/visualstudio/visual-studio-2022/) with .NET 6 SDK is required for building this project

1. Open `src/LibreCards.sln` in Visual Studio and build as usual
