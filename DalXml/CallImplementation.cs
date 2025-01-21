using DalApi;
using DO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Dal;

internal class CallImplementation : ICall
{

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (calls.Any(c => c.Id == item.Id))
            throw new DalAlreadyExistsException($"Call with ID={item.Id} already exists");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_call_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (calls.RemoveAll(c => c.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_call_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        List<Call> calls = new(); // Create an empty list
        XMLTools.SaveListToXMLSerializer(calls, Config.s_call_xml); // Overwrite with an empty list
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        Call? call = calls.FirstOrDefault(c => c.Id == id);
        if (call == null)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");
        return call;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        return calls.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        return filter == null ? calls : calls.Where(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (calls.RemoveAll(c => c.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exist");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_call_xml);
    }
}
