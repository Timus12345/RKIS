using AvaloniaApplication1.Models;

namespace AvaloniaApplication1;

public class Service
{
    private static RevkoContext? _db;
    
    public static RevkoContext  GetDbContext()
    {
        if (_db == null)
        {
            _db = new RevkoContext();
        }
        return _db;
    }
}