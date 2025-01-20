# Stack Trace Explorer
Parse those pesty unreadable long stack traces. Stack Trace Explorer provides syntax highlighting and easy navigation to elements in the stack trace.

[![StackTraceExplorer](https://github.com/sboulema/StackTraceExplorer/actions/workflows/workflow.yml/badge.svg)](https://github.com/sboulema/StackTraceExplorer/actions/workflows/workflow.yml)
[![Sponsor](https://img.shields.io/badge/-Sponsor-fafbfc?logo=GitHub%20Sponsors)](https://github.com/sponsors/sboulema)

## Installing
[Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=SamirBoulema.StackTraceExplorer)

[Github Releases](https://github.com/sboulema/StackTraceExplorer/releases)

[Open VSIX Gallery](http://vsixgallery.com/extension/StackTraceExplorer.Samir%20Boulema/)

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
There are multiple ways open opening new tabs to show your stack traces:

**Copy a stack trace to your clipboard:**
- Click the paste as new tab but
- Select a tab, make sure your cursor is not in the text editor, Paste your stack trace with `Ctrl + V`

**Stack trace from file:**
- Click the open file button
- Drag & drop the file to the toolwindow

## Supported stack trace formats
- Visual Studio
- Application Insights
- [Ben.Demystifier](https://github.com/benaadams/Ben.Demystifier)

## Screenshots
![Screenshot](https://i.imgur.com/42mKURv.png)

## Debug extension

On `StackTraceExplorer.VS2022` project property, set 'Debug' properties as the following:
* set radio button to `Start external program`
* browse for Visual Studio executable (like `C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe`)
* set `/RootSuffix Exp` in `Command line arguments`

## Thanks
- [Resharper](https://www.jetbrains.com/resharper/) (for the initial idea to recreate this)
- [Terrajobst](https://github.com/terrajobst/stack-trace-explorer) (for some inspiration on optimizing the extension)
- [dlstucki](https://github.com/dlstucki) (for a large PR optimizing StackTraceExplorer for large solutions)
- [pmiossec](https://github.com/pmiossec) (for several PRs reviving StackTraceExplorer)