# Stack Trace Explorer
Parse those pesty unreadable long stack traces. Stack Trace Explorer provides syntax highlighting and easy navigation to elements in the stack trace.

[![Beerpay](https://beerpay.io/sboulema/StackTraceExplorer/badge.svg?style=flat)](https://beerpay.io/sboulema/StackTraceExplorer)

## Installing
[Visual Studio Marketplace](https://marketplace.visualstudio.com/vsgallery/0886a4d9-35e3-431a-b86c-bf0e346ad036) ![Marketplace](http://vsmarketplacebadge.apphb.com/version-short/SamirBoulema.StackTraceExplorer.svg)

[Github Releases](https://github.com/sboulema/StackTraceExplorer/releases)

[Open VSIX Gallery](http://vsixgallery.com/extension/StackTraceExplorer.Samir%20Boulema/)

[AppVeyor](https://ci.appveyor.com/project/sboulema/stacktraceexplorer)
[![Build status](https://ci.appveyor.com/api/projects/status/topfx6jokbg26qs9?svg=true)](https://ci.appveyor.com/project/sboulema/stacktraceexplorer)

## Usage
You can find the Stack Trace Explorer at

`View - Other Windows - Stack Trace Explorer`

or the default keybinding

`Ctrl + S, T`

## Features
- Clicking a filepath will open the file at the line mentioned in the stacktrace
- Clicking a method will open the corresponding file at the start of that method
- Wrap long stacktraces
- Syntax highlighting
- Dark theme support
- Tabs

### Opening a new tab
- Copy a stack trace to your clipboard
- Select a tab, make sure your cursor is not in the text editor
- Paste your stack trace with `Ctrl + V`

## Supported stack trace formats
- Visual Studio
- Application Insights

## Screenshots
![Screenshot](https://i.imgur.com/42mKURv.png)

---

![VS2017 Partner](http://i.imgur.com/wlgwRF1.png)