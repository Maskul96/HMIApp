# HMIApp
Aplikacja w VisualStudio 2022 .Net8.0 Windows Forms
Aplikacja jest panelem HMI(Human Manual Interface) maszyny dla operatora produkcyjnego, który komunikuje się ze sterownikami PLC firmy Siemens po protokole komunikacyjnym ProfiNet.
Aplikacja posiada generowanie wykresów Siły od przemieszczenia w czasie rzeczywistym, referencję zapisywane w bazie danych, obsługa logowania kartą RFID przez operatora, bazę danych użytkowników w pliku xml, archiwizację alarmów oraz archiwizacje zdarzeń takich jak: Przełączenie maszyny w tryb auto/manual oraz logowanie kartą i przezbrojenie maszyny.
Z poziomu aplikacji możlwie jest odczytywanie aktualnych danych z maszyny oraz sterowanie maszyną na podstawie wysyłanych sygnałów z aplikacji