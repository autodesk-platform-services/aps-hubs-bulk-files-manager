
$now = Get-Date -Format "yyyyMMdd"
$zipFileName = "$now - Bulk File Manager - APPLICATION BUNDLE.zip"

Compress-Archive `
-Path `
"./BUILD/ClientApp", `
"./BUILD/Bulk-File-Manager.exe", `
"./BUILD/WebView2Loader.dll", `
"./BUILD/e_sqlite3.dll", `
"./BUILD/v8-base-ia32.dll", `
"./BUILD/v8-base-x64.dll", `
"./BUILD/v8-ia32.dll", `
"./BUILD/v8-x64.dll", `
"./BUILD/v8-zlib-ia32.dll", `
"./BUILD/v8-zlib-x64.dll" `
-DestinationPath "$zipFileName" `
-Force
