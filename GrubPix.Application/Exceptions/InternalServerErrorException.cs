using System;

namespace GrubPix.Application.Exceptions
{
    public class InternalServerErrorException : Exception
    {
        public InternalServerErrorException(string message) : base(message) { }
    }
}