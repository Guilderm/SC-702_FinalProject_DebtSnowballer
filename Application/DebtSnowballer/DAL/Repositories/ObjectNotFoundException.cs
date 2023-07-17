﻿//using System.Data.Entity.Core;
using System.Runtime.Serialization;

namespace DAL.Repositories;
[Serializable]
internal class ObjectNotFoundException : Exception
    {
    public ObjectNotFoundException()
        {
        }

    public ObjectNotFoundException(string? message) : base(message)
        {
        }

    public ObjectNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    protected ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }