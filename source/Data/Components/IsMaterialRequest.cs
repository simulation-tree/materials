using System;
using Unmanaged;

namespace Materials.Components
{
    public struct IsMaterialRequest
    {
        public FixedString address;
        public TimeSpan timeout;
        public TimeSpan duration;
        public Status status;

        public IsMaterialRequest(FixedString address, TimeSpan timeout)
        {
            this.address = address;
            this.timeout = timeout;
            duration = TimeSpan.Zero;
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
