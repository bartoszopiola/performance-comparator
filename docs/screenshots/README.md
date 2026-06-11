# Przykładowe dane (samples)

Ten katalog zawiera gotowe pliki CSV z historią cen (NAV) pobrane ze Stooq, służące do
szybkiego wypełnienia aplikacji danymi po sklonowaniu repozytorium.

## Pliki

| Plik | Instrument | Symbol w aplikacji |
|---|---|---|
| `etfbw20tr.csv` | Beta ETF WIG20TR | ETFBW20TR |
| `etfbm40tr.csv` | Beta ETF mWIG40TR | ETFBM40TR |
| `etfbs80tr.csv` | Beta ETF sWIG80TR | ETFBS80TR |
| `etfsp500.csv` | Lyxor/Amundi ETF S&P 500 | ETFSP500 |
| `wig20.csv` | Indeks WIG20 (benchmark) | WIG20 |

## Jak wgrać

Pełna instrukcja krok po kroku znajduje się w głównym pliku `README.md`
w sekcji „Jak zaimportować dane”. W skrócie:

1. Zaloguj się jako administrator.
2. Utwórz klasy aktywów i fundusze (tabele w README).
3. Dla każdego funduszu kliknij **NAV** i wgraj odpowiedni plik z tego katalogu.

## Format

Pliki są w formacie Stooq:
```
Data,Otwarcie,Najwyzszy,Najnizszy,Zamkniecie,Wolumen
```
Importer aplikacji automatycznie rozpoznaje ten format i używa kolumny `Zamkniecie`
jako wartości NAV.

## Źródło

Dane pochodzą ze Stooq (https://stooq.pl). Wykorzystane wyłącznie w celach edukacyjnych.
