# Aper Bot

https://github.com/dpeter99/Aper_bot

## Feladat

Egy Discord bot készítése. A bot moduláris felépítéssel készül. A Bot tudja a követni a szerverenkét az idézeteket, illetve a szerver szabályzatát. illetve a discord új "slash commands" bot parancs szintaxisát.

## A kisháziban elérhető funkciók [adatmódosítással járó is legyen benne]

- Idézetek létrehozása
- Discord szerver szabályzat kezekése (Rules)
### Technikai
- A Modulokat késöbb lehessen betölteni dll ből.
- Moduláris adatbázis kezelés.


## Adatbázis entitások [min. 3 db.]
- guild
- user
- rule
- quote

## Alkalmazott alaptechnológiák [a szerver oldal mindenkinek ugyanez lesz, kliensoldal választható]
- adatelérés: Entity Framework Core v5
- kommunikáció, szerveroldal: ASP.NET Core v5
- kliensoldal: Discord

## Továbbfejlesztési tervek [opcionális, a pontrendszerből érdemes válogatni]
- Docker hosztolás
- Weblap az idézetek megjelenítésére
- A pluginek ddl ből való betöltése
