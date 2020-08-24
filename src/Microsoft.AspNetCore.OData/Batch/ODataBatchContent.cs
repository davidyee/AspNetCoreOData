﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Common;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OData;

namespace Microsoft.AspNetCore.OData.Batch
{
    /// <summary>
    /// Encapsulates a collection of OData batch responses.
    /// </summary>
    /// <remarks>
    /// In AspNet, <see cref="ODataBatchContent"/> derives from <see cref="IDisposable"/>.
    /// </remarks>
    public partial class ODataBatchContent
    {
        private IServiceProvider _requestContainer;
        private ODataMessageWriterSettings _writerSettings;

        /// <summary>
        /// Gets the batch responses.
        /// </summary>
        public IEnumerable<ODataBatchResponseItem> Responses { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataBatchContent"/> class.
        /// </summary>
        /// <param name="responses">The batch responses.</param>
        /// <param name="requestContainer">The dependency injection container for the request.</param>
        /// <remarks>This signature uses types that are AspNetCore-specific.</remarks>
        public ODataBatchContent(IEnumerable<ODataBatchResponseItem> responses, IServiceProvider requestContainer)
            : this(responses, requestContainer, null/*contentType*/)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ODataBatchContent"/> class.
        /// </summary>
        /// <param name="responses">The batch responses.</param>
        /// <param name="requestContainer">The dependency injection container for the request.</param>
        /// <param name="contentType">The response content type.</param>
        /// <remarks>This signature uses types that are AspNetCore-specific.</remarks>
        public ODataBatchContent(IEnumerable<ODataBatchResponseItem> responses, IServiceProvider requestContainer,
           string contentType)
        {
            Initialize(responses, requestContainer);

            // Set the Content-Type header for existing batch formats
            if (contentType == null)
            {
                contentType = String.Format(
                    CultureInfo.InvariantCulture, "multipart/mixed;boundary=batchresponse_{0}", Guid.NewGuid());
            }

            Headers[HeaderNames.ContentType] = contentType;
            ODataVersion version = _writerSettings.Version ?? ODataVersionConstraint.DefaultODataVersion;
            Headers.Add(ODataVersionConstraint.ODataServiceVersionHeader, ODataUtils.ODataVersionToString(version));
        }

        /// <summary>
        /// Gets the Headers for the batch content.
        /// </summary>
        public HeaderDictionary Headers { get; } = new HeaderDictionary();

        /// <summary>
        /// Serialize the batch content to a stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        /// <remarks>This function uses types that are AspNetCore-specific.</remarks>
        public Task SerializeToStreamAsync(Stream stream)
        {
            IODataResponseMessage responseMessage = ODataMessageWrapperHelper.Create(stream, Headers, _requestContainer);
            return WriteToResponseMessageAsync(responseMessage);
        }

        /// <summary>
        ///  Serialize the batch responses to an <see cref="IODataResponseMessage"/>.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <returns></returns>
        private async Task WriteToResponseMessageAsync(IODataResponseMessage responseMessage)
        {
            ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage, _writerSettings);
            ODataBatchWriter writer = await messageWriter.CreateODataBatchWriterAsync();

            await writer.WriteStartBatchAsync();

            foreach (ODataBatchResponseItem response in Responses)
            {
                await response.WriteResponseAsync(writer, /*asyncWriter*/ true);
            }

            await writer.WriteEndBatchAsync();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataBatchContent"/> class.
        /// </summary>
        /// <param name="responses">The batch responses.</param>
        /// <param name="requestContainer">The dependency injection container for the request.</param>
        private void Initialize(IEnumerable<ODataBatchResponseItem> responses, IServiceProvider requestContainer)
        {
            if (responses == null)
            {
                throw Error.ArgumentNull("responses");
            }

            Responses = responses;
            _requestContainer = requestContainer;
            _writerSettings = requestContainer.GetRequiredService<ODataMessageWriterSettings>();
        }
    }
}