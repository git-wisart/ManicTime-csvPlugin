﻿using System;
using Finkit.ManicTime.Client.Views;
using Finkit.ManicTime.Common.Core;
using Finkit.ManicTime.Shared;
using Finkit.ManicTime.Shared.Logging;
using System.Threading.Tasks;
using System.Windows;

namespace Wizart.ManicTime.CsvPlugin.Wizard
{
    public class AddPluginTimelineViewModel : WizardModel<TimelineEditorWizardState>
    {
        private readonly ITimelineFactory _timelineFactory;

        //title of timeline
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        //source file for timeline
        private string _file;
        public string File
        {
            get
            {
                return _file;
            }
            set
            {
                if (_file == value)
                    return;
                _file = value;
                OnPropertyChanged("File");
            }
        }
        

        public AddPluginTimelineViewModel(ITimelineFactory timelineFactory)
        {
            _timelineFactory = timelineFactory;
        }

        public override void OnViewLoaded(bool isReloaded)
        {
            if (State.EditedTimeline == null)
            {
                //set default state for add
                Title = "ManicTime csvPlugin";
                File = "C:\\Temp\\times.csv";
            }
            else
            {
                //load state for edit    
                Title = State.EditedTimeline.DisplayName;
                File = State.EditedTimeline.SourceAddress;
                var timeline = State.EditedTimeline as PluginTimeline;
                if (timeline == null)
                    return;
            }
        }

        private bool IsValid()
        {
            return !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(File);
        }

        public override WizardModel<TimelineEditorWizardState> GetNextPage()
        {
            return null;
        }

        public override Task<bool> CanMoveToNextPage()
        {
            return Task.Run(() => Application.Current.Dispatcher.Invoke(() => CreateOrUpdateTimeline()));
        }

        //function creates or update current timeline
        private bool CreateOrUpdateTimeline()
        {
            try
            {
                PluginTimeline timeline;

                if (!IsValid())
                    return false;
                
                if (State.EditedTimeline == null)
                {
                    timeline = (PluginTimeline)_timelineFactory.Create(PluginLoader.TimelineTypeName, TimelineTypeNames.GenericGroup);
                    timeline.SourceTypeName = PluginLoader.SourceTypeName;
                }
                else
                    timeline = (PluginTimeline)State.EditedTimeline;

                timeline.SourceAddress = File;
                timeline.DisplayName = Title;
                timeline.UpdateInterval = TimeSpan.FromMinutes(10);

                if (State.EditedTimeline == null)
                    State.AddTimeline(timeline);
                else
                    State.UpdateTimeline(timeline);

                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteError(ex);
                return false;
            }
        }


        public override bool IsLastPage
        {
            get { return true; }
        }
    }
}
