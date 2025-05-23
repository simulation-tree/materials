using Unmanaged;

namespace Materials.Components
{
    public struct IsMaterialRequest
    {
        public ASCIIText256 address;
        public double timeout;
        public double duration;
        public Status status;

        public IsMaterialRequest(ASCIIText256 address, double timeout)
        {
            this.address = address;
            this.timeout = timeout;
            duration = 0;
            status = Status.Submitted;
        }

        public readonly IsMaterialRequest BecomeLoaded()
        {
            IsMaterialRequest request = this;
            request.status = Status.Loaded;
            return request;
        }

        public enum Status : byte
        {
            Submitted,
            Loading,
            Loaded,
            NotFound
        }
    }
}
