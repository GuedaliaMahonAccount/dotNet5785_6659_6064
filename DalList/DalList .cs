namespace Dal
{
    using DalApi;

    sealed internal class DalList : IDal
    {// Properties initialized with appropriate implementations

        public static IDal Instance { get; } = new DalList();
        private DalList() { }
        public IAssignment Assignment { get; } = new AssignmentImplementation();
        public ICall Call { get; } = new CallImplementation();
        public IVolunteer Volunteer { get; } = new VolunteerImplementation();
        public IConfig Config { get; } = new ConfigImplementation();

        // ResetDB method to reset all data
        public void ResetDB()
        {
            Assignment.DeleteAll();
            Call.DeleteAll();
            Volunteer.DeleteAll();
            Config.Reset();
        }
    }

}