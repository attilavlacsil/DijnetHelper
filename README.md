# DijnetHelper

A Xamarin.Forms application to view and pay your dijnet bills on Android and iOS. [_Magyar leírás._](#magyar-leírás)

## Introduction

This project is a Proof of Concept for paying dijnet bills by **already registered cards** through a mobile application.

## Features

* View list of unpaid bills
* View details of an unpaid bill
* Initiate the payment of an unpaid bill by a registered card
* Store credentials (securely) and auto-login at startup

## Preview

<div>
  <img src="/preview/Android/1-login.png" width="120px" />
  <img src="/preview/Android/2-billpage.png" width="120px" />
  <img src="/preview/Android/3-billpage.png" width="120px" />
  <img src="/preview/Android/4-paypage.png" width="120px" />
  <img src="/preview/Android/5-paypage.png" width="120px" />
</div>

## Build and test

Download or clone the repository, open the solution, build and deploy to your platform.

I used to test on Android only with
* Visual Studio Community 2019 16.5.3
* Xamarin 16.5.000.533
* Xamarin.Android SDK 10.2.0.100

The iOS implementation is done, including the [App Store requirement](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/webview#uiwebview-deprecation-and-app-store-rejection-itms-90809), but it may not work and require improvement.

You may need to log in to dijnet on a regular browser and set the page language to Hungarian.

You can test with fake dependencies by setting `FAKE_AUTH` and `FAKE_BROWSER` symbols.

## Implementation

User interaction is simulated by a hidden [Xamarin.Forms WebView](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/webview), which extended to support injection of cookies. Once authentication cookies have been injected, the web page is manipulated by Javascript calls to the WebView.

The logic expects that the webpage is displayed in Hungarian.

[Xamarin.Essentials Secure Storage](https://docs.microsoft.com/en-us/xamarin/essentials/secure-storage) is used to store credentials securely, if requested. No other data is stored.

## License

However the source code is licensed under the [MIT license](LICENSE), all rights of dijnet system reserved to _Díjnet Zrt._

You should always check https://www.dijnet.hu if your bills are paid properly.

## Magyar leírás

Xamarin.Forms applikáció Androidra és iOS-re dijnetes számlák befizetésére előre regisztrált bankkártyával.

Funkciók:
* számla lista megtekintése
* számla részletek megtekintése
* számla befizetésének elindítása regisztrált bankkártyával
* belépési adatok elmentése és automatikus bejelentkezés induláskor

Ez egy teszt alkalmazás, ezért mindig ellenőrizd, hogy a számláid be vannak fizetve a https://www.dijnet.hu weboldalon.

A forráskód [MIT licenc](LICENSE) alatt érhető el, de a dijnet rendszerhez minden jog fenntartva a _Díjnet Zrt._-nek.
