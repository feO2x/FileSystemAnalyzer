using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

public static class Paging
{
    public static bool CheckIfScrollIsNearTheEnd(double totalOffset,
                                                 double offsetDelta,
                                                 double viewportLength,
                                                 double extentLength,
                                                 double thresholdPercentage)
    {
        thresholdPercentage.MustBeIn(Range.FromExclusive(0.0).ToInclusive(1.0));
        
        if (offsetDelta < 0.0 || offsetDelta.IsApproximately(0.0))
            return false;

        var totalTravelledLength = totalOffset + viewportLength;
        return totalTravelledLength > extentLength * thresholdPercentage;
    }
}