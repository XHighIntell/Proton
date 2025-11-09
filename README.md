# Proton
[![GitHub release](https://img.shields.io/github/release/xhighintell/proton/all.svg)](https://github.com/xhighintell/proton/releases) [![GitHub Release Date](https://img.shields.io/github/release-date/xhighintell/proton.svg)](https://github.com/xhighintell/proton/releases) ![.NET](https://img.shields.io/badge/.NET->=%206.0-brightgreen) 

Proton is built on top of the WebView2 control, which enables developers to embed web technologies—HTML, CSS, and JavaScript — into native applications powered by Microsoft Edge (Chromium). Proton inherited ideas from [Electron](https://www.electronjs.org/), adopting its approach of using web technologies to build desktop applications.

## Get it on NuGet
The nuget package can be found at [nuget.org/packages/Proton](https://www.nuget.org/packages/Proton)

## Technology backends
[Electron](https://www.electronjs.org/) uses [NodeJs](https://nodejs.org)  as its backend, while Proton is built on .NET, allowing developers to leverage built-in .NET features and better target the Win32 platform.


## Supportability
The Proton package supports the below environments:
-  net6.0-windows and later version


## Get Started
For quick start you can use win form template at https://github.com/XHighIntell/Proton/tree/master/src/QuickTemplate


```c#
var webView = new ProtonWebView(this) { Dock = DockStyle.Fill };
webView.AllowResizable = true;

var environment = await CoreWebView2Environment.CreateAsync(null, "chromium");
await webView.EnsureCoreWebView2Async(environment);
webView.DefaultBackgroundColor = Color.Transparent;

//WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false; // Uncomment this line if you want to disable the context menus
webView.CoreWebView2.SetVirtualHostNameToFolderMapping("localhost.pro", AppDomain.CurrentDomain.BaseDirectory + "wwwroot", CoreWebView2HostResourceAccessKind.Allow);
webView.CoreWebView2.Navigate("http://localhost.pro/demo.html");

this.Controls.Add(webView);
```