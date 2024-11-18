using DO;

namespace DalApi
{
    public interface ICrud<T> where T : class
    {
        void Create(T item); //Creates new entity object in DAL
        T Read(int id); //Reads entity object from DAL
        T? Read(Func<T, bool> filter); // Reads entity object with optional filtering
        IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // Reads all entity objects with optional filtering
        void Update(T item); //Updates entity object in DAL
        void Delete(int id); //Deletes entity object from DAL
        void DeleteAll(); //Delete all entity objects from DAL

    }
}




