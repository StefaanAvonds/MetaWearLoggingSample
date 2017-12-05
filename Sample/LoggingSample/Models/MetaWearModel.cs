using MetaWear.NetStandard;

namespace LoggingSample.Models
{
    public class MetaWearModel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public MWDevice Device { get; private set; }

        public MetaWearModel(MWDevice device)
        {
            Id = device.Uuid.ToString("N");
            Name = device.Name;
            Device = device;
        }
    }
}
