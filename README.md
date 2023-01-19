**iCoin** je Web aplikacija koja prati promene cena kriptovaluta. U njoj se mogu registrovati novi korisnici koji ce moci da se pretplate na zeljene kriptovalute i da dobijaju obavestenja o promenama njihovih cena.

## Uputstvo za pokretanje
1. Potrebno je kreirati SQL bazu. SQL export se nalazi u repozitorijum u folderu "SQL" ili  u folderu projekta preko terminala sa komandama:

```
sqllocaldb create iCoin
sqllocaldb start iCoin
dotnet ef migrations add V1
dotnet ef database update
```

### Nacin rada aplikacije
Aplikacija preko *besplatnog API-ja prikuplja podatke o (reprezentativno 50) kriptovalutama. U servisu koji se odvija u pozadini (BackgroundService) se na 5 minuta kesiraju podaci o kriptovalutama, a nakon 24 sata se svi podaci dumpuju u sql bazu i brise se sve iz redisa. Na svakih 5 minuta kada se prikupe novi podaci sa API-ja preko pub/sub mehanizma redisa se serverskoj strani salju poruke u Message Queue-u, a potom preko socketa (SignalR) se u vidu obavestenja prikazuju na korisnickoj stranici korisnika koji je ulogovan novosti o kriptovalutama na kojima je korisnik pretplacen.
Na grafikonima su uzete vrednosti na osnovu poslednjih sat vremena ili 12 vrednosti.
Registrovanje i prijavljivanje je realizovano pomocu .NET Identity-ja.
*Timeout od 5 minuta je postavljen zato sto besplatan API osvezava podatke nakon tog intervala(moguce je pozivanje API-ja 50/minut, ali vraceni podaci su uvek isti).

![Index - logged in](wwwroot/assets/indexLoggedIn.jpg?raw=true)
Pocetna stranica na kojoj se vide podaci o kriptovalutama

![User](wwwroot/assets/UserWithNotify.jpg?raw=true)
Korisnicka stranica na kojoj se vide kriptovalute koje korisnik prati sa obavestenjima o njihovoj promeni u trenutku kada su pristigli novi podaci