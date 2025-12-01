using System.ComponentModel.DataAnnotations.Schema;

namespace talearc_backend.src.data.entities;

/// <summary>
/// 人员实体
/// </summary>
[Table("person")]
public class Person
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Column("name")]
    public string Name { get; set; } = string.Empty;
}