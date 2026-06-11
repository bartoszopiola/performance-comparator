# Instrukcja użytkownika — Performance Comparator

Instrukcja krok po kroku dla osoby korzystającej z aplikacji. Każdy krok jest ponumerowany.

---

## 1. Wprowadzenie

Performance Comparator to aplikacja do porównywania funduszy inwestycyjnych i ETF-ów. Pozwala
przeglądać fundusze, sprawdzać ich wyniki oraz porównywać kilka funduszy jednocześnie względem
wybranego punktu odniesienia (benchmarku).

![Strona główna](screenshots/home.png)

---

## 2. Przeglądanie funduszy

1. Na górnym pasku menu kliknij **Funds**.

![Menu z zaznaczonym Funds](image-12.png)

2. Zobaczysz listę funduszy w postaci kart. Każda karta pokazuje:
   - **logo** funduszu (lub jego symbol, jeśli brak logo),
   - **nazwę** funduszu,
   - **symbol** (np. ETFBW20TR),
   - **klasę aktywów** (np. Polish Equity — Large Cap),
   - **dostawcę** (np. AgioFunds TFI),
   - **zakres dat** dostępnych danych (np. 2022-01-03 — 2025-12-30).

![Lista funduszy](screenshots/funds.png)


3. Aby zawęzić listę, użyj rozwijanego pola **Filter by asset class** na górze i wybierz
   klasę aktywów. Lista odświeży się automatycznie.

   ![Rozwinięte pole filtra klasy aktywów](image-1.png)

4. Aby zobaczyć szczegóły funduszu, kliknij **View details** na jego karcie. Otworzy się
   strona z metrykami i wykresem.

   ![View details](image-2.png)
   ![Strona szczegółów funduszu z metrykami i wykresem](image-3.png)

---

## 3. Porównywanie funduszy

1. W górnym menu kliknij **Compare**.

   ![Menu z zaznaczonym  Compare](image-13.png)

2. W sekcji **Select funds (1–4)** wybierz fundusz z listy rozwijanej i kliknij **+ Add**.
   Wybrany fundusz pojawi się jako kolorowy „znaczek” (chip). Możesz dodać od 1 do 4 funduszy.

   ![Dropdown wyboru funduszu i przycisk Add](image-5.png)

3. Aby usunąć dodany fundusz, kliknij **×** na jego znaczku.

   ![Znaczki dodanych funduszy z przyciskiem x](image-6.png)

4. W sekcji **Benchmark** wybierz fundusz odniesienia (punkt porównania, np. indeks WIG20).

   ![Dropdown wyboru benchmarku](image-7.png)

5. Ustaw **Start date** i **End date** (zakres dat), aby ograniczyć analizę do wybranego
   okresu. Domyślnie ustawione są ostatnie 3 lata.

   ![Pola wyboru dat](image-8.png)

6. W polu **Risk-free rate (%)** wpisz stopę wolną od ryzyka (domyślnie 2). Wpływa ona na
   obliczenie niektórych metryk.

7. Kliknij **Compare**.

8. Pojawi się strona wyników. Na górze znajdziesz krótkie **podsumowanie tekstowe**
   wskazujące fundusz o najwyższym zwrocie, najlepszym wskaźniku Sharpe'a i najmniejszym
   obsunięciu.

   ![Podsumowanie tekstowe wyników](image-9.png)

9. Poniżej znajduje się **tabela metryk**. Każdy wiersz to jedna metryka, każda kolumna to
   jeden fundusz. **Zielone pole** oznacza najlepszą wartość w danym wierszu.

   ![Tabela metryk z zielonymi polami](image-10.png)

   Znaczenie metryk:
   - **CAGR** — średni roczny zysk funduszu.
   - **Volatility (zmienność)** — jak bardzo wahała się wartość funduszu; mniej = stabilniej.
   - **Max Drawdown (maks. obsunięcie)** — największy spadek od szczytu; bliżej zera = lepiej.
   - **Sharpe** — ile zysku przypada na jednostkę ryzyka; więcej = lepiej.
   - **Sortino** — jak Sharpe, ale uwzględnia tylko spadki; więcej = lepiej.
   - **Beta** — jak mocno fundusz rusza się względem benchmarku (1 = tak samo).
   - **Alpha** — o ile fundusz pobił benchmark; dodatnia = lepiej niż benchmark.
   - **Tracking Error** — jak bardzo fundusz odbiega od benchmarku.
   - **Information Ratio** — jakość „pobicia” benchmarku; więcej = lepiej.

