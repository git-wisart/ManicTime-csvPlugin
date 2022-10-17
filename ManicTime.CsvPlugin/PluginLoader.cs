using Finkit.ManicTime.Client.ActivityTimelines;
using Finkit.ManicTime.Shared.ObjectBuilding;
using Finkit.ManicTime.Plugins.Activities;
using Finkit.ManicTime.Plugins.Groups;
using Finkit.ManicTime.Plugins.Timelines;
using Finkit.ManicTime.Shared;
using Finkit.ManicTime.Shared.Ioc;
using Finkit.ManicTime.Shared.Plugins;
using Finkit.ManicTime.Shared.Plugins.ServiceProviders.Manager;
using Wizart.ManicTime.CsvPlugin.Wizard;
using System;

namespace Wizart.ManicTime.CsvPlugin
{
    //plugin definition
    [Plugin]
    public class PluginLoader
    {
        public const string SourceTypeName = "ManicTime/CsvPluginSource";
        public const string TimelineTypeName = "ManicTime/CsvPluginTimeline";
        private const string DefaultDisplayName = "ManicTime csvPlugin";

        public PluginLoader(PluginContext pluginContext,
            ITimelineTypeRegistry timelineTypeRegistry, ISourceTypeRegistry sourceTypeRegistry,
            IObjectBuilder objectBuilder, IServiceLocator serviceLocator)
        {
            var timelineType = new TimelineType(pluginContext.Spec, TimelineTypeName, () => DefaultDisplayName)
                .WithAllowMultipleInstances(true);

            timelineTypeRegistry.RegisterTimeline(timelineType);

            timelineTypeRegistry.RegisterTimelineFactory(TimelineTypeName,
                () => new PluginTimeline(timelineType, TimelineTypeName, TimelineTypeNames.GenericGroup,
                    t => t.TimelineType.GetDefaultDisplayName()));
            
            timelineTypeRegistry
                .RegisterTimelineEntityFactory(TimelineTypeName, () => new SingleGroupActivity((Activity activity) => "ActivityDisplayName"));

            timelineTypeRegistry.RegisterTimelineEntityFactory(TimelineTypeName, () => new Group((Group group) => "GroupDisplayName"));
            
            //register function for loading day view data
            sourceTypeRegistry.RegisterDayViewLoader(SourceTypeName, objectBuilder.Build<PluginDayViewLoader>());

            //register user inferface view
            sourceTypeRegistry.RegisterAddTimelineViewModelFactory(SourceTypeName, serviceLocator.GetInstance<AddPluginTimelineViewModel>);

            sourceTypeRegistry.RegisterSourceType(
                new SourceType(SourceTypeName, () => DefaultDisplayName, 6)
                .WithIcon32Path("pack://application:,,,/Wizart.ManicTime.CsvPlugin;component/Images/SourceImage.png")
                .WithGetDescription(() =>"Timeline plugin to show data from csv in its own timeline."));
        }

    }
}
