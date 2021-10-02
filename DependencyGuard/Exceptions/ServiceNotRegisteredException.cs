using System;
using System.Runtime.Serialization;

namespace DependencyGuard.Exceptions
{
    /// <summary>
    /// Errors for services whose constructor dependencies are not registered.
    /// </summary>
    [Serializable]
    public class ServiceNotRegisteredException : Exception
    {
        public ServiceNotRegisteredException()
        {
        }

        public ServiceNotRegisteredException(string message) : base(message)
        {
        }

        public ServiceNotRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }


        protected ServiceNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
