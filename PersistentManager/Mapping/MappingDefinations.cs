namespace PersistentManager.Mapping
{
    public enum Cascade
    {
        NOTSET = 0,
        CREATE = 1,
        UPDATE = 2,
        DELETE = 3,
        ALL = 4,
    }

    public enum LoadType : int
    {
        Lazy = 0 ,
        Eager = 1,        
        Default = Lazy 
    }
}