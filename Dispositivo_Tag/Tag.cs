namespace Dispositivo_Tag
{
    public class Tag
    {
        public readonly string Id = "1";
        public string Name = "Alfa";
        public string button;

        public Tag() { }

        public Tag(string button)
        {
            this.button = button;            
        }

        public string BuildInfo(Tag tag)
        {
            return ">"+tag.Id+";"+tag.Name+";"+tag.button+"$";
        }

    }
}
