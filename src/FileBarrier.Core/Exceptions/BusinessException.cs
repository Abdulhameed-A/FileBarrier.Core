using FileBarrier.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileBarrier.Core.Exceptions
{
    [Serializable]
    internal class BusinessException : Exception
    {
        public List<string> Errors { get; protected set; }

        public string SqlExceptionMessage { get; protected set; }

        public ErrorType ErrorType { get; set; }

        public BusinessException()
        {
        }

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, Exception inner) : base(message, inner)
        {

        }

        public BusinessException(string message, ErrorType errorType) : base(message)
        {
            this.ErrorType = errorType;
        }

        public BusinessException(List<string> erros)
        {
            this.Errors = erros;
        }

        protected BusinessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {

        }
    }
}