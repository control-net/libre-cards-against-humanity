using System;

namespace LibreCards.Core.Entities.Client
{
    public class TemplateModel
    {
        public string Text { get; set; }

        public int RequiredResponseCardCount { get; set; }

        public static TemplateModel FromEntity(Template t)
        {
            if (t is null)
                return null;

            return new TemplateModel
            {
                Text = t.Content,
                RequiredResponseCardCount = t.BlankCount
            };
        }
    }
}
