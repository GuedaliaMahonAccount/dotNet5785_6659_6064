

using DO;

namespace DalApi
{
    public interface ICrud<T> where T : class
    {
        void Create(T item); //Creates new entity object in DAL
        T Read(int id); //Reads entity object from DAL
        List<T> ReadAll(); //stage 1 only, Reads all entity objects
        void Update(T item); //Updates entity object in DAL
        void Delete(int id); //Deletes entity object from DAL
        void DeleteAll(); //Delete all entity objects from DAL

    }
}
