using DalApi;
using DO;
using System.Xml.Serialization;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{

    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (assignments.Any(a => a.Id == item.Id))
            throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists");
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignment_xml);
    }

    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (assignments.RemoveAll(a => a.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignment_xml);
    }

    public void DeleteAll()
    {
        List<Assignment> assignments = new(); // Create an empty list
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignment_xml); // Overwrite with an empty list
    }

    public Assignment Read(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        Assignment? assignment = assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");
        return assignment;
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        return assignments.FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        return filter == null ? assignments : assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignment_xml);
        if (assignments.RemoveAll(a => a.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignment_xml);
    }
}
