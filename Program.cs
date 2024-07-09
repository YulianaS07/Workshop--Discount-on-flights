using System.Globalization;
internal class Program
{
    private static void Main(string[] args)
    {
        // Inicjalizacja zmiennych oraz słownika dla sprawdzenia, czy lot jest w sezonie

        DateTime dataUr = DateTime.MinValue;
        DateTime dataLotu = DateTime.MinValue;
        int rabat = 0;
        int wiek;
        string co = "";
        char lot;
        char klient;
        bool czyZarezerwowany;
        string typLotu = "";
        string trKlient = "";
        string psKlient = "";
        string sezon = "";

        Dictionary<string, Tuple<DateTime, DateTime>> sezony = new Dictionary<string, Tuple<DateTime, DateTime>>
        {
            { "Swieta", Tuple.Create(new DateTime(DateTime.Now.Year, 12, 20), new DateTime(DateTime.Now.Year + 1, 1, 10)) },
            { "Wiosna", Tuple.Create(new DateTime(DateTime.Now.Year, 3, 20), new DateTime(DateTime.Now.Year, 4, 10)) },
            { "Lato", Tuple.Create(new DateTime(DateTime.Now.Year, 7, 1), new DateTime(DateTime.Now.Year, 8, 31)) }
        };

        // Odczytanie wprowadzonej daty urodzenia
        // Konwersja do typu DateTime
        // Sprawdzenie formatu wprowadzonych danych (czy występuje null)

        Console.Write("Podaj swoją datę urodzenia w formacie RRRR-MM-DD: ");
        while (true)
        {
            string wDataUr = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(wDataUr) && DateTime.TryParseExact(wDataUr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataUr) && dataUr <= DateTime.Today)
            {
                wiek = Wiek(dataUr);
                co = (wiek >= 105) ? "???" : "";
                break;
            }
            Console.Write(string.IsNullOrEmpty(wDataUr) ? "Nic nie wpisałeś! Spróbuj jeszcze raz: " :
                (dataUr > DateTime.Today) ? "Wpisałeś przyszłą datę! Spróbuj jeszcze raz: " :
                "Zły format! Spróbuj jeszcze raz: ");
        }

        // Odczytanie wprowadzonej daty lotu
        // Konwersja do typu DateTime
        // Sprawdzenie formatu wprowadzonych danych (czy występuje null)

        Console.Write("Podaj datę lotu w formacie RRRR-MM-DD: ");
        while (true)
        {
            string wDataLotu = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(wDataLotu) && DateTime.TryParseExact(wDataLotu, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataLotu) && dataLotu > DateTime.Today && dataLotu.Year <= 2026)
            {
                sezon = Sezon(dataLotu, sezony);
                czyZarezerwowany = CzyLotZarezerwowany(dataLotu);
                break;
            }
            Console.Write(string.IsNullOrEmpty(wDataLotu) ? "Nic nie wpisałeś! Spróbuj jeszcze raz: " :
                (DateTime.TryParseExact(wDataLotu, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataParse) && (dataParse <= DateTime.Today || dataParse.Year > 2026)) ? "Podałeś datę lotu, która już minęła lub na którą nie ma lotów! Spróbuj jeszcze raz: " :
                "Zły format! Spróbuj jeszcze raz: ");
        }

        // Odczytanie wprowadzonej litery w celu ustalenia rodzaju lotu
        // Sprawdzenie formatu wprowadzonych danych (czy występuje null)

        Console.Write("Czy lot jest krajowy (T/N)? ");
        while (true)
        {
            string wLot = Console.ReadLine()?.Trim().ToUpper();
            if (!string.IsNullOrEmpty(wLot) && char.TryParse(wLot, out lot) && (lot == 'T' || lot == 'N'))
            {
                typLotu = (lot == 'T') ? "krajowy" : "międzynarodowy";
                break;
            }
            Console.Write(string.IsNullOrEmpty(wLot) ? "Nic nie wpisałeś! Spróbuj jeszcze raz: " : "Zły format! Spróbuj jeszcze raz: ");
        }

        // Odczytanie wprowadzonej litery w celu ustalenia, czy pasażer jest stałym klientem
        // Sprawdzenie formatu wprowadzonych danych (czy występuje null)

        Console.Write("Czy jesteś stałym klientem (T/N)? ");
        while (true)
        {
            string wKlient = Console.ReadLine()?.Trim().ToUpper();
            if (!string.IsNullOrEmpty(wKlient) && char.TryParse(wKlient, out klient) && (klient == 'T' || klient == 'N'))
            {
                trKlient = (klient == 'T' && wiek >= 18) ? "Tak" : "Nie";
                psKlient = (klient == 'T') ? "Tak" : "Nie";
                break;
            }
            Console.Write(string.IsNullOrEmpty(wKlient) ? "Nic nie wpisałeś! Spróbuj jeszcze raz: " : "Zły format! Spróbuj jeszcze raz: ");
        }

        // Obliczenie rabatu

        if (wiek < 2)
            rabat += (typLotu == "krajowy") ? 80 : 70;
        else if (wiek >= 2 && wiek <= 16)
            rabat += 10;

        if (czyZarezerwowany && typLotu == "krajowy" && rabat != 80)
            rabat += 10;

        if (typLotu == "międzynarodowy" && sezon == "Lot poza sezonem" && wiek >= 2)
            rabat += 15;
        else if (typLotu == "międzynarodowy" && sezon == "Lot poza sezonem" && wiek < 2)
            rabat += 10;

        if (typLotu == "krajowy" && trKlient == "Tak")
            rabat += 15;

        // Wydruk raportu

        Console.WriteLine($"\n=== Do obliczeń przyjęto:\n * Data urodzenia: {dataUr:dd.MM.yyyy} {co}\n * Data lotu: {dataLotu:dddd, d MMMM yyyy}. {sezon}\n * Lot {typLotu}\n * Stały klient: {psKlient}\n");
        Console.WriteLine($"Przysługuje Ci rabat w wysokości: {rabat}%\nData wygenerowania raportu: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }

    // Pomocnicze metody do obliczenia wieku, sprawdzenia,
    // czy lot jest w sezonie oraz czy jest zarezerwowany 

    static int Wiek(DateTime dataUrodzenia)
    {
        DateTime dzisiaj = DateTime.Today;
        int wiek = dzisiaj.Year - dataUrodzenia.Year;
        if (dzisiaj < dataUrodzenia.AddYears(wiek))
            wiek--;
        return wiek;
    }

    static string Sezon(DateTime data, Dictionary<string, Tuple<DateTime, DateTime>> sezony)
    {
        foreach (var kvp in sezony)
        {
            if (CzyDataNalezyDoOkresu(data, kvp.Value.Item1, kvp.Value.Item2))
                return "Lot w sezonie";
        }
        return "Lot poza sezonem";
    }

    static bool CzyDataNalezyDoOkresu(DateTime data, DateTime poczatekOkresu, DateTime koniecOkresu)
        => data >= poczatekOkresu && data <= koniecOkresu;

    static bool CzyLotZarezerwowany(DateTime dataLotu)
    {
        DateTime dzisiaj = DateTime.Now;
        int roznicaMiesiecy = ((dataLotu.Year - dzisiaj.Year) * 12) + dataLotu.Month - dzisiaj.Month;
        if (roznicaMiesiecy > 5)
            return true;
        else if (roznicaMiesiecy == 5)
            return dataLotu.Day >= dzisiaj.Day;

        return false;
    }

}

