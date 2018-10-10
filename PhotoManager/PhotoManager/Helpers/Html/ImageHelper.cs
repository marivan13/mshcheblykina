using PhotoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace PhotoManager.Helpers.Html
{
    public static class ImageHelper
    {
        public static MvcHtmlString ImageWithLink(this HtmlHelper helper,string imageLink, string imageSrc, string imageStyle)
        {
            var link = new TagBuilder("a");
            link.MergeAttribute("href", imageLink);

            var image = new TagBuilder("img");
            image.AddCssClass("img-responsive");
            image.MergeAttribute("src", imageSrc);
            image.MergeAttribute("style", imageStyle);

            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append(link.ToString(TagRenderMode.StartTag));
            htmlBuilder.Append(image.ToString(TagRenderMode.Normal));
            htmlBuilder.Append(link.ToString(TagRenderMode.EndTag));
            return MvcHtmlString.Create(htmlBuilder.ToString());

        }
    }
}