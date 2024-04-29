# HMIApp
## Aplikacja panela HMI (Human Manual Interface) do maszyny produkcyjnej. 
### Funkcjonalności
1. Komunikacja po protokole ProfiNet ze sterownikami PLC marki Siemens.
2. Komunikacja operatora z maszyną poprzez aplikację panela HMI
    1. Wysyłanie parametrów do sterownika PLC
    2. Sterowanie aktuatorami maszyny
    3. Odbieranie sygnałów z urządzeń
3. Wysyłanie komend z panela do maszyny
4. Odbieranie sygnałów z maszyny
5. Wyświetlanie wykresu siły od przemieszczenia w czasie rzeczywistym
6. Logowanie użytkowników poprzez wpisanie loginu z klawiatury lub poprzez kartę RFID
7. Referencję zapisywane są w bazie danych i wyświetlane dla operatora i możliwe do edycji przez uprawnione osoby
8. Uprawnienia nadawane są z aplikacji, użytkownicy mogą być edytowani z poziomu aplikacji i wszystkie ich dane zapisywane są w pliku xml
9. Archiwizacja zdarzeń tj.:
    1. Przełączenie w tryb Auto/Manual
    2. Logowanie
    3. Przezbrojenie
        1. Każde zdarzenia archiwizuje opis, numer karty użytkownika i datę i godzinę
10. Archiwizacja została rozszerzona o archiwizacje parametrów procesowych w momencie wystąpienia powyższych zdarzeń
11. Aplikacja archiwizuje 100 alarmów, które wystąpią na maszynie