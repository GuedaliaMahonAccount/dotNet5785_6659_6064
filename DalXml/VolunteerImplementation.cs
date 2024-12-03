using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal;

internal class VolunteerImplementation : IVolunteer
{

    private static Volunteer getVolunteer(XElement v)
    {
        return new Volunteer
        {
            Id = (int?)v.Element("Id") ?? throw new FormatException("Unable to parse Id"),
            Name = (string?)v.Element("Name") ?? "",
            Phone = (string?)v.Element("Phone") ?? "",
            Email = (string?)v.Element("Email") ?? "",
            IsActive = (bool?)v.Element("IsActive") ?? true,
            Role = Enum.TryParse((string?)v.Element("Role"), out Role role) ? role : Role.GeneralVolunteer,
            DistanceType = Enum.TryParse((string?)v.Element("DistanceType"), out DistanceType distanceType) ? distanceType : DistanceType.Foot,
            Password = (string?)v.Element("Password"),
            Address = (string?)v.Element("Address"),
            Latitude = (double?)v.Element("Latitude"),
            Longitude = (double?)v.Element("Longitude"),
            MaxDistance = (double?)v.Element("MaxDistance")
        };
    }

    private static XElement createVolunteerElement(Volunteer v)
    {
        return new XElement("Volunteer",
            new XElement("Id", v.Id),
            new XElement("Name", v.Name),
            new XElement("Phone", v.Phone),
            new XElement("Email", v.Email),
            new XElement("IsActive", v.IsActive),
            new XElement("Role", v.Role.ToString()),
            new XElement("DistanceType", v.DistanceType.ToString()),
            new XElement("Password", v.Password),
            new XElement("Address", v.Address),
            new XElement("Latitude", v.Latitude),
            new XElement("Longitude", v.Longitude),
            new XElement("MaxDistance", v.MaxDistance)
        );
    }

    public void Create(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteer_xml);

        if (volunteersRootElem.Elements().Any(v => (int?)v.Element("Id") == item.Id))
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

        volunteersRootElem.Add(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteer_xml);
    }

    public void Delete(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteer_xml);

        XElement? volunteerElem = volunteersRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == id);
        if (volunteerElem == null)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteer_xml);
    }

    public void DeleteAll()
    {
        // Create an empty root element with the correct name
        XElement volunteersRootElem = new XElement("ArrayOfVolunteer");

        // Save the empty XML file
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteer_xml);
    }

    public Volunteer Read(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteer_xml);

        XElement? volunteerElem = volunteersRootElem.Elements()
            .FirstOrDefault(v => (int?)v.Element("Id") == id);

        if (volunteerElem == null)
            return null;

        return getVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteer_xml)
            .Elements()
            .Select(v => getVolunteer(v))
            .FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLElement(Config.s_volunteer_xml)
            .Elements()
            .Select(v => getVolunteer(v));

        return filter == null ? volunteers : volunteers.Where(filter);
    }

    public void Update(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteer_xml);

        XElement? volunteerElem = volunteersRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.Id);
        if (volunteerElem == null)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");

        volunteerElem.Remove(); // Remove the old volunteer
        volunteersRootElem.Add(createVolunteerElement(item)); // Add the updated volunteer

        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteer_xml);
    }
}
