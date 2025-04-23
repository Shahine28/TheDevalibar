[System.Serializable]
public class SerializablePort
{
    public string PortName;
    public PortDirection Direction;
    public PortCapacity Capacity;
    public bool IsPortNull = false;

    public SerializablePort(string portName, PortDirection direction, PortCapacity capacity, bool isPortNull = false)
    {
        PortName = portName;
        Direction = direction;
        Capacity = capacity;
        IsPortNull = isPortNull;
    }
}

public enum PortDirection
{
    Input,
    Output
}

public enum PortCapacity
{
    Single,
    Multi
}