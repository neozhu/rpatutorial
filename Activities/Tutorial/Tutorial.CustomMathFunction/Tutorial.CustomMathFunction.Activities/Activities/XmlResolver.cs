using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Tutorial.CustomMathFunction.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;

namespace Tutorial.CustomMathFunction.Activities
{
    [LocalizedDisplayName(nameof(Resources.XmlResolver_DisplayName))]
    [LocalizedDescription(nameof(Resources.XmlResolver_Description))]
    public class XmlResolver : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.XmlResolver_XPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.XmlResolver_XPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> XPath { get; set; }

        [LocalizedDisplayName(nameof(Resources.XmlResolver_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.XmlResolver_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataTable> Result { get; set; }

        [LocalizedDisplayName(nameof(Resources.XmlResolver_XmlString_DisplayName))]
        [LocalizedDescription(nameof(Resources.XmlResolver_XmlString_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> XmlString { get; set; }

        #endregion


        #region Constructors

        public XmlResolver()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (XPath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(XPath)));
            if (XmlString == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(XmlString)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var xpath = XPath.Get(context);
            var xmlstring = XmlString.Get(context);



            ///////////////////////////
            // Add execution logic HERE
            ///////////////////////////
            var xdoc = XDocument.Parse(xmlstring);
            var elements = xdoc.XPathSelectElements(xpath);
            var table = new DataTable();
            if(elements!=null && elements.Count() > 0)
            {
                var ele = elements.First();
                foreach(var field in ele.Elements())
                {
                    table.Columns.Add(new DataColumn() { ColumnName=field.Name.LocalName, DataType=typeof(object) });
                }
                foreach(var item in elements)
                {
                    
                  table.Rows.Add(item.Elements().Select(x=>x.Value).ToArray());
                }
            }
            // Outputs
            return (ctx) => {
                Result.Set(ctx, table);
            };
        }

        #endregion
    }
}

