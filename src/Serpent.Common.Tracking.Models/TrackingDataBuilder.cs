namespace Serpent.Common.Tracking.Models
{
    using System;

    /// <summary>
    /// Provides a way to create a new TrackingData instance, either as a copy of an existing one or as a brand new
    /// </summary>
    public struct TrackingDataBuilder
    {
        private string correlationId;

        private string operationId;

        private string parentOperationId;

        private string requestId;

        private bool parentOperationIdHasBeenSetExplicitly;

        /// <summary>
        /// Initializes a new instance of a TrackingData builder, copying the values from "source"
        /// </summary>
        /// <param name="source">The tracking data instance to copy</param>
        public TrackingDataBuilder(TrackingData source)
        {
            this.correlationId = source.CorrelationId;
            this.operationId = source.OperationId;
            this.parentOperationId = source.ParentOperationId;
            this.requestId = source.RequestId;
            this.parentOperationIdHasBeenSetExplicitly = false;
        }

        /// <summary>
        /// Appends a request id to the tracking data. If there is a request id already, a pipe (|) followed by the new request id
        /// </summary>
        /// <param name="newRequestId">The request id to add, or null to generate a new unique id to add</param>
        /// <returns>The tracking data builder</returns>
        public TrackingDataBuilder AppendRequestId(string newRequestId = null)
        {
            this.requestId = this.requestId + "|" + (newRequestId ?? Guid.NewGuid().ToString("D"));
            return this;
        }

        /// <summary>
        /// Sets the request id
        /// </summary>
        /// <param name="newRequestId">The new request id, or null to generate a new unique id</param>
        /// <returns>The tracking data builder</returns>
        public TrackingDataBuilder SetRequestId(string newRequestId = null)
        {
            this.requestId = newRequestId ?? Guid.NewGuid().ToString("D");
            return this;
        }

        /// <summary>
        /// Sets the operation id and copies the current operationid to ParentOperationId, unless ParentOperationId has been explicity set
        /// </summary>
        /// <param name="newOperationId">The current operation id, or null to generate a new unique id</param>
        /// <returns>The tracking data builder</returns>
        public TrackingDataBuilder SetOperationId(string newOperationId = null)
        {
            if (this.parentOperationIdHasBeenSetExplicitly == false)
            {
                this.parentOperationId = this.operationId;
            }

            this.operationId = newOperationId;
            return this;
        }

        /// <summary>
        ///  Sets the parent operation id
        /// </summary>
        /// <param name="newParentOperationId">The parent operation id</param>
        /// <returns>The tracking data builder</returns>
        public TrackingDataBuilder SetParentOperationId(string newParentOperationId)
        {
            this.parentOperationId = newParentOperationId;
            this.parentOperationIdHasBeenSetExplicitly = true;
            return this;
        }

        /// <summary>
        /// Sets the new correlation id
        /// </summary>
        /// <param name="newCorrelationId">The new correlation id or null to generate a new one</param>
        /// <returns>The tracking data builder</returns>
        public TrackingDataBuilder SetCorrelationId(string newCorrelationId = null)
        {
            this.correlationId = newCorrelationId ?? Guid.NewGuid().ToString("D");
            return this;
        }

        /// <summary>
        /// Builds a new instance of TrackingData from the builder parameters
        /// </summary>
        /// <returns></returns>
        public TrackingData Build()
        {
            return new TrackingData(this.correlationId ?? Guid.NewGuid().ToString("D"), this.requestId, this.operationId ?? Guid.NewGuid().ToString("D"), this.parentOperationId);
        }
    }
}