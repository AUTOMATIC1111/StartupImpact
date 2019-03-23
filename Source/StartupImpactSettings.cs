using Verse;

namespace StartupImpact
{
    public enum ProfilerType
    {
        Ticks = 1,
        Stopwatch = 2,
        Date = 3,
    }

    public class StartupImpactSettings : ModSettings
    {
 
        public ProfilerType profilerType;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref profilerType, "profilerType", ProfilerType.Ticks);
        }
    }
}