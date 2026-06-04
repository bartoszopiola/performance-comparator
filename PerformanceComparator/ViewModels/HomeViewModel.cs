namespace PerformanceComparator.ViewModels
{
    public class HomeViewModel
    {
        public string HeroTitle { get; set; } = "Performance Comparator";
        public string HeroBody { get; set; } =
            "Compare Polish investment funds (TFI) and ETFs by return and risk metrics.";

        public string IntroTitle { get; set; } = "How it works";
        public string IntroBody { get; set; } =
            "Browse funds, analyze their historical performance, and compare them against a chosen benchmark.";
    }

    public class AboutViewModel
    {
        public string Title { get; set; } = "About the project";
        public string Body { get; set; } =
            "Performance Comparator is an educational project presenting performance metrics for investment funds.";
    }
}