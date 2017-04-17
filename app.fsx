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
open Suave                 // always open suave
open Suave.Http
open Suave.Filters
open Suave.Successful // for OK-result
open Suave.Web             // for config
open System.Net
open Suave.Operators 

printfn "initializing script..."

let basicAuth login password =
  Authentication.authenticateBasic ((=) (login, password))

let config = 
    let port = System.Environment.GetEnvironmentVariable("PORT")
    let ip127  = IPAddress.Parse("127.0.0.1")
    let ipZero = IPAddress.Parse("0.0.0.0")

    { defaultConfig with 
        bindings=[ (if port = null then HttpBinding.create HTTP ip127 (uint16 8080)
                    else HttpBinding.create HTTP ipZero (uint16 port)) ] }

printfn "starting web server..."


let introAuth = basicAuth "CryptoPaula" "Kotek"
let (intro : WebPart) =
    Files.sendFile "Intro.html" true

let giftAuth = basicAuth "Bandit26" "5czgV9L3Xx8JPOyRbXh6lQbmIOWvPT6Z"
let (gift : WebPart) =
    Files.sendFile "Gift.html" true

let gift2Auth = basicAuth "Leviathan4" "vuH0coox6m"
let (gift2 : WebPart) =
    Files.sendFile "Gift2.7z" true

let app =  GET >=> choose
                [ 
                    path "/CA208BDC-2439-44DA-BE14-250C8F870C0E" >=> introAuth intro
                    path "/AB07D5DB-C0D8-43D3-AE1C-63B4D503EC30" >=> giftAuth  gift
                    path "/E426FCEC-9C95-4161-9748-41CEE14DE8E6" >=> gift2Auth gift2
                ]

startWebServer config app
printfn "exiting server..."


