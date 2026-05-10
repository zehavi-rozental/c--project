namespace BlApi;

public static class Factory
{
    public static IBl Get() => new BL.BlImplementation.Bl();
}