10. Niżej znajdują się **wykresy**: skumulowanego zwrotu (jak rosła wartość, start = 100)
    oraz obsunięć kapitału (spadki od szczytu). Najedź kursorem na wykres, aby zobaczyć
    dokładne wartości.

    ![Wykres skumulowanego zwrotu i wykres obsunięć](image-11.png)

11. Aby wykonać nowe porównanie, kliknij **New comparison**.

---

## 4. Logowanie do panelu administracyjnego

1. W prawym górnym rogu kliknij **Login**.

   ![Przycisk Login w prawym górnym rogu](image-14.png)

2. Wpisz dane logowania administratora (znajdziesz je w pliku README projektu, w sekcji
   „Dane logowania do panelu administracyjnego”):
   - E-mail: `admin@local.test`
   - Hasło: `Admin123!`

   ![Formularz logowania](image-15.png)

3. Kliknij **Log in**.

4. Po zalogowaniu w prawym górnym rogu zobaczysz napis **Hello admin@local.test!**, a w menu
   pojawi się dodatkowy przycisk **Admin**. To znaczy, że jesteś zalogowany.

   ![Górny pasek po zalogowaniu z napisem Hello i przyciskiem Admin](image-16.png)

---

## 5. Zarządzanie funduszami

### Dodawanie nowego funduszu

1. Po zalogowaniu kliknij **Admin** w menu, a następnie **Manage Funds**
   (lub przejdź do panelu i wybierz kafelek Funds).

   ![Panel administracyjny z kafelkami](image-17.png)

2. Kliknij **Create New**.

   ![Lista funduszy w panelu admina z przyciskiem Create New](image-18.png)

3. Wypełnij formularz:
   - **Name** — nazwa funduszu,
   - **Symbol** — symbol (np. ETFBW20TR),
   - **Asset Class** — wybierz klasę aktywów z listy,
   - **Provider** — dostawca (opcjonalnie),
   - **Description** — opis (opcjonalnie),
   - **Currency** — waluta (np. PLN),
   - **Is Benchmark** — zaznacz, jeśli fundusz ma pełnić rolę benchmarku.

   ![Formularz tworzenia funduszu](image-19.png)

4. Kliknij **Create**. Fundusz pojawi się na liście.

### Edycja funduszu

5. Na liście funduszy kliknij **Edit** przy wybranym funduszu, zmień dane i kliknij **Save**.

   ![Przycisk Edit na liście funduszy](image-20.png)
   ![Formularz do edycji](image-21.png)

### Usuwanie funduszu

6. Na liście funduszy kliknij **Delete** przy wybranym funduszu. Pojawi się strona
   potwierdzenia (jeśli fundusz ma dane NAV, zobaczysz ostrzeżenie). Kliknij **Delete**, aby
   potwierdzić.

   ![Przycisk Delete](image-22.png)
   ![Strona potwierdzenia usunięcia funduszu](image-23.png)

---

## 6. Wgrywanie danych (NAV)

1. **Skąd wziąć plik CSV?** Masz dwie możliwości:

   **Możliwość A — gotowe pliki z repozytorium (zalecane)**
   W folderze `samples/` w repozytorium projektu znajdują się gotowe pliki CSV
   z historią cen dla wszystkich funduszy. Wystarczy je pobrać razem z projektem
   (są już na dysku po sklonowaniu).

   ![Folder samples/ z plikami CSV](image-24.png)

   **Możliwość B — pobranie ze Stooq przez stronę**
   Wejdź na stronę instrumentu na Stooq, np.:
   ```
   https://stooq.pl/q/d/?s=etfbw20tr.pl
   ```
   Przewiń stronę na dół i kliknij przycisk **„Pobierz dane w pliku CSV"**.

   ![Strona Stooq z przyciskiem pobierania CSV na dole](image.png)

   > **Uwaga:** bezpośrednie linki do pobierania (`/q/d/l/?s=...`) mogą wymagać
   > klucza API. Korzystaj ze strony instrumentu i przycisku na dole.

