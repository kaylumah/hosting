namespace Kaylumah.Ssg.Utilities
{
    public interface IPlugin
    {
        string Name { get; }
        string Render(object data);
    }
}