# QuickMark

## Opis

Quick Mark jest aplikacją przeznaczoną na komputery osobiste, pozwalającą na przeprowadzanie spersonalizowanych testów/ankiet w dowolnej sieci LAN.

## Funkcjonalności

* wygodne tworzenie i edycje testów za pomocą intuicyjnego interfejsu
* równoległą obsługę kilku róznych arkuszy (grup testowych) wraz z opcją automatycznej ich generacji
* obliczanie i podgląd wyników w czasie rzeczywistym
* zapisem wyników do pliku (.csv, .xmls)
* automatyczne mieszanie kolejności pytań i odpowiedzi dla każdego użytkownika
* weryfikacja podobieństwa nadsyłanych formularzy/odpowiedzi (system FairPlay)
* możliwość drukowania testów do formy papierowej
* szeroką możliwość konfiguracji - od funkcji autozapisu czy ignorowania duplikatów, do samodzielnego skonfigurowania prefixów URI

## Szczegóły techniczne

* DotNET 4.7.2
* WPF
* Inne biblioteki min. MahApps.Metro, ControlzEx

Aplikacja wykorzystuje klasę System.Net.HttpListener do stworzenia prostego asynchronicznego serwera, przetwarzającego zapytania klientów, który jest rdzeniem całego programu.

Po wczytaniu testu i uruchomieniu serwera, test staje się widoczny dla wszystkich klientów (innych użądeń w tej samej sieci LAN) jako strona HTML. Serwer w czasie rzeczywstym wyświetla uzyskane wyniki i obsługuje zapis do .csv i .xmls.

Interfejs użytkownika został stworzony z wykorzystaniem wzorca projektowego MVVM.

Logiczny podział programu na 3 główne segmenty
* Tworzenie testu
* Przeprowadzanie testu
* Ustawienia

Testy jednostkowe tworzone z wykorzystaniem VisualStudio UnitTestFramework.

## Screenshots

<p align="center">
  <img src="/Demo/sc1.jpg" width="320" height="200">
  <img src="/Demo/sc2.jpg" width="320" height="200">
  <img src="/Demo/sc3.jpg" width="320" height="200">
  <img src="/Demo/sc4.jpg" width="320" height="200">
  <img src="/Demo/sc5.jpg" width="320" height="200">
  <img src="/Demo/sc6.jpg" width="320" height="200">
</p>
