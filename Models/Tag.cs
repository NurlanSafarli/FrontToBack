using System.ComponentModel.DataAnnotations;

namespace FronyToBack.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }

    }
}
