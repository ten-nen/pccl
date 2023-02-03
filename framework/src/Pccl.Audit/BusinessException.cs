using System;

namespace Pccl.Audit
{
    public class BusinessException : Exception, IBusinessException
    {
        public BusinessException(string message) :base(message)
        {

        }
    }
}
