
[System.AttributeUsage(System.AttributeTargets.Field)]
public class DataType : System.Attribute
{
    public System.Type type;
    public DataType(System.Type type)
    {
        this.type = type;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class RIShow : System.Attribute
{
    public RIShow()
    {

    }
}