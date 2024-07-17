# Developer Guide

This is a help document with notes when you want to debug or extend the ability of this application.

## Layout

This repository comprises the following top level folders.

 - [Ac.Net.Authentication](../Ac.Net.Authentication): help project that manages the workflow of user context authentication (3-legged token) of APS.    
 - [ApsSettings.Data](../ApsSettings.Data):data models
 - [Bulk-Uploader-FrontEnd](../Bulk-Uploader-FrontEnd): main project, including user interfaces
 - [BulkUploaderUtils](../BulkUploaderUtils): help functions for upload/download
 - [Documentation](../Documentation): documentations and screenshots
 - [MigrationPlugin](../MigrationPlugin): This is project will have batch jobs controller to initiate the background jobs
 - [PluginBase](../PluginBase): This Plugins have models based out of Hangfire Btach jobs.
 - [TestPlugin](../TestPlugin): This is a Test project will help developer to test the scenarios and user/developers can add their own test cases if needed.


## Communication between UI and Backend

There are two ways to communicate between the host and the UI:
* REST calls
  * Standard ASP.net controllers may be used to send messages from the UI to the backend.
* PostMessage
  * The UI is able to send messages to the backend using `window.chrome.webview.postMessage(YOUR_MESSAGE_HERE)`.
    It is then processed by the `WebMessageReceived()` method in `Bulk-Uploader-FrontEnd/Views/MainForm.cs`.
  * The backend is able to respond using `this.webView21.CoreWebView2.PostWebMessageAsString(YOUR_MESSAGE_HERE)`. It 
    is then processed by the UI using `window.chrome.webview.addEventListener('message', event=>{ alert(event.data) })` 
    in `Bulk-Uploader-FrontEnd/ClientApp/src/App.tsx`.
  * This has been simplified on the front end with the `useMessageListener(messagename)` hook.

## React Notes

When using `<NavLink/>` to go between the SPA and other pages (such as the hangfire dashboard), you must use the `reloadDocument` parameter to force React Router to treat it as a normal `<a href="...">` element. 

## Database

This application uses Entity Framework. If you make any changes to the data structures, you'll need to run the following command from the `Bulk-Uploader-FrontEnd` folder:

  `dotnet ef migrations add NameOfYourMigrationHere --project "..\ApsSettings.Data\ApsSettings.Data.csproj"`

## Building
To build the application for a windows machine, use dotnet publish -r win-x64 while in the root solution folder.

