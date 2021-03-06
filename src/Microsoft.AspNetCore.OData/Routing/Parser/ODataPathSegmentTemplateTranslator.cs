﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNetCore.OData.Routing.Parser
{
    /// <summary>
    /// Translator an OData path to a path segment templates.
    /// </summary>
    public class ODataPathSegmentTemplateTranslator : PathSegmentTranslator<ODataSegmentTemplate>
    {
        /// <summary>
        /// Translate a TypeSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template</returns>
        public override ODataSegmentTemplate Translate(TypeSegment segment)
        {
            return new CastSegmentTemplate(segment);
        }

        /// <summary>
        /// Translate a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(NavigationPropertySegment segment)
        {
            return new NavigationSegmentTemplate(segment);
        }

        /// <summary>
        /// Translate an EntitySetSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(EntitySetSegment segment)
        {
            return new EntitySetSegmentTemplate(segment);
        }

        /// <summary>
        /// Translate an SingletonSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(SingletonSegment segment)
        {
            return new SingletonSegmentTemplate(segment);
        }

        /// <summary>
        /// Translate a KeySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(KeySegment segment)
        {
            return new KeySegmentTemplate(segment);
        }

        /// <summary>
        /// Translate a PropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(PropertySegment segment)
        {
            return new PropertySegmentTemplate(segment);
        }

        /// <summary>
        /// Handle a PropertySegment
        /// </summary>
        /// <param name="segment">the segment to handle</param>
        public override ODataSegmentTemplate Translate(PathTemplateSegment segment)
        {
            return new PathTemplateSegmentTemplate(segment);
        }

        /// <summary>
        /// Translate a OperationImportSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(OperationImportSegment segment)
        {
            if (segment == null)
            {
                throw Error.ArgumentNull(nameof(segment));
            }

            if (segment.OperationImports.First().IsActionImport())
            {
                return new ActionImportSegmentTemplate(segment);
            }

            IEdmFunctionImport functionImport = segment.OperationImports.First() as IEdmFunctionImport;

            // TODO: need process the optional parameter in the "Segment".
            return new FunctionImportSegmentTemplate(functionImport, segment.EntitySet);
        }

        /// <summary>
        /// Translate a OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(OperationSegment segment)
        {
            if (segment == null)
            {
                throw Error.ArgumentNull(nameof(segment));
            }

            IEdmOperation operation = segment.Operations.First();
            if (operation.IsAction())
            {
                return new ActionSegmentTemplate(operation as IEdmAction, segment.EntitySet);
            }

            // TODO: need process the optional parameter in the "Segment".
            return new FunctionSegmentTemplate(operation as IEdmFunction, segment.EntitySet);
        }

        /// <summary>
        /// Translate an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(DynamicPathSegment segment)
        {
            return new DynamicSegmentTemplate(segment);
        }

        /// <summary>
        /// Visit a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(NavigationPropertyLinkSegment segment)
        {
            return new NavigationPropertyLinkSegmentTemplate(segment);
        }

        /// <summary>
        /// Translate a CountSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(CountSegment segment)
        {
            return CountSegmentTemplate.Instance;
        }

        /// <summary>
        /// Translate a ValueSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(ValueSegment segment)
        {
            return new ValueSegmentTemplate(segment);
        }

        /// <summary>
        /// Translate a BatchSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(BatchSegment segment)
        {
            throw new ODataException(Error.Format(SRResources.TargetKindNotImplemented, "ODataPathSegment", "BatchSegment"));
        }

        /// <summary>
        /// Translate a MetadataSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(MetadataSegment segment)
        {
            return MetadataSegmentTemplate.Instance;
        }

        /// <summary>
        /// Translate a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Translated the path segment template.</returns>
        public override ODataSegmentTemplate Translate(BatchReferenceSegment segment)
        {
            throw new ODataException(Error.Format(SRResources.TargetKindNotImplemented, "ODataPathSegment", "BatchReferenceSegment"));
        }
    }
}
