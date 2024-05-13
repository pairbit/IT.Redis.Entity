namespace IT.Redis.Entity;

public interface IKeyReader
{
    byte[] ReadKey(IKeyRebuilder builder);
}