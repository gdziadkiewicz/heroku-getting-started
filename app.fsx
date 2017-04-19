#if BOOTSTRAP
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
if not (System.IO.File.Exists "paket.exe") then let url = "https://github.com/fsprojects/Paket/releases/download/3.13.3/paket.exe" in use wc = new System.Net.WebClient() in let tmp = System.IO.Path.GetTempFileName() in wc.DownloadFile(url, tmp); System.IO.File.Move(tmp,System.IO.Path.GetFileName url);;
#r "paket.exe"
Paket.Dependencies.Install (System.IO.File.ReadAllText "paket.dependencies")
#endif

//---------------------------------------------------------------------

#I "packages/Suave/lib/net40"
#r "packages/Suave/lib/net40/Suave.dll"

open System
open System.IO
open Suave                 // always open suave
open Suave.Http
open Suave.Filters
open Suave.Successful // for OK-result
open Suave.Web             // for config
open System.Net
open Suave.Operators 

#if INTERACTIVE
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
#endif
printfn "initializing script..."

let basicAuth login password =
  Authentication.authenticateBasic ((=) (login, password))

let config = 
    let port = System.Environment.GetEnvironmentVariable("PORT")
    let ip127  = IPAddress.Parse("127.0.0.1")
    let ipZero = IPAddress.Parse("0.0.0.0")
    
    { defaultConfig with 
        bindings=[ (if port = null then HttpBinding.create HTTP ip127 (uint16 8080)
                    else HttpBinding.create HTTP ipZero (uint16 port)) ]
        homeFolder = Some (Path.GetFullPath "./public") }

printfn "starting web server..."

let introAuth = basicAuth "CryptoPaula@w84s3x" "43D3-AE1C"
let intro : WebPart =
    Files.file "Intro.html"
let obrazek =
    Files.file "NiezlaZabawa.jpg"
let giftAuth = basicAuth "bandit26@bandit.labs.overthewire.org" "5czgV9L3Xx8JPOyRbXh6lQbmIOWvPT6Z"
let gift : WebPart =
    Files.file  "Gift.html"

let gift2Auth = basicAuth "leviathan4@leviathan.labs.overthewire.org" "vuH0coox6m"
let gift2 : WebPart =
    let name = "Prezent.7z"
    Writers.addHeader "Content-Disposition" ("attachment; filename=\"" + name + "\"")
    >=> Files.sendFile "Gift2.7z" false

let fakeAuth = basicAuth "RESUEKAF" "ZAQ12WsxC"
let fakeLink : WebPart =
    Redirection.redirect "http://www.fakt.pl/wydarzenia/polityka/ipn-w-domu-wojciecha-jaruzelskiego-jaruzelski-wykiwal-wspolpracownikow/22njpeq.amp"
let app =  GET >=> choose
                [ 
                    path "/CA208BDC-2439-44DA-BE14-250C8F870C0E" >=> introAuth intro
                    path "/AB07D5DB-C0D8-43D3-AE1C-63B4D503EC30" >=> giftAuth  gift
                    path "/E426FCEC-9C95-4161-9748-41CEE14DE8E6" >=> gift2Auth gift2
                    path "/E426FCEC-9C95-4161-9749-41CEE14DE8E6" >=> introAuth obrazek
                    path "/D426FCAC-9C95-4161-9749-41CFE14DE8E6" >=> fakeAuth fakeLink
                ]

startWebServer config app
printfn "exiting server..."


