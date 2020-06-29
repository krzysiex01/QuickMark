# QuickMark

## Opis

Quick Mark jest aplikacją przeznaczoną na komputery osobiste, pozwalającą na przeprowadzanie spersonalizowanych testów/ankiet w dowolnej sieci LAN.
Program pozwala min. na:
* wygodne tworzenie i edycje testów za pomocą intuicyjnego interfejsu
* równoległą obsługę kilku róznych arkuszy (grup testowych)
* śledzenie wyników na żywo wraz z ich zapisem do pliku (.csv, .xmls)
* automatyczne mieszanie kolejności pytań i odpowiedzi dla każdego użytkownika
* weryfikacja podobieństwa nadsyłanych formularzy/odpowiedzi (system FairPlay)
* możliwość drukowania testów do formy papierowej
* szeroką możliwość konfiguracji - od funkcji autozapisu czy ignorowania duplikatów do samodzielnego skonfigurowania prefixów URI

## Szczegóły techniczne

* .NET 4.7.2
* WPF
* Inne biblioteki min. MahApps.Metro, ControlzEx

Aplikacja wykorzystuje klasę System.Net.HttpListener do stworzenia prostego asynchronicznego serwera, przetwarzającego zapytania klientów, który jest rdzeniem całego programu.

Po wczytaniu testu i uruchomieniu serwera, test staje się widoczny dla wszystkich klientów (innych użądeń w tej samej sieci LAN) jako strona HTML.

Interfejs użytkownika został stworzony z wykorzystaniem wzorca projektowego MVVM.

Logiczny podział programu na 3 główne segmenty
* Tworzenie testu
* Przeprowadzanie testu
* Ustawienia

Testy jednostkowe tworzone z wykorzystaniem VisualStudio UnitTestFramework.
