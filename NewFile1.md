
 ASP.Net

- ✅ verziókezelt API. Szemléltetés két különböző verziós API egyidejű kiszolgálásával.
    - nem HTTP header (pl. URL szegmens) alapján [7]

- ✅ külső online szolgáltatás (Twitter, Facebook, Google Maps, Bing Maps, stb.) integrálása a szerveroldali alkalmazásba klienskönyvtárral (pl. HttpClient) vagy SDK-va: Discord
  - SDK-val / REST API-val, authentikációt (pl. OIDC) végrehajtva. (3rd party Library használatával discord integráció) [7]
    
- ✅ Publikálás docker konténerbe és futtatás konténerből (Docker self hosted) [7]

- ✅ OpenAPI leíró (swagger) alapú dokumentáció
  - ✅ minden végpont kliens szempontjából releváns működése dokumentált, minden lehetséges válaszkóddal együtt [3]
  - ✅ az API-nak egyidejűleg több támogatott verziója van, mindegyik dokumentált és mindegyik támogatott verzió dokumentációja elérhető [+2]
    
EF:
 - MS SQL/LocalDB-től eltérő adatbáziskiszolgáló használata EF Core-ra
    - ✅ egyéb, EF Core v5 támogatott adatbázis [10] (Maria DB)
     
 - értékkonverter (value converter) alkalmazása EF Core leképezésben
    - ✅ saját value converter [5]


.NET Core részfunkciók alkalmazása
 - ✅ az EF Core működőképességét jelző health check végpont publikálása a Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore NuGet csomag használatával [3]

- unit tesztek készítése:
    - ✅ minimum 10 függvényhez: [7]
      
 
 
29/50


TODO:

* diagnosztika beépített vagy külső komponens segítségével **\[5-9\]**
    * legalább két célba, amiből legalább egy perzisztens (pl. fájl vagy adatbázis vagy külső szolgáltatás) **5**
    * struktúrált naplózás (structured logging) **+2**
    * fájl cél esetén rolling log (minden napon/héten/10 MB-onként új naplófájl) **+2**
    * az egyik cél egy külső naplózó szolgáltatás (pl. Azure Application Insights) **+2**

* platformfüggetlen kódbázisú szerveralkalmazás készítése és bemutatása legalább 2 operációs rendszeren az alábbiak közül: Windows, Linux, Mac, ARM alapú OS (Raspberry Pi). **\[7\]**

* külső osztálykönyvtár használata szerver oldalon. A külső komponens által megvalósított funkcionalitásért, képességért további pontszám nem adható. Nem számít ide a projekt generálásakor automatikusan bekerülő, illetve a Microsoft által készített, az alaptechnológiák függőségeit jelentő NuGet csomagok **\[7\]**

 - a unit tesztekben a mock objektumok injektálása 
