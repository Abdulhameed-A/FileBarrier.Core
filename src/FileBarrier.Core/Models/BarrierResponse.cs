using System;
using System.Collections.Generic;
using System.Text;

namespace FileBarrier.Core.Models
{
    public class BarrierResponse
    {
        public string ErrorMessage { get; set; }
        public bool IsAllowed { get; set; } = true;
        public ErrorType? ErrorType { get; set; }

        public BarrierResponse()
        {

        }

        public BarrierResponse(bool isAllowed, ErrorType? errorType)
        {
            IsAllowed = isAllowed;
            ErrorType = errorType;
        }

        public BarrierResponse(string errorMessage, bool isAllowed, ErrorType? errorType) : this(isAllowed, errorType)
        {
            ErrorMessage = errorMessage;
        }

        public void SetErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public void SetIsAllowed(bool isAllowed, ErrorType? errorType)
        {
            IsAllowed = isAllowed;
            ErrorType = errorType;
        }
    }
}