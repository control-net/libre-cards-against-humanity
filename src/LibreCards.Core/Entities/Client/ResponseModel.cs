using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core.Entities.Client
{
    public class ResponseModel
    {
        public IEnumerable<string> CardTexts { get; set; }
        public int Id { get; set; }

        public static ResponseModel FromEntity(Response r)
        {
            return new ResponseModel
            {
                Id = r.Id,
                CardTexts = r.Cards.Select(c => c.Text)
            };
        }
    }
}