2. W panelu administracyjnym przejdź do **Manage Funds** i kliknij **NAV** przy funduszu,
   do którego chcesz wgrać dane.

   ![przycisk NAV na liście funduszy](image-4.png)

3. Kliknij **Choose file** (Wybierz plik) i wskaż pobrany plik CSV.

   ![Formularz wgrywania pliku CSV](image-25.png)

4. Kliknij **Upload**.

5. Po wgraniu pojawi się komunikat, np. *„Import complete. Added: 980, Skipped (duplicates): 0”*.
   - **Added** — liczba dodanych nowych notowań.
   - **Skipped (duplicates)** — liczba pominiętych, bo już istniały w bazie (te same daty).

   ![Komunikat o wyniku importu](image-26.png)

6. Jeśli wgrasz ten sam plik ponownie, wszystkie rekordy zostaną pominięte jako duplikaty —
   to normalne i bezpieczne.

---

## 7. Wgrywanie logo funduszu

1. W panelu administracyjnym przejdź do **Manage Funds** i kliknij **Edit** przy funduszu.

2. Przewiń do sekcji **Logo**. Jeśli fundusz ma już logo, zobaczysz jego podgląd.

   ![Sekcja Logo w formularzu edycji funduszu](image-27.png)

3. Kliknij **Choose file** i wskaż plik graficzny (PNG, JPG lub WEBP, maksymalnie 2 MB).

4. Kliknij **Save**. Nowe logo pojawi się na liście funduszy i na stronie szczegółów.

   ![Karta funduszu z wgranym logo](image-28.png)

> Wskazówka: najlepiej wyglądają logo w formacie PNG z przezroczystym tłem.

---

## 8. Edycja treści na stronie

1. W panelu administracyjnym kliknij **Manage Content**
   (lub kafelek **Content Blocks** na pulpicie).

   ![Lista bloków treści](image-29.png)

2. Zobaczysz listę bloków, np.:
   - `home.hero` — nagłówek na stronie głównej,
   - `home.intro` — sekcja „How it works”,
   - `about.body` — treść strony „O projekcie”.

3. Kliknij **Edit** przy wybranym bloku.

   ![Przycisk Edit przy bloku treści](image-30.png)

4. Zmień **Title** (tytuł) lub **Body** (treść) i kliknij **Save**.

   ![Formularz edycji bloku treści](image-31.png)

5. Wejdź na stronę publiczną (np. **Home**) — zmiana będzie widoczna od razu.

---

## 9. Najczęstsze problemy (FAQ)

**Nie widzę wykresu / metryk na stronie funduszu.**
Fundusz nie ma wgranych danych NAV albo ma mniej niż 2 notowania. Wgraj plik CSV
(patrz punkt 6). Do obliczeń potrzebne są co najmniej 2 rekordy.

**Po porównaniu pojawia się komunikat, że fundusz został pominięty.**
Wybrany zakres dat nie pokrywa się z danymi funduszu. Zmień zakres dat na taki, w którym
fundusz ma notowania.

**Import CSV pokazuje „Skipped (duplicates)” dla wszystkich rekordów.**
Te notowania już są w bazie. To normalne przy ponownym wgrywaniu tego samego pliku.

**Nie mogę wgrać pliku — komunikat o nieprawidłowym formacie.**
Upewnij się, że plik ma rozszerzenie `.csv` i format Stooq
(`Data,Otwarcie,...,Zamkniecie,Wolumen`) lub prosty (`date,value`).

**Nie widzę przycisku Admin w menu.**
Nie jesteś zalogowany jako administrator. Zaloguj się (patrz punkt 4).

**Logo funduszu się nie wgrało.**
Sprawdź, czy plik jest w formacie PNG, JPG lub WEBP i nie przekracza 2 MB.

**Benchmark nie został uwzględniony w porównaniu.**
Upewnij się, że wybrałeś benchmark z listy i że ma on dane w wybranym zakresie dat.
