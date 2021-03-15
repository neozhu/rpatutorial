using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Tutorial.CustomMathFunction.Activities.Design.Designers;
using Tutorial.CustomMathFunction.Activities.Design.Properties;

namespace Tutorial.CustomMathFunction.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(Addition), categoryAttribute);
            builder.AddCustomAttributes(typeof(Addition), new DesignerAttribute(typeof(AdditionDesigner)));
            builder.AddCustomAttributes(typeof(Addition), new HelpKeywordAttribute(""));

            builder.AddCustomAttributes(typeof(XmlResolver), categoryAttribute);
            builder.AddCustomAttributes(typeof(XmlResolver), new DesignerAttribute(typeof(XmlResolverDesigner)));
            builder.AddCustomAttributes(typeof(XmlResolver), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
