namespace Serpent.Common.Tracking.Models
{
    /// <summary>
    ///     Tracks the chain of operations both in a single project/solution but also throughout a tree of operations, for
    ///     example micro services, service buses, web api's. TrackingData is immutable.
    /// </summary>
    public class TrackingData
    {
        /// <summary>
        ///     Initializes a new instance of the tracking data type
        /// </summary>
        /// <param name="correlationId">The correlation id</param>
        /// <param name="requestId">The request id</param>
        /// <param name="operationId">The current operation id</param>
        /// <param name="parentOperationId">The </param>
        public TrackingData(string correlationId = null, string requestId = null, string operationId = null, string parentOperationId = null)
        {
            this.CorrelationId = correlationId;
            this.RequestId = requestId;
            this.OperationId = operationId;
            this.ParentOperationId = parentOperationId;
        }

        /// <summary>
        ///     CorrelationId is used to identify an entire chain of events.
        ///     Set it to a unique identifier when tracking data is first created and never change it once it's set.
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        ///     The current operation unique id
        /// </summary>
        public string OperationId { get; }

        /// <summary>
        ///     The parent operation unique id
        /// </summary>
        public string ParentOperationId { get; }

        /// <summary>
        ///     Represents a chain of operations. Append your current operation id to RequestId.
        /// </summary>
        public string RequestId { get; }
    }
}
