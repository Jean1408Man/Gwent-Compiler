namespace LogicalSide;

public class CustomList<T>
{
    public bool? Displayable;
    public bool? Removable;
    public CustomList(bool? Displayable, bool? Removable)
    {
        this.Displayable = Displayable;
        this.Removable = Removable;
    }
    
    public List<T> list = new List<T>();

    public void Add(T item)
    {
        // Operación adicional antes de agregar el elemento
        
        
        // Agregar el elemento a la lista interna
        list.Add(item);
    }

    public bool Remove(T item)
    {
        // Operación adicional antes de eliminar el elemento
        
        
        // Eliminar el elemento de la lista interna
        return list.Remove(item);
    }

    // Otros métodos que delegan a la lista interna
    public int Count => list.Count;
    public T this[int index] => list[index];
    public void Shuffle<T>()
    {
        int n = list.Count;
        Random random= new Random();
        while (n > 0)
        {
            n--;
            int k = random.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
    public T Pop<T>(List<T> list)
    {
        T obj= list[list.Count-1];
        list.RemoveAt(list.Count-1);
        return obj;
    }
    public void Push(T card)=> list.Add(card);
    public void SendBottom(T card)=> list.Insert(0,card);
}
