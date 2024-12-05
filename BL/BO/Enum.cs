namespace BO;


public enum Role
{
    Admin,                // System administrator with full platform access
    GeneralVolunteer,     // A general role for volunteers without a specific specialty
    Cook,                 // Prepares meals or food supplies for soldiers
    Driver,               // Transports supplies or personnel
    TeamLeader,           // Coordinates a group of volunteers for specific missions
    MedicalAssistant,     // Provides basic medical support or first aid (if qualified)
    Counselor,            // Provides emotional or mental health support
    LogisticsCoordinator, // Manages logistics for supply chains and operations
    EventOrganizer,       // Plans and organizes morale-boosting events
    LaundryHelper,        // Collects and washes soldiers' laundry
    EquipmentTechnician,  // Repairs or maintains essential equipment
    ITSupport,            // Helps with tech setup or troubleshooting
    Translator,           // Assists with multilingual communication and translations
    Tutor,                // Provides educational support or tutoring
    SupplyDistributor,    // Distributes essential supplies or hygiene kits
    CampSetupCrew,        // Sets up temporary camps or living spaces
    Cleaner,              // Cleans facilities or common areas used by soldiers
    VehicleMechanic,      // Provides maintenance and repairs for vehicles
    DonationCollector,    // Organizes and collects donations for needed supplies
    EmergencyResponder,   // Responds to urgent calls or emergencies
    CommunicationOfficer, // Manages communication logistics (e.g., radios, phones)
    CommunityLiaison,     // Connects with the local community for support or resources
    MoraleOfficer,        // Engages in activities to boost soldiers' morale
    HygieneCoordinator,   // Ensures hygiene supplies and cleanliness standards
    FirstAidTrainer,      // Trains soldiers or volunteers in first aid (if certified)
    SuppliesManager,      // Manages stock and distribution of supplies
    FieldMedic,           // Assists in more advanced medical support (if certified)
    DocumentationHelper,  // Assists with paperwork and record-keeping
    OutreachCoordinator,  // Manages outreach to donors and community supporters
    ResourceAllocator,    // Assigns resources based on priorities and needs
    SecurityVolunteer,    // Helps with securing areas or managing crowd control
    FoodSupplier,         // Sources and supplies ingredients or ready-made food
    RecreationCoordinator // Organizes recreational activities for relaxation
}
public enum DistanceType
{
    Plane,               // Straight-line (as-the-crow-flies) distance, often used in aviation
    Foot,                // Walking distance, accounting for paths and walkways
    Car,                 // Driving distance, following road networks
    Bike,                // Biking distance, considering bike paths and roads
    PublicTransport,     // Distance and route based on available public transit options
    Train,               // Distance covered by railway routes (if relevant to nearby train services)
    Helicopter,          // Direct distance assuming helicopter travel, suitable for hard-to-reach areas
    Boat,                // Distance by water, suitable for areas near water bodies or islands
    Horse,               // Distance covered by horseback routes (if applicable to rural or off-road areas)
    HikingTrail,         // Distance along dedicated hiking trails or paths
    Drone,               // Shortest path for drone delivery (similar to plane but for shorter ranges)
    Ski,                 // Distance by ski paths or slopes, relevant in snowy or mountainous areas
    Snowmobile,          // Distance on snowmobile trails, for remote or snowy regions
    OffRoadVehicle,      // Distance suitable for off-road vehicles, taking rough terrain into account
    Waterway,            // Distance via canals, rivers, or other navigable waterways
    Bus,                 // Distance following standard bus routes and stops
    Scooter,             // Distance accounting for paths accessible by scooter or small vehicles
    BicycleShare,        // Distance based on bike-sharing availability and bike paths
    Segway,              // Distance accessible by segway paths, typically in urban areas
    Rollerblade,         // Paths accessible for rollerblading or skating
    Skateboard,          // Routes where skateboards can travel, typically urban or park paths
    UrbanShortcuts,      // Distance considering pedestrian shortcuts within urban settings (e.g., alleys)
}
public enum CallType
{
    Open,              // The call is open and not currently assigned to any volunteer. It may have been previously assigned but was canceled by the volunteer or administrator.
    InTreatment,       // The call is currently being handled by a volunteer.
    Closed,            // A volunteer has completed the call.
    Expired,           // The call was not handled or was assigned but not completed within the allowed time.
    OpenAtRisk,        // The call is open and nearing its required completion time, within a risk time range defined by the administrator.
    InTreatmentAtRisk  // The call is currently in treatment but nearing its required completion time, within a risk time range defined by the administrator.
}
public enum EndType
{
    Completed,            // The call was successfully handled and completed before the maximum allowed time.
    SelfCanceled,         // The volunteer chose to cancel their assignment before the maximum allowed time.
    AdminCanceled,        // The administrator canceled the volunteer's assignment before the maximum allowed time.
    Expired               // The call was canceled as it was not handled before the maximum allowed time.
}

public enum TimeUnit
{
    MINUTE,
    HOUR,
    DAY,
    MONTH,
    YEAR
}

public enum VolunteerInListSortFields
{
    Id,
    Name,
    Phone,
    IsActive,
    Role,
    Latitude,
    Longitude,
}

