namespace PerformanceComparator.ViewModels
{
    public class HomeViewModel
    {
        // Sensible defaults shown when the matching ContentBlock is missing.
        public string HeroTitle { get; set; } = "Performance Comparator";
        public string HeroBody { get; set; } =
            "Porównuj polskie fundusze inwestycyjne (TFI) oraz ETF-y według metryk zwrotu i ryzyka.";

        public string IntroTitle { get; set; } = "Jak to działa";
        public string IntroBody { get; set; } =
            "Przeglądaj fundusze, analizuj ich historyczne wyniki i porównuj je względem wybranego benchmarku.";
    }

    public class AboutViewModel
    {
        public string Title { get; set; } = "O projekcie";
        public string Body { get; set; } =
            "Performance Comparator to projekt edukacyjny prezentujący metryki wynikowe funduszy inwestycyjnych.";
    }
}