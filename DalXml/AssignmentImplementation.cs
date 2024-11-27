using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    private static Assignment getAssignment(XElement a)
    {
        return new Assignment
        {
            Id = (int?)a.Element("Id") ?? throw new FormatException("Unable to parse Id"),
            CallId = (int?)a.Element("CallId") ?? throw new FormatException("Unable to parse CallId"),
            VolunteerId = (int?)a.Element("VolunteerId") ?? throw new FormatException("Unable to parse VolunteerId"),
            StartTime = (DateTime?)a.Element("StartTime") ?? throw new FormatException("Unable to parse StartTime"),
            EndTime = (DateTime?)a.Element("EndTime"),
            EndType = Enum.TryParse((string?)a.Element("EndType"), out EndType endType) ? endType : null
        };
    }

    private static XElement createAssignmentElement(Assignment a)
    {
        return new XElement("Assignment",
            new XElement("Id", a.Id),
            new XElement("CallId", a.CallId),
            new XElement("VolunteerId", a.VolunteerId),
            new XElement("StartTime", a.StartTime),
            new XElement("EndTime", a.EndTime),
            new XElement("EndType", a.EndType?.ToString())
        );
    }

    public void Create(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignment_xml);

        // Assign a unique ID using the auto-increment logic
        item = item with { Id = Config.NextAssignmentId };

        if (assignmentsRootElem.Elements().Any(a => (int?)a.Element("Id") == item.Id))
            throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists");

        assignmentsRootElem.Add(createAssignmentElement(item));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignment_xml);
    }

    public void Delete(int id)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignment_xml);

        XElement? assignmentElem = assignmentsRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == id);
        if (assignmentElem == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        assignmentElem.Remove();
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignment_xml);
    }

    public void DeleteAll()
    {
        // Create an empty root element with the correct name
        XElement assignmentsRootElem = new XElement("ArrayOfAssignment");

        // Save the empty XML file
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignment_xml);
    }

    public Assignment Read(int id)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignment_xml);

        XElement? assignmentElem = assignmentsRootElem.Elements()
            .FirstOrDefault(a => (int?)a.Element("Id") == id);

        if (assignmentElem == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        return getAssignment(assignmentElem);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignment_xml)
            .Elements()
            .Select(a => getAssignment(a))
            .FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = XMLTools.LoadListFromXMLElement(Config.s_assignment_xml)
            .Elements()
            .Select(a => getAssignment(a));

        return filter == null ? assignments : assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignment_xml);

        XElement? assignmentElem = assignmentsRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == item.Id);
        if (assignmentElem == null)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");

        assignmentElem.Remove(); // Remove the old assignment
        assignmentsRootElem.Add(createAssignmentElement(item)); // Add the updated assignment

        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignment_xml);
    }
}
