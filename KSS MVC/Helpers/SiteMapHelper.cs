using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace KSS.Helpers
{
    public static class SiteMapHelper
    {
        public static void RenderNavMenu(this HtmlHelper html)
        {
            HtmlTextWriter writer = new HtmlTextWriter(html.ViewContext.HttpContext.Response.Output);
            RenderRecursive(writer, SiteMap.RootNode);
        }

        private static void RenderRecursive(HtmlTextWriter writer, SiteMapNode node)
        {
            if (SiteMap.CurrentNode == node)
// Здесь мы подсвечиваем текущее наше местоположение, причем элемент будет иметь класс "current", что в связке с цсс даст нам богатые возможности по изменению стиля элемента, так же мы применяем тег "strong", чтобы придать ссылке больший вес (это из области SEO)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "current");
                writer.AddAttribute(HtmlTextWriterAttribute.Href, node.Url);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.RenderEndTag();
            }
            else
            {
// Рисуем ссылку (анкор)
                writer.AddAttribute(HtmlTextWriterAttribute.Href, node.Url);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
            }
            writer.Write(node.Title);
            writer.RenderEndTag();

            if (node.ChildNodes.Count > 0)
            {
                writer.WriteLine();
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                foreach (SiteMapNode child in node.ChildNodes)
                {
                        writer.RenderBeginTag(HtmlTextWriterTag.Li);
                        RenderRecursive(writer, child);
                        writer.RenderEndTag();
                        writer.WriteLine();
                }
                writer.RenderEndTag();
            }
        }
    }
}