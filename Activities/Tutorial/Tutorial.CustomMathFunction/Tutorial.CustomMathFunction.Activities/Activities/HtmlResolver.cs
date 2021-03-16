using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Tutorial.CustomMathFunction.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using HtmlAgilityPack;
using System.Linq;

namespace Tutorial.CustomMathFunction.Activities
{
    [LocalizedDisplayName(nameof(Resources.HtmlResolver_DisplayName))]
    [LocalizedDescription(nameof(Resources.HtmlResolver_Description))]
    public class HtmlResolver : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.HtmlResolver_HtmlContent_DisplayName))]
        [LocalizedDescription(nameof(Resources.HtmlResolver_HtmlContent_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> HtmlContent { get; set; }

        [LocalizedDisplayName(nameof(Resources.HtmlResolver_XPath_DisplayName))]
        [LocalizedDescription(nameof(Resources.HtmlResolver_XPath_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> XPath { get; set; }

        [LocalizedDisplayName(nameof(Resources.HtmlResolver_FirstRowIsHeader_DisplayName))]
        [LocalizedDescription(nameof(Resources.HtmlResolver_FirstRowIsHeader_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> FirstRowIsHeader { get; set; }

        [LocalizedDisplayName(nameof(Resources.HtmlResolver_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.HtmlResolver_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataTable> Result { get; set; }

        #endregion


        #region Constructors

        public HtmlResolver()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (HtmlContent == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(HtmlContent)));
            if (XPath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(XPath)));
            if (FirstRowIsHeader == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FirstRowIsHeader)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var htmlContent = HtmlContent.Get(context);
            var xpath = XPath.Get(context);
            var firstRowIsHeader = FirstRowIsHeader.Get(context);

            ///////////////////////////
            // Add execution logic HERE
            ///////////////////////////
            ///


            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            var firsttrishead = true;
            var htmltable = htmlDoc.DocumentNode.SelectSingleNode(xpath);
            var datatable = new DataTable();
            if (htmltable != null)
            {
                var hasthead = htmltable.Descendants().Where(x => x.Name == "thead").Any();
                if (hasthead)
                {
                    foreach (var td in htmltable.Descendants("thead").First().Descendants().Where(x => x.Name == "td" || x.Name == "th"))
                    {
                        datatable.Columns.Add(HtmlEntity.DeEntitize(td.InnerText));
                    }
                }
                else if (firsttrishead)
                {
                    foreach (var td in htmltable.Descendants("tr").First().Descendants().Where(x => x.Name == "td" || x.Name == "th"))
                    {
                        datatable.Columns.Add(HtmlEntity.DeEntitize(td.InnerText));
                    }
                }
                else
                {
                    var fieldIndex = 0;
                    foreach (var td in htmltable.Descendants("tr").First().Descendants().Where(x => x.Name == "td" || x.Name == "th"))
                    {
                        datatable.Columns.Add($"Field{fieldIndex++}");
                    }
                }

                var hastbody = !htmltable.Descendants().Where(x => x.Name == "tbody").Any();
                if (hastbody)
                {
                    foreach (var tr in htmltable.Descendants("tbody").First().Descendants().Where(x => x.Name == "tr"))
                    {
                        var rowdata = tr.Descendants().Where(x => x.Name == "td" || x.Name == "th").Select(x => HtmlEntity.DeEntitize(x.InnerText));
                        datatable.Rows.Add(rowdata.ToArray());
                    }
                }
                else if (firsttrishead)
                {
                    var rowIndex = 0;
                    foreach (var tr in htmltable.Descendants().Where(x => x.Name == "tr"))
                    {
                        if (rowIndex++ > 0)
                        {
                            var rowdata = tr.Descendants().Where(x => x.Name == "td" || x.Name == "th").Select(x => HtmlEntity.DeEntitize(x.InnerText));
                            datatable.Rows.Add(rowdata.ToArray());
                        }

                    }
                }
                else
                {
                    foreach (var tr in htmltable.Descendants().Where(x => x.Name == "tr"))
                    {

                        var rowdata = tr.Descendants().Where(x => x.Name == "td" || x.Name == "th").Select(x => HtmlEntity.DeEntitize(x.InnerText));
                        datatable.Rows.Add(rowdata.ToArray());


                    }
                }


            }

            // Outputs
            return (ctx) => {
                Result.Set(ctx, datatable);
            };
        }

        #endregion
    }
}

