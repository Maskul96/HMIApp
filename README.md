# HMIApp
Aplikacja panela HMI (Human Manual Interface) do maszyny produkcyjnej. Aplikacja obsługuje komunikację po protokole ProfiNet ze sterownikami PLC marki Siemens.
Aplikacja pozwala na komunikację oepratora na linii produkcyjnej z maszyną.
Funkcjonalności aplikacji to:
Komunikacja operatora z maszyną poprzez właśnie aplikację panela HMI - Wysyłanie parametrów do sterownika PLC, sterowanie aktuatorami maszyny, odbieranie sygnałów z urządzeń
Wysyłanie sygnałów z panela do maszyny
Odbieranie sygnałów z maszyny
Wyświetlanie wykresu siły od przemieszczenia w czasie rzeczywistym
Logowanie użytkowników poprzez wpisanie loginu z klawiatury lub poprzez kartę RFID
Referencję zapisywane są w bazie danych i wyświetlane dla operatora i możliwe do edycji przez uprawnione osoby
Uprawnienia nadawane są z aplikacji, użytkownicy mogą być edytowani z poziomu aplikacji i wszystkie ich dane zapisywane są w pliku xml
Archiwizacja zdarzeń tj.: Przełączenie w tryb Auto/Manual, Logowanie, Przezbrojenie - każde zdarzenia archiwizuje opis, numer karty użytkownika i datę i godzinę
Archiwizacja została rozszerzona o archiwizacje parametrów procesowych w momencie wystąpienia powyższych zdarzeń
Aplikacja archiwizuje 100 alarmów, które wystąpią na maszynie