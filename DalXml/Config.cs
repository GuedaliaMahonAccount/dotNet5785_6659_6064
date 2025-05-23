﻿

namespace Dal;
internal static class Config
{
    internal const string s_data_config_xml = "data-configs.xml";
    internal const string s_volunteer_xml = "volunteers.xml";
    internal const string s_call_xml = "calls.xml";
    internal const string s_assignment_xml = "assignments.xml";



    /// <summary>
    /// configures fields
    /// </summary>
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }
    internal static void Reset()
    {
        XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", 1);
        XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", 1);
        XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", DateTime.Now);
        XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", new TimeSpan(0, 0, 0));
    }
}

