using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

[Table("person")]
public class Person
{
    [Column("name")]
    public string Name { get; set; } = string.Empty;
}