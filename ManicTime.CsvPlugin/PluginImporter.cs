using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Finkit.ManicTime.Plugins.Activities;
using Finkit.ManicTime.Plugins.Groups;
using Finkit.ManicTime.Shared.Helpers;
using Finkit.ManicTime.Shared.Logging;

namespace Wizart.ManicTime.CsvPlugin
{
    public static class PluginImporter
    {
        public static Activity[] GetData(PluginTimeline timeline, Func<Group> createGroup,
            Func<Activity> createActivity, DateTime fromLocalTime, DateTime toLocalTime)
        {
            /*
                get activities from your source for time range fromLocalTime-toLocalTime
                then create Activity objects and return the collection.
            */
            try
            {
                List<Activity> activities = new List<Activity>();
                int activityCounter = 0;
                var groupImported = CreateGroup("imported", "Imported", "ff0000", createGroup);

                if(File.Exists(timeline.SourceAddress))
                {
                    // Read fromTime, toTime and activityName from csv file
                    using (var reader = new StreamReader(timeline.SourceAddress))
                    {
                        // Read the csv file line by line
                        while (!reader.EndOfStream)
                        {
                            DateTimeOffset fromTime, toTime;
                            string activityName;

                            string line = reader.ReadLine();

                            // For now the separator is hardcoded to semicolon
                            string[] values = line.Split(";");

                            // Parse the first 2 rows to DateTimeOffset and take the 3rd as activity name
                            fromTime = DateTimeOffset.ParseExact(values[0], "dd/MM/yyyy HH.mm.ss zzz", CultureInfo.InvariantCulture);
                            toTime = DateTimeOffset.ParseExact(values[1], "dd/MM/yyyy HH.mm.ss zzz", CultureInfo.InvariantCulture);
                            activityName = values[2];

                            // Create the activity
                            Activity activity = CreateActivity("importedActivity-" + activityCounter, fromTime, toTime, activityName, groupImported, createActivity);
                            activities.Add(activity);
                            activityCounter++;
                        }
                    }
                } else
                {
                    ApplicationLog.WriteError("File '" + timeline.SourceAddress + "' does not exist.");
                }
                
                // return created activities
                return activities.ToArray();
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteError(ex);
                throw;
            }
        }

        private static Group CreateGroup(string id, string displayName, string color, Func<Group> createGroup)
        {
            var group = createGroup();
            group.Color = color.ToRgb();
            group.SourceId = id;
            group.DisplayName = displayName;
            return group;
        }

        private static Activity CreateActivity(string id, DateTimeOffset startTime, DateTimeOffset endTime, 
            string displayName, Group group, Func<Activity> createActivity)
        {
            var activity = createActivity();
            activity.StartTime = startTime;
            activity.EndTime = endTime;
            activity.SourceId = id;
            activity.Group = group;
            activity.DisplayName = displayName;
            return activity;
        }
    }
}
