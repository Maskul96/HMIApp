# HMIApp
## Aplikacja panela HMI (Human Manual Interface) do maszyn produkcyjnych. 
#### Instalacja niezbędnego oprogramowania
1. Należy pobrać SQL Server Management Studio 19
#### Konfiguracja niezbędnych plików do komunikacji z PLC
1. W folderze *HMIApp/Resources/Files* należy uzupełnić pliki csv **"tag_zone_X"** zgodnie ze swoją strukturą Data Blocków w maszynie - więcej informacji o tym jak to zrobić znajdziesz w zakładce **HMIApp wiki** 
2. W folderze *HMIApp* w pliku tekstowym **"DataBaseConfiguration.txt"** należy wrzucić bezpośrednio wygenerowany ConnectionString do bazy danych
3. W folderze *HMIApp/Resources/Machine* należy wrzucić screeny modułów 3D, które będą odzwierciedlały urządzenia sterujące maszyną - więcej informacji o tym jak to zrobić znajdziesz w zakładce **HMIApp wiki**
4. W folderze *HMIApp* w pliku tekstowym "PodpinanieTagowForm1.txt" znajdziesz krótki opis jak podpinać Tagi z plików csv **tag_zone_X** do kontrolek w Fomularzu Form1 - przykłady znajdziesz w zakładce **HMIApp wiki** 
5. Po wykonaniu powyższych konfiguracji plik wykonywalny *"HMIApp.exe"* jest gotowy do użycia
#### Dodatkowe informacje dotyczące użytkowania
1. W folderze *HMIApp/Resources/Files* znajdziesz plik **"document.xml"**. W tym pliku zapisywani są użytkownicy i ich uprawnienia - więcej informacji na temat struktury pliku znajdziesz w zakładce **HMIApp wiki**
2. W folderze *HMIApp/Resources/Files/Archivizations* generowane będą pliki csv, w których zapisane będą zdarzenia tj. przełączenie w tryb Auto/Manual, Logowanie czy Przezbrojenie
3. W folderze *HMIApp/Resources/Files/ArchivizationsExtended* generowane będą pliki csv, w których poza zapisem zdarzeń tak jak opisano to w podpunkcie numer 2 logowane są także parametry referencji w trakcie odpalenia eventu - więcej informacji na temat plików csv do archiwizacji znajdziesz w zakładcę **HMIApp wiki**
#### Funkcjonalności
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