
 ASP.Net
   - külső online szolgáltatás (Twitter, Facebook, Google Maps, Bing Maps, stb.) integrálása a szerveroldali alkalmazásba klienskönyvtárral (pl. HttpClient) vagy SDK-va: Discord
     - SDK-val / REST API-val, authentikációt (pl. OIDC) végrehajtva. (3rd party Library használatával discord integráció)
     - egyszerű REST API, SDK használat nélkül, egyszerű API kulcs alapú authentikáció
       (slash command use HTTP webhooks)
       
   - ✅ Publikálás docker konténerbe és futtatás konténerből (Docker self hosted) [7]

EF:
 - MS SQL/LocalDB-től eltérő adatbáziskiszolgáló használata EF Core-ra
    - ✅ egyéb, EF Core v5 támogatott adatbázis [10] (Maria DB)
 - értékkonverter (value converter) alkalmazása EF Core leképezésben
    - ✅ saját value converter [5]

 - unit tesztek készítése:
   - ✅ minimum 10 függvényhez: [7]

.NET Core részfunkciók alkalmazása
 - ✅ az EF Core működőképességét jelző health check végpont publikálása a Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore NuGet csomag használatával [3]

 - 7 pont:
 külső osztálykönyvtár használata szerver oldalon. A külső komponens által megvalósított funkcionalitásért, képességért további pontszám nem adható. Nem számít ide a projekt generálásakor automatikusan bekerülő, illetve a Microsoft által készített, az alaptechnológiák függőségeit jelentő NuGet csomagok
   
 
 
29/50


TODO:
 - verziókezelt API. Szemléltetés két különböző verziós API egyidejű kiszolgálásával.
   - nem HTTP header (pl. URL szegmens) alapján [7]
 - a unit tesztekben a mock objektumok injektálása 
