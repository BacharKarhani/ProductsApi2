namespace ProductsApi.Models
{
    public class OperationResult
    {
        //public static readonly OperationResult Ok = new OperationResult();

        public OperationResult()
        {
        }

        //protected OperationResult(string errorCode)
        //{
        //    this.ErrorCode = errorCode;
        //}

        protected OperationResult(object extraData)
        {
            this.ExtraData = extraData;
        }

        protected OperationResult(string errorCode, object extraData)
        {
            this.ErrorCode = errorCode;
            this.ExtraData = extraData;
        }

        /// <summary>
        /// Gets or sets success
        /// </summary>
        public bool Success
        {
            get
            {
                return this.ErrorCode == null;
            }
        }

        public string ErrorCode
        {
            get;
            protected set;
        }

        //public static OperationResult Error(string errorCode)
        //{
        //    return new OperationResult(errorCode);
        //}

        public object ExtraData
        {
            get;
            protected set;
        }

        public static OperationResult Ok(object extraData = null)
        {
            return new OperationResult(extraData);
        }

        public static OperationResult Error(string errorCode, object extraData = null)
        {
            return new OperationResult(errorCode, extraData);
        }
    }
}
