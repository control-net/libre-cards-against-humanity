using System;
using System.Collections.Generic;
using System.Text;

namespace LibreCards.Core.Entities
{
    public class Template
    {
        public const string BlankPlaceholder = "<BLANK>";

        public Template(string content)
        {
            if(content is null)
                throw new ArgumentNullException(nameof(content));

            if(string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Template's content cannot be empty", nameof(content));

            if(!content.Contains(BlankPlaceholder))
                throw new ArgumentException($"Template's content must contain at least one '{BlankPlaceholder}' placeholder.", nameof(content));

            Content = content;
            BlankCount = content.Split(new[] { BlankPlaceholder }, StringSplitOptions.None).Length - 1;
        }

        public string Content { get; private set; }

        public int BlankCount {  get; private set; }
    }
}
